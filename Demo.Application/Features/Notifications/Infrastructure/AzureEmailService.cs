using Azure;
using Azure.Communication.Email;
using Demo.Application.Domain.Settings;
using Demo.Application.Features.Notifications.Interfaces;
using Demo.Application.Features.Users.Models;
using System.Text;

namespace Demo.Application.Features.Notifications.Infrastructure;

public class AzureEmailService(IOptions<DemoSettings> settings, ILogger<AzureEmailService> logger) : IEmailService
{
    private readonly DemoSettings _settings = settings.Value;

    /// <summary>
    /// Sends an email to a user with a link to reset their password.
    /// </summary>
    /// <param name="user">User to send the message to</param>
    /// <param name="callbackUrl">Callback URL to reset their password</param>
    public async Task SendResetPasswordEmailAsync(AppUser user, string callbackUrl)
    {
        logger.LogDebug($"Params: UserId={user.Id}");

        StringBuilder content = new();
        content.AppendLine(AddParagraph($"Hello {user.FirstName},"));
        content.AppendLine(AddParagraph("Please reset your password by clicking on the button below. If you did not initiate a password reset, please ignore this email. <strong>This link will expire in 2 hours</strong>."));
        content.AppendLine(AddButton("Reset Password", callbackUrl));

        await SendEmailAsync(user, "Demo Reset Password Request", content.ToString());
    }

