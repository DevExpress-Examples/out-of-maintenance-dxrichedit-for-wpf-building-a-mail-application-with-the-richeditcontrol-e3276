Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Windows

Imports DevExpress.Utils
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.RichEdit
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.Export
Imports DevExpress.XtraRichEdit.Services
Imports DevExpress.Office.Utils

Namespace RichEditSendMail
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Public Sub New()
			ThemeManager.ApplicationThemeName = "Office2007Silver"
			InitializeComponent()
			richEditControl1.LoadDocument("Hello.docx")
		End Sub

		Private Sub btnSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If (edtTo.Text.Trim() = "") OrElse (edtSubject.Text.Trim() = "") OrElse (edtSmtp.Text.Trim() = "") Then
				MessageBox.Show("Fill in required fields")
				Return
			End If

			Try
				Dim mailMessage As New MailMessage("DXRichEdit@devexpress.com", edtTo.Text)
				mailMessage.Subject = edtSubject.Text

				Dim exporter As New RichEditMailMessageExporter(richEditControl1, mailMessage)
				exporter.Export()

				Dim mailSender As New SmtpClient(edtSmtp.Text)
				'specify your login/password to log on to the SMTP server, if required
				'mailSender.Credentials = new NetworkCredential("login", "password");
				mailSender.Send(mailMessage)
				DXMessageBox.Show("Message sent", "RichEditSendMail", MessageBoxButton.OK, MessageBoxImage.Information)
			Catch exc As Exception
				DXMessageBox.Show(exc.Message)

			End Try
		End Sub
		Public Class RichEditMailMessageExporter
			Implements IUriProvider
			Private ReadOnly control As RichEditControl
			Private ReadOnly message As MailMessage
			Private attachments As List(Of AttachementInfo)
			Private imageId As Integer

			Public Sub New(ByVal control As RichEditControl, ByVal message As MailMessage)
				Guard.ArgumentNotNull(control, "control")
				Guard.ArgumentNotNull(message, "message")

				Me.control = control
				Me.message = message

			End Sub

			Public Overridable Sub Export()
				Me.attachments = New List(Of AttachementInfo)()

				Dim htmlView As AlternateView = CreateHtmlView()
				message.AlternateViews.Add(htmlView)
				message.IsBodyHtml = True
			End Sub

			Protected Friend Overridable Function CreateHtmlView() As AlternateView
				AddHandler control.BeforeExport, AddressOf OnBeforeExport
				Dim htmlBody As String = control.Document.GetHtmlText(control.Document.Range, Me)
				Dim view As AlternateView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html)
				RemoveHandler control.BeforeExport, AddressOf OnBeforeExport

				Dim count As Integer = attachments.Count
				For i As Integer = 0 To count - 1
					Dim info As AttachementInfo = attachments(i)
					Dim resource As New LinkedResource(info.Stream, info.MimeType)
					resource.ContentId = info.ContentId
					view.LinkedResources.Add(resource)
				Next i
				Return view
			End Function

			Private Sub OnBeforeExport(ByVal sender As Object, ByVal e As BeforeExportEventArgs)
				Dim options As HtmlDocumentExporterOptions = TryCast(e.Options, HtmlDocumentExporterOptions)
				If options IsNot Nothing Then
					options.Encoding = Encoding.UTF8
				End If
			End Sub


			#Region "IUriProvider Members"

			Public Function CreateCssUri(ByVal rootUri As String, ByVal styleText As String, ByVal relativeUri As String) As String Implements IUriProvider.CreateCssUri
				Return String.Empty
			End Function
			Public Function CreateImageUri(ByVal rootUri As String, ByVal image As OfficeImage, ByVal relativeUri As String) As String Implements IUriProvider.CreateImageUri
				Dim imageName As String = String.Format("image{0}", imageId)
				imageId += 1


				Dim imageFormat As OfficeImageFormat = GetActualImageFormat(image.RawFormat)
				Dim stream As Stream = New MemoryStream(image.GetImageBytes(imageFormat))
				Dim mediaContentType As String =OfficeImage.GetContentType(imageFormat)
				Dim info As New AttachementInfo(stream, mediaContentType, imageName)
				attachments.Add(info)

				Return "cid:" & imageName
			End Function

			Private Function GetActualImageFormat(ByVal OfficeImageFormat As OfficeImageFormat) As OfficeImageFormat
				If OfficeImageFormat = OfficeImageFormat.Exif OrElse OfficeImageFormat = OfficeImageFormat.MemoryBmp Then
					Return OfficeImageFormat.Png
				Else
					Return OfficeImageFormat
				End If
			End Function
			#End Region
		End Class

		Public Class AttachementInfo
			Private stream_Renamed As Stream
			Private mimeType_Renamed As String
			Private contentId_Renamed As String

			Public Sub New(ByVal stream As Stream, ByVal mimeType As String, ByVal contentId As String)
				Me.stream_Renamed = stream
				Me.mimeType_Renamed = mimeType
				Me.contentId_Renamed = contentId
			End Sub

			Public ReadOnly Property Stream() As Stream
				Get
					Return stream_Renamed
				End Get
			End Property
			Public ReadOnly Property MimeType() As String
				Get
					Return mimeType_Renamed
				End Get
			End Property
			Public ReadOnly Property ContentId() As String
				Get
					Return contentId_Renamed
				End Get
			End Property
		End Class
	End Class
End Namespace
