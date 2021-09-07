<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3276)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/MainWindow.xaml.vb))
<!-- default file list end -->
# DXRichEdit for WPF: Building a mail application with the RichEditControl


<p>This example illustrates how the built-in document export functionality of the RichEditControl can be used to construct a simple application allowing you to send the document via email. A document, loaded into the RichEditControl, is sent in HTML email format.</p><p>To accomplish the task, we have to transform the document into the HTML formatted stream. Inline pictures will form a collection of <strong>linked resources</strong> (<strong>System.Net.Mail.LinkedResourceCollection</strong> ) for the email attachment. Common methods of the <strong>System.Net.Mail.MailMessage</strong> class are used to create a message. The message is sent with the help of the <strong>System.Net.Mail.SmtpClient</strong> class instance.</p><p>Let's elaborate a document export process further. To gain a control over RichEdit export, we construct a <strong>RichEditMailMessageExporter</strong> class, which implements the <a href="http://documentation.devexpress.com/#WindowsForms/clsDevExpressXtraRichEditServicesIUriProvidertopic"><strong><u>IUriProvider</u></strong></a> interface. This interface contains two methods -<a href="http://documentation.devexpress.com/#WindowsForms/DevExpressXtraRichEditServicesIUriProvider_CreateCssUritopic"><u> CreateCssUri</u></a> and <a href="http://documentation.devexpress.com/#WindowsForms/DevExpressXtraRichEditServicesIUriProvider_CreateImageUritopic"><u>CreateImageUri</u></a>. We use default CSS handling, so the <strong>CreateCssUri</strong> method always returns null. The <strong>CreateImageUri</strong> method is used to transform each document image into an object of the helper class - the AttachmentInfo class instance. An instance of this class contains an image's name, type and the data stream. An image is identified by its name, so this method returns a CID (Content-ID) URL containing the image name, to include a link to the image in the message body.<br />
The <strong>CreateHtmlView</strong> method of the RichEditMailMessageExporter gets the content of the document using the <a href="http://documentation.devexpress.com/#WindowsForms/DevExpressXtraRichEditAPINativeDocument_GetHtmlTexttopic"><strong><u>GetHtmlText</u></strong></a> method, handles the  <a href="http://documentation.devexpress.com/#WindowsForms/DevExpressXtraRichEditRichEditControl_BeforeExporttopic"><strong><u>BeforeExport</u></strong></a> event to specify encoding and creates the <strong>System.Net.Mail.AlternateView</strong> object required for HTML email format. Note that the document's HTML code contains CID URIs constructed using the CreateImageUri method in place of images. Then a collection of <strong>AttachmentInfo</strong> objects, representing document images, is used to fill a collection of embedded resources for the message.<br />
The <strong>Export</strong> method of the RichEditMailMessageExporter class calls the CreateHtmlView method to accomplish the major task, and adjusts the message parameters as required.<br />
Now, when the main functionality is incorporated into the RichEditMailMessageExporter class, the code for creating and sending the message looks simple and straightforward. Create a message, fill in 'from' and 'to' fields, create an exporter and call its 'Export' method, create an smtp client instance, specify the E-mail account settings and call the 'Send' method to send a message.</p><br />


<br/>