    /// <summary>
    /// Sends an email to the user with a subject and body
    /// </summary>
    /// <param name="user">User to send the email to</param>
    /// <param name="subject">Subject of the message</param>
    /// <param name="body">Body of the message</param>
    public async Task SendEmailAsync(AppUser user, string subject, string body)
    {
        logger.LogDebug($"Params: UserId={user.Id}, subject={subject}, enabled={_settings.Email.Enabled}");

        if (!_settings.Email.Enabled) return;

        // Create the email client
        string connectionString = _settings.Email.AzureCommunicationServicesConnectionString;
        EmailClient emailClient = new(connectionString);

        // Encapsulate body if needed
        if (!body.Contains("mceTextBlockContainer"))
        {
            body = AddContent(body);
        }

        // Replace any tags
        body = body.Replace("###FIRST_NAME###", user.FirstName);

        // Create the email content
        string template = BuildHtmlTemplate(body);
        EmailContent emailContent = new(subject)
        {
            Html = template
        };

        // Create the recipient list
        List<EmailAddress> to =
        [
            _settings.Email.DemoMode || user.Email!.EndsWith("@demo.com") ?
                new EmailAddress(_settings.Email.DemoSendToEmail, _settings.Email.AdminName) :
                new EmailAddress(user.Email, user.FullName),
        ];

        EmailRecipients emailRecipients = new(to);

        // Create the email message
        EmailMessage emailMessage = new(
            senderAddress: _settings.Email.FromEmail,
            emailRecipients,
            emailContent);

        // Send the message
        try
        {
            EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Started, emailMessage);
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, $"Error sending email {subject}");
            //SentrySdk.CaptureException(ex);
        }
    }

    /// <summary>
    /// Adds content to the email that can be added to the template
    /// </summary>
    /// <param name="content">Content to add</param>
    /// <returns>Content wrapped in a table row that can be added to the template</returns>
    private string AddContent(string content)
    {
        StringBuilder html = new();

        html.AppendLine("<tr>");
        html.AppendLine("   <td style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0\" valign=\"top\">");
        html.AppendLine("       <table width=\"100%\" style=\"border:0;border-radius:0;border-collapse:separate\">");
        html.AppendLine("           <tbody>");
        html.AppendLine("               <tr>");
        html.AppendLine("                   <td style=\"padding-left:24px;padding-right:24px;font-size: 16px;\" class=\"mceTextBlockContainer\">");
        html.AppendLine($"                      {content}");
        html.AppendLine("                   </td>");
        html.AppendLine("               </tr>");
        html.AppendLine("           </tbody>");
        html.AppendLine("       </table>");
        html.AppendLine("   </td>");
        html.AppendLine("</tr>");

        return html.ToString();
    }

    /// <summary>
    /// Adds a text paragraph to the email that can be added to the template
    /// </summary>
    /// <param name="content">Text to add</param>
    /// <returns>Content wrapped in a table row that can be added to the template</returns>
    private string AddParagraph(string content)
    {
        StringBuilder html = new();

        html.AppendLine("<tr>");
        html.AppendLine("   <td style=\"padding-top:0;padding-bottom:0;padding-right:0;padding-left:0\" valign=\"top\">");
        html.AppendLine("       <table width=\"100%\" style=\"border:0;border-radius:0;border-collapse:separate\">");
        html.AppendLine("           <tbody>");
        html.AppendLine("               <tr>");
        html.AppendLine("                   <td style=\"padding-left:24px;padding-right:24px;\" class=\"mceTextBlockContainer\">");
        html.AppendLine("                       <div data-block-id=\"3\" class=\"mceText\" id=\"dataBlockId-3\" style=\"width:100%\">");
        html.AppendLine($"                           <p {ParagraphStyle} class=\"last-child\">{content}</p>");
        html.AppendLine("                       </div>");
        html.AppendLine("                   </td>");
        html.AppendLine("               </tr>");
        html.AppendLine("           </tbody>");
        html.AppendLine("       </table>");
        html.AppendLine("   </td>");
        html.AppendLine("</tr>");

        return html.ToString();
    }

    /// <summary>
    /// Paragraph Style
    /// </summary>
    public string ParagraphStyle => "style=\"text-align: left; margin-top: 0 !important; font-size: 16px;\"";

    /// <summary>
    /// Adds a button to the email that can be added to the template
    /// </summary>
    /// <param name="text">Text of the button</param>
    /// <param name="url">URL the button will navigate to</param>
    /// <param name="buttonColor">Button color. Default is #256fa4</param>
    /// <param name="align">Alignment of the buttons. Default is center.</param>
    /// <returns>Content wrapped in a table row that can be added to the template</returns>
    private string AddButton(string text, string url, string buttonColor = "#256fa4", string align = "center")
    {
        StringBuilder html = new();

        html.AppendLine("<tr>");
        html.AppendLine("   <td style=\"padding-top:12px;padding-bottom:12px;padding-right:24px;padding-left:24px\" class=\"mceBlockContainer\" align=\"center\" valign=\"top\">");
        html.AppendLine($"       <table align=\"{align}\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" data-block-id=\"5\" class=\"mceButtonContainer\">");
        html.AppendLine("           <tbody>");
        html.AppendLine("               <tr>");
        html.AppendLine("                   <!--[if !mso]><!-->");
        html.AppendLine("                       </tr>");
        html.AppendLine("                       <tr class=\"mceStandardButton\">");
        html.AppendLine($"                           <td style=\"background-color:{buttonColor};border-radius:10px;text-align:center\" class=\"mceButton\" valign=\"top\">");
        html.AppendLine($"                               <a href=\"{url}\" target=\"_blank\" class=\"mceButtonLink\" style=\"background-color:{buttonColor};border-radius:10px;border:2px solid {buttonColor};color:#ffffff;display:block;font-family:'Helvetica Neue', Helvetica, Arial, Verdana, sans-serif;font-size:16px;font-weight:normal;font-style:normal;padding:16px 28px;text-decoration:none;min-width:30px;text-align:center;direction:ltr;letter-spacing:0px\" rel=\"noreferrer\">{text}</a>");
        html.AppendLine("                           </td>");
        html.AppendLine("                       </tr>");
        html.AppendLine("                       <tr>");
        html.AppendLine("                   <!--<![endif]-->");
        html.AppendLine("               </tr>");
        html.AppendLine("               <tr>");
        html.AppendLine("                   <!--[if mso]>");
        html.AppendLine("                       <td align=\"center\">");
        html.AppendLine("                           <v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\"");
        html.AppendLine("                               xmlns:w=\"urn:schemas-microsoft-com:office:word\"");
        html.AppendLine($"                               href=\"{url}\"");
        html.AppendLine("                               style=\"v-text-anchor:middle; width:167.64000000000001px; height:54px;\"");
        html.AppendLine("                               arcsize=\"30%\"");
        html.AppendLine($"                              strokecolor=\"{buttonColor}\"");
        html.AppendLine("                               strokeweight=\"2px\"");
        html.AppendLine($"                              fillcolor=\"{buttonColor}\">");
        html.AppendLine("                               <v:stroke dashstyle=\"solid\"/>");
        html.AppendLine("                               <w:anchorlock />");
        html.AppendLine("                               <center style=\"");
        html.AppendLine("                                   color: #ffffff;");
        html.AppendLine("                                   display: block;");
        html.AppendLine("                                   font-family: 'Helvetica Neue', Helvetica, Arial, Verdana, sans-serif;");
        html.AppendLine("                                   font-size: 16;");
        html.AppendLine("                                   font-style: normal;");
        html.AppendLine("                                   font-weight: normal;");
        html.AppendLine("                                   letter-spacing: 0px;");
        html.AppendLine("                                   text-decoration: none;");
        html.AppendLine("                                   text-align: center;");
        html.AppendLine("                                   direction: ltr;\"");
        html.AppendLine("                               >");
        html.AppendLine(text);
        html.AppendLine("                               </center>");
        html.AppendLine("                           </v:roundrect>");
        html.AppendLine("                       </td>");
        html.AppendLine("                   <![endif]-->");
        html.AppendLine("               </tr>");
        html.AppendLine("           </tbody>");
        html.AppendLine("       </table>");
        html.AppendLine("   </td>");
        html.AppendLine("</tr>");

        return html.ToString();
    }

    /// <summary>
    /// Builds the HTML template for the email
    /// </summary>
    /// <param name="content">Content to add as the body of the email</param>
    /// <returns>HTML that can be sent as an email</returns>
    private string BuildHtmlTemplate(string content)
    {
        StringBuilder html = new();

        html.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">");
        html.AppendLine("<head>");
        html.AppendLine("<!--[if gte mso 15]>");
        html.AppendLine("<xml>");
        html.AppendLine("<o:OfficeDocumentSettings>");
        html.AppendLine("<o:AllowPNG/>");
        html.AppendLine("<o:PixelsPerInch>96</o:PixelsPerInch>");
        html.AppendLine("</o:OfficeDocumentSettings>");
        html.AppendLine("</xml>");
        html.AppendLine("<![endif]-->");
        html.AppendLine("<meta charset=\"UTF-8\"/>");
        html.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"/>");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/>");
        html.AppendLine("<style>");
        html.AppendLine("img{-ms-interpolation-mode:bicubic;}");
        html.AppendLine("table, td{mso-table-lspace:0pt; mso-table-rspace:0pt;} ");
        html.AppendLine(".mceStandardButton, .mceStandardButton td, .mceStandardButton td a{mso-hide:all !important;}");
        html.AppendLine("p, a, li, td, blockquote{mso-line-height-rule:exactly;}");
        html.AppendLine("p, a, li, td, body, table, blockquote{-ms-text-size-adjust:100%; -webkit-text-size-adjust:100%;}");
        html.AppendLine("@media only screen and (max-width: 480px){");
        html.AppendLine("body, table, td, p, a, li, blockquote{-webkit-text-size-adjust:none !important;}");
        html.AppendLine("}");
        html.AppendLine(".mcnPreviewText{display: none !important;}");
        html.AppendLine(".bodyCell{margin:0 auto; padding:0; width:100%;}");
        html.AppendLine(".ExternalClass, .ExternalClass p, .ExternalClass td, .ExternalClass div, .ExternalClass span, .ExternalClass font{line-height:100%;};");
        html.AppendLine(".ReadMsgBody{width:100%;} .ExternalClass{width:100%;}");
        html.AppendLine("a[x-apple-data-detectors]{color:inherit !important; text-decoration:none !important; font-size:inherit !important; font-family:inherit !important; font-weight:inherit !important; line-height:inherit !important;}");
        html.AppendLine("body{height:100%; margin:0; padding:0; width:100%; background: #ffffff;}");
        html.AppendLine("p{margin:0; padding:0;}");
        html.AppendLine("table{border-collapse:collapse;}");
        html.AppendLine("td, p, a{word-break:break-word;}");
        html.AppendLine("h1, h2, h3, h4, h5, h6{display:block; margin:0; padding:0;}");
        html.AppendLine("img, a img{border:0; height:auto; outline:none; text-decoration:none;}");
        html.AppendLine("a[href^=\"tel\"], a[href^=\"sms\"]{color:inherit; cursor:default; text-decoration:none;}");
        html.AppendLine("li p {margin: 0 !important;}");
        html.AppendLine(".ProseMirror a {");
        html.AppendLine("pointer-events: none;");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 640px){");
        html.AppendLine(".mceClusterLayout td{padding: 4px !important;}");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px){");
        html.AppendLine("body{width:100% !important; min-width:100% !important; }");
        html.AppendLine("body.mobile-native {");
        html.AppendLine("-webkit-user-select: none; user-select: none; transition: transform 0.2s ease-in; transform-origin: top center;");
        html.AppendLine("}");
        html.AppendLine("body.mobile-native.selection-allowed a, body.mobile-native.selection-allowed .ProseMirror {");
        html.AppendLine("user-select: auto;");
        html.AppendLine("-webkit-user-select: auto;");
        html.AppendLine("}");
        html.AppendLine("colgroup{display: none;}");
        html.AppendLine("img{height: auto !important;}");
        html.AppendLine(".mceWidthContainer{max-width: 660px !important;}");
        html.AppendLine(".mceColumn{display: block !important; width: 100% !important;}");
        html.AppendLine(".mceColumn-forceSpan{display: table-cell !important; width: auto !important;}");
        html.AppendLine(".mceColumn-forceSpan .mceButton a{min-width:0 !important;}");
        html.AppendLine(".mceBlockContainer{padding-right:16px !important; padding-left:16px !important;}");
        html.AppendLine(".mceTextBlockContainer{padding-right:16px !important; padding-left:16px !important;}");
        html.AppendLine(".mceBlockContainerE2E{padding-right:0px; padding-left:0px;}");
        html.AppendLine(".mceSpacing-24{padding-right:16px !important; padding-left:16px !important;}");
        html.AppendLine(".mceImage, .mceLogo{width: 100% !important; height: auto !important;}");
        html.AppendLine(".mceFooterSection .mceText, .mceFooterSection .mceText p{font-size: 16px !important; line-height: 140% !important;}");
        html.AppendLine("}");
        html.AppendLine("div[contenteditable=\"true\"] {outline: 0;}");
        html.AppendLine(".ProseMirror h1.empty-node:only-child::before,");
        html.AppendLine(".ProseMirror h2.empty-node:only-child::before,");
        html.AppendLine(".ProseMirror h3.empty-node:only-child::before,");
        html.AppendLine(".ProseMirror h4.empty-node:only-child::before {");
        html.AppendLine("content: 'Heading';");
        html.AppendLine("}");
        html.AppendLine(".ProseMirror p.empty-node:only-child::before, .ProseMirror:empty::before {");
        html.AppendLine("content: 'Start typing...';");
        html.AppendLine("}");
        html.AppendLine(".mceImageBorder {display: inline-block;}");
        html.AppendLine(".mceImageBorder img {border: 0 !important;}");
        html.AppendLine("body, #bodyTable { background-color: rgb(244, 244, 244); }.mceText, .mcnTextContent, .mceLabel { font-family: \"Helvetica Neue\", Helvetica, Arial, Verdana, sans-serif; }.mceText, .mcnTextContent, .mceLabel { color: rgb(0, 0, 0); }.mceText p { margin-bottom: 0px; }.mceText label { margin-bottom: 0px; }.mceText input { margin-bottom: 0px; }.mceSpacing-24 .mceInput + .mceErrorMessage { margin-top: -12px; }.mceText p { margin-bottom: 0px; }.mceText label { margin-bottom: 0px; }.mceText input { margin-bottom: 0px; }.mceSpacing-12 .mceInput + .mceErrorMessage { margin-top: -6px; }.mceInput { background-color: transparent; border: 2px solid rgb(208, 208, 208); width: 60%; color: rgb(77, 77, 77); display: block; }.mceInput[type=\"radio\"], .mceInput[type=\"checkbox\"] { float: left; margin-right: 12px; display: inline; width: auto !important; }.mceLabel > .mceInput { margin-bottom: 0px; margin-top: 2px; }.mceLabel { display: block; }.mceText p, .mcnTextContent p { color: rgb(0, 0, 0); font-family: \"Helvetica Neue\", Helvetica, Arial, Verdana, sans-serif; font-size: 16px; font-weight: normal; line-height: 150%; text-align: center; direction: ltr; }.mceText a, .mcnTextContent a { color: rgb(0, 0, 0); font-style: normal; font-weight: normal; text-decoration: underline; direction: ltr; }");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceText p { margin: 0px; font-size: 16px !important; line-height: 150% !important; }");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceBlockContainer { padding-left: 16px !important; padding-right: 16px !important; }");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceButtonContainer { width: fit-content !important; max-width: fit-content !important; }");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceButtonLink { padding: 18px 28px !important; font-size: 16px !important; }");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceDividerBlock { border-top-width: 2px !important; }");
        html.AppendLine("}");
        html.AppendLine("@media only screen and (max-width: 480px) {");
        html.AppendLine(".mceDividerContainer { width: 100% !important; }");
        html.AppendLine("}");
        html.AppendLine("#dataBlockId-9 p, #dataBlockId-9 h1, #dataBlockId-9 h2, #dataBlockId-9 h3, #dataBlockId-9 h4, #dataBlockId-9 ul { text-align: center; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");

        html.AppendLine("<body>");
        html.AppendLine("   <center>");
        html.AppendLine("       <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"100%\" width=\"100%\" id=\"bodyTable\" style=\"background-color: rgb(244, 244, 244);\">");
        html.AppendLine("           <tbody>");
        html.AppendLine("               <tr>");
        html.AppendLine("                   <td class=\"bodyCell\" align=\"center\" valign=\"top\">");
        html.AppendLine("                       <table id=\"root\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">");
        html.AppendLine("                           <tbody data-block-id=\"13\" class=\"mceWrapper\">");
        html.AppendLine("                               <tr>");
        html.AppendLine("                                   <td align=\"center\" valign=\"top\" class=\"mceWrapperOuter\">");
        html.AppendLine("                                       <!--[if (gte mso 9)|(IE)]>");
        html.AppendLine("                                       <table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"660\" style=\"width:660px;\">");
        html.AppendLine("                                           <tr>");
        html.AppendLine("                                               <td>");
        html.AppendLine("                                       <![endif]-->");
        html.AppendLine("                                       <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width:660px\" role=\"presentation\">");
        html.AppendLine("                                           <tbody>");
        html.AppendLine("                                               <tr>");
        html.AppendLine("                                                   <td style=\"background-color:#ffffff;background-position:center;background-repeat:no-repeat;background-size:cover\" class=\"mceWrapperInner\" valign=\"top\">");
        html.AppendLine("                                                       <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" data-block-id=\"12\">");
        html.AppendLine("                                                           <tbody>");
        html.AppendLine("                                                               <tr class=\"mceRow\">");
        html.AppendLine("                                                                   <td style=\"background-position:center;background-repeat:no-repeat;background-size:cover\" valign=\"top\">");
        html.AppendLine("                                                                       <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\">");
        html.AppendLine("                                                                           <tbody>");
        html.AppendLine("                                                                               <tr>");
        html.AppendLine("                                                                                   <td style=\"padding-top:0;padding-bottom:0\" class=\"mceColumn\" data-block-id=\"-7\" valign=\"top\" colspan=\"12\" width=\"100%\">");
        html.AppendLine("                                                                                       <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\">");
        html.AppendLine("                                                                                           <tbody>");
        html.AppendLine("                                                                                               <tr>");
        html.AppendLine("                                                                                                   <td style=\"padding-top:12px;padding-bottom:0;padding-right:48px;padding-left:48px\" class=\"mceBlockContainer\" align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                       <a href=\"https://huddlemonkey.com\" style=\"display:block\" target=\"_blank\" data-block-id=\"2\">");
        html.AppendLine("                                                                                                           <span class=\"mceImageBorder\" style=\"border:0;border-radius:0;vertical-align:top;margin:0\">");
        html.AppendLine("                                                                                                               <img width=\"282\" height=\"auto\" style=\"width:282px;height:auto;max-width:282px !important;border-radius:0;display:block\" alt=\"\" src=\"https://mcusercontent.com/8471a9a5790d2d1b0a281195a/images/a2ae023d-8a85-31ee-3b89-c330d9554cf5.png\" class=\"mceLogo\"/>");
        html.AppendLine("                                                                                                           </span>");
        html.AppendLine("                                                                                                       </a>");
        html.AppendLine("                                                                                                   </td>");
        html.AppendLine("                                                                                               </tr>");
        html.AppendLine("                                                                                               <tr>");
        html.AppendLine("                                                                                                   <td style=\"background-color:transparent;padding-top:20px;padding-bottom:20px;padding-right:24px;padding-left:24px\" class=\"mceBlockContainer\" valign=\"top\">");
        html.AppendLine("                                                                                                       <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"background-color:transparent;width:100%\" role=\"presentation\" class=\"mceDividerContainer\" data-block-id=\"15\">");
        html.AppendLine("                                                                                                           <tbody>");
        html.AppendLine("                                                                                                               <tr>");
        html.AppendLine("                                                                                                                   <td style=\"min-width:100%;border-top-width:6px;border-top-style:solid;border-top-color:#256fa4\" class=\"mceDividerBlock\" valign=\"top\">");
        html.AppendLine("                                                                                                                   </td>");
        html.AppendLine("                                                                                                               </tr>");
        html.AppendLine("                                                                                                           </tbody>");
        html.AppendLine("                                                                                                       </table>");
        html.AppendLine("                                                                                                   </td>");
        html.AppendLine("                                                                                               </tr>");
        html.AppendLine(content);
        html.AppendLine("                                                                                               <tr>");
        html.AppendLine("                                                                                                   <td style=\"background-color:transparent;padding-top:20px;padding-bottom:20px;padding-right:24px;padding-left:24px\" class=\"mceBlockContainer\" valign=\"top\">");
        html.AppendLine("                                                                                                       <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"background-color:transparent;width:100%\" role=\"presentation\" class=\"mceDividerContainer\" data-block-id=\"6\">");
        html.AppendLine("                                                                                                           <tbody>");
        html.AppendLine("                                                                                                               <tr>");
        html.AppendLine("                                                                                                                   <td style=\"min-width:100%;border-top-width:6px;border-top-style:solid;border-top-color:#256fa4\" class=\"mceDividerBlock\" valign=\"top\">");
        html.AppendLine("                                                                                                                   </td>");
        html.AppendLine("                                                                                                               </tr>");
        html.AppendLine("                                                                                                           </tbody>");
        html.AppendLine("                                                                                                       </table>");
        html.AppendLine("                                                                                                   </td>");
        html.AppendLine("                                                                                               </tr>");
        html.AppendLine("                                                                                               <tr>");
        html.AppendLine("                                                                                                   <td style=\"padding-top:12px;padding-bottom:20px;padding-right:0;padding-left:0\" class=\"mceLayoutContainer\" valign=\"top\">");
        html.AppendLine("                                                                                                       <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" data-block-id=\"7\">");
        html.AppendLine("                                                                                                           <tbody>");
        html.AppendLine("                                                                                                               <tr class=\"mceRow\">");
        html.AppendLine("                                                                                                                   <td style=\"background-position:center;background-repeat:no-repeat;background-size:cover\" valign=\"top\">");
        html.AppendLine("                                                                                                                       <table border=\"0\" cellpadding=\"0\" cellspacing=\"24\" width=\"100%\" role=\"presentation\">");
        html.AppendLine("                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                   <td class=\"mceColumn\" data-block-id=\"-6\" valign=\"top\" colspan=\"12\" width=\"100%\">");
        html.AppendLine("                                                                                                                                       <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                   <td valign=\"top\">");
        html.AppendLine("                                                                                                                                                       <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" class=\"mceSocialFollowBlock\" data-block-id=\"-5\">");
        html.AppendLine("                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                   <td align=\"center\" valign=\"middle\">");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <table align=\"left\" border=\"0\" cellspacing= \"0\" cellpadding=\"0\">");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <td align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:inline;float:left\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                                   <td style=\"padding-top:3px;padding-bottom:3px;padding-left:12px;padding-right:12px\" class=\"mceSocialFollowIcon\" align=\"center\" width=\"32\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                                       <a href=\"https://www.facebook.com/huddlemonkey\" target=\"_blank\" rel=\"noreferrer\">");
        html.AppendLine("                                                                                                                                                                                           <img class=\"mceSocialFollowImage\" width=\"32\" height=\"32\" alt=\"\" src=\"https://cdn-images.mailchimp.com/icons/social-block-v3/block-icons-v3/facebook-filled-color-40.png\"/>");
        html.AppendLine("                                                                                                                                                                                       </a>");
        html.AppendLine("                                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </td>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <td align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:inline;float:left\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                                   <td style=\"padding-top:3px;padding-bottom:3px;padding-left:12px;padding-right:12px\" class=\"mceSocialFollowIcon\" align=\"center\" width=\"32\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                                       <a href=\"https://www.instagram.com/huddlemonkey/\" target=\"_blank\" rel=\"noreferrer\">");
        html.AppendLine("                                                                                                                                                                                           <img class=\"mceSocialFollowImage\" width=\"32\" height=\"32\" alt=\"\" src=\"https://cdn-images.mailchimp.com/icons/social-block-v3/block-icons-v3/instagram-filled-color-40.png\"/>");
        html.AppendLine("                                                                                                                                                                                       </a>");
        html.AppendLine("                                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </td>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <td align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:inline;float:left\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                                   <td style=\"padding-top:3px;padding-bottom:3px;padding-left:12px;padding-right:12px\" class=\"mceSocialFollowIcon\" align=\"center\" width=\"32\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                                       <a href=\"https://x.com/huddlemonkey\" target=\"_blank\" rel=\"noreferrer\">");
        html.AppendLine("                                                                                                                                                                                           <img class=\"mceSocialFollowImage\" width=\"32\" height=\"32\" alt=\"\" src=\"https://cdn-images.mailchimp.com/icons/social-block-v3/block-icons-v3/twitter-filled-color-40.png\"/>");
        html.AppendLine("                                                                                                                                                                                       </a>");
        html.AppendLine("                                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </td>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <td align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:inline;float:left\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                                   <td style=\"padding-top:3px;padding-bottom:3px;padding-left:12px;padding-right:12px\" class=\"mceSocialFollowIcon\" align=\"center\" width=\"32\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                                       <a href=\"https://www.youtube.com/@huddlemonkey\" target=\"_blank\" rel=\"noreferrer\">");
        html.AppendLine("                                                                                                                                                                                           <img class=\"mceSocialFollowImage\" width=\"32\" height=\"32\" alt=\"\" src=\"https://cdn-images.mailchimp.com/icons/social-block-v3/block-icons-v3/youtube-filled-color-40.png\"/>");
        html.AppendLine("                                                                                                                                                                                       </a>");
        html.AppendLine("                                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </td>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           <td align=\"center\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:inline;float:left\" role=\"presentation\">");
        html.AppendLine("                                                                                                                                                                           <tbody>");
        html.AppendLine("                                                                                                                                                                               <tr>");
        html.AppendLine("                                                                                                                                                                                   <td style=\"padding-top:3px;padding-bottom:3px;padding-left:12px;padding-right:12px\" class=\"mceSocialFollowIcon\" align=\"center\" width=\"32\" valign=\"top\">");
        html.AppendLine("                                                                                                                                                                                       <a href=\"https://huddlemonkey.com/newsletter\" target=\"_blank\" rel=\"noreferrer\">");
        html.AppendLine("                                                                                                                                                                                           <img class=\"mceSocialFollowImage\" width=\"32\" height=\"32\" alt=\"\" src=\"https://cdn-images.mailchimp.com/icons/social-block-v3/block-icons-v3/website-filled-color-40.png\"/>");
        html.AppendLine("                                                                                                                                                                                       </a>");
        html.AppendLine("                                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </td>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                       <!--[if mso]>");
        html.AppendLine("                                                                                                                                                                           </tr></table>");
        html.AppendLine("                                                                                                                                                                       <![endif]-->");
        html.AppendLine("                                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                                       </table>");
        html.AppendLine("                                                                                                                                   </td>");
        html.AppendLine("                                                                                                                               </tr>");
        html.AppendLine("                                                                                                                           </tbody>");
        html.AppendLine("                                                                                                                       </table>");
        html.AppendLine("                                                                                                                   </td>");
        html.AppendLine("                                                                                                               </tr>");
        html.AppendLine("                                                                                                           </tbody>");
        html.AppendLine("                                                                                                       </table>");
        html.AppendLine("                                                                                                   </td>");
        html.AppendLine("                                                                                               </tr>");
        html.AppendLine("                                                                                           </tbody>");
        html.AppendLine("                                                                                       </table>");
        html.AppendLine("                                                                                   </td>");
        html.AppendLine("                                                                               </tr>");
        html.AppendLine("                                                                           </tbody>");
        html.AppendLine("                                                                       </table>");
        html.AppendLine("                                                                   </td>");
        html.AppendLine("                                                               </tr>");
        html.AppendLine("                                                           </tbody>");
        html.AppendLine("                                                       </table>");
        html.AppendLine("                                                   </td>");
        html.AppendLine("                                               </tr>");
        html.AppendLine("                                           </tbody>");
        html.AppendLine("                                       </table>");
        html.AppendLine("                                       <!--[if (gte mso 9)|(IE)]>");
        html.AppendLine("                                           </td></tr></table>");
        html.AppendLine("                                       <![endif]-->");
        html.AppendLine("                                   </td>");
        html.AppendLine("                               </tr>");
        html.AppendLine("                           </tbody>");
        html.AppendLine("                       </table>");
        html.AppendLine("                   </td>");
        html.AppendLine("               </tr>");
        html.AppendLine("           </tbody>");
        html.AppendLine("       </table>");
        html.AppendLine("   </center>");
        html.AppendLine("</body>");

        html.AppendLine("</html>");

        return html.ToString();
    }
}
