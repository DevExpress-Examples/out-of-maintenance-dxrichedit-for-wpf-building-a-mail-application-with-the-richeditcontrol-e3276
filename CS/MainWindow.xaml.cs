using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Windows;

using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;

namespace RichEditSendMail {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            ThemeManager.ApplicationThemeName = "Office2007Silver";
            InitializeComponent();
            richEditControl1.LoadDocument("Hello.docx");
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if ((edtTo.Text.Trim() == "") || (edtSubject.Text.Trim() == "") || (edtSmtp.Text.Trim() == "")) {
                MessageBox.Show("Fill in required fields");
                return;
            }

            try {
                MailMessage mailMessage = new MailMessage("DXRichEdit@devexpress.com", edtTo.Text);
                mailMessage.Subject = edtSubject.Text;

                RichEditMailMessageExporter exporter = new RichEditMailMessageExporter(richEditControl1, mailMessage);
                exporter.Export();

                SmtpClient mailSender = new SmtpClient(edtSmtp.Text);
                //specify your login/password to log on to the SMTP server, if required
                //mailSender.Credentials = new NetworkCredential("login", "password");
                mailSender.Send(mailMessage);
                DXMessageBox.Show("Message sent", "RichEditSendMail", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exc) {
                DXMessageBox.Show(exc.Message);

            }
        }
        public class RichEditMailMessageExporter : IUriProvider {
            readonly RichEditControl control;
            readonly MailMessage message;
            List<AttachementInfo> attachments;
            int imageId;

            public RichEditMailMessageExporter(RichEditControl control, MailMessage message) {
                Guard.ArgumentNotNull(control, "control");
                Guard.ArgumentNotNull(message, "message");

                this.control = control;
                this.message = message;

            }

            public virtual void Export() {
                this.attachments = new List<AttachementInfo>();

                AlternateView htmlView = CreateHtmlView();
                message.AlternateViews.Add(htmlView);
                message.IsBodyHtml = true;
            }

            protected internal virtual AlternateView CreateHtmlView() {
                control.BeforeExport += OnBeforeExport;
                string htmlBody = control.Document.GetHtmlText(control.Document.Range, this);
                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                control.BeforeExport -= OnBeforeExport;

                int count = attachments.Count;
                for (int i = 0; i < count; i++) {
                    AttachementInfo info = attachments[i];
                    LinkedResource resource = new LinkedResource(info.Stream, info.MimeType);
                    resource.ContentId = info.ContentId;
                    view.LinkedResources.Add(resource);
                }
                return view;
            }

            void OnBeforeExport(object sender, BeforeExportEventArgs e) {
                HtmlDocumentExporterOptions options = e.Options as HtmlDocumentExporterOptions;
                if (options != null) {
                    options.Encoding = Encoding.UTF8;
                }
            }


            #region IUriProvider Members

            public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
                return String.Empty;
            }
            public string CreateImageUri(string rootUri, RichEditImage image, string relativeUri) {
                string imageName = String.Format("image{0}", imageId);
                imageId++;


                RichEditImageFormat imageFormat = GetActualImageFormat(image.RawFormat);
                Stream stream = new MemoryStream(image.GetImageBytes(imageFormat));
                string mediaContentType =RichEditImage.GetContentType(imageFormat);
                AttachementInfo info = new AttachementInfo(stream, mediaContentType, imageName);
                attachments.Add(info);

                return "cid:" + imageName;
            }

             RichEditImageFormat GetActualImageFormat(RichEditImageFormat richEditImageFormat) {
                if (richEditImageFormat == RichEditImageFormat.Exif ||
                    richEditImageFormat == RichEditImageFormat.MemoryBmp)
                    return RichEditImageFormat.Png;
                else
                    return richEditImageFormat;
            }
            #endregion
        }

        public class AttachementInfo {
            Stream stream;
            string mimeType;
            string contentId;

            public AttachementInfo(Stream stream, string mimeType, string contentId) {
                this.stream = stream;
                this.mimeType = mimeType;
                this.contentId = contentId;
            }

            public Stream Stream { get { return stream; } }
            public string MimeType { get { return mimeType; } }
            public string ContentId { get { return contentId; } }
        }
    }
}
