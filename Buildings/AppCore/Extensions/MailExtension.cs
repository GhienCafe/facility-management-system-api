using MailKit.Security;
using MimeKit;
namespace AppCore.Extensions;

public class MailExtension
{
    private readonly string _accountEmailSystem = EnvironmentExtension.GetAccountEmailSystem();
    private readonly string  _passwordEmailPrivate = EnvironmentExtension.GetPasswordEmailPrivate();

    public void SendMailCommon(string subject, string recipientName, string emailTo, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_accountEmailSystem));
        email.To.Add(MailboxAddress.Parse(emailTo));
        email.Date = DateTime.Now;
        email.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                }}
                .container {{
                    max-width: 500px;
                    margin: 0 auto;
                    padding: 20px;
                    border: 1px solid #ccc;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                h3 {{
                    color: #333;
                }}
                p {{
                    color: #555;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h3>Thân gửi {recipientName},</h3>
                <p>{body}</p>
            </div>
        </body>
        </html>
    ";

        email.Body = builder.ToMessageBody();
        //
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_accountEmailSystem, _passwordEmailPrivate);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}