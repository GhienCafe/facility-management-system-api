using AppCore.Data;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using AppCore.Models;

namespace AppCore.Extensions
{
    public interface ISendNotification
    {
        Task SendFirebaseMessage(Notification noti, Registration registrationToken);
        Task SendFirebaseMulticastMessage(Request request);
    }
    public class SendNotification : ISendNotification
    {
        public async Task SendFirebaseMessage(Notification noti, Registration registrationToken)
        {
            await InitializeFirebase();

            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { "score", "850" },
                    { "time", "1:00" },
                },
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = noti.Title,
                    Body = noti.Body,
                },
                Token = registrationToken.Token,
                Webpush = new WebpushConfig
                {
                    Notification = new WebpushNotification
                    {
                        Title = noti.Title,
                        Body = noti.Body,
                    },
                },
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);
        }

        public async Task SendFirebaseMulticastMessage(Request request)
        {
            await InitializeFirebase();

            if (request.ListToken != null)
            {
                var message = new MulticastMessage()
                {
                    Tokens = request.ListToken.Tokens,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = request.Notification?.Title,
                        Body = request.Notification?.Body,
                    },
                    Data = new Dictionary<string, string>()
                    {
                        { "content_type", "notification" },
                        { "value", "2" }
                    },
                    Webpush = new WebpushConfig
                    {
                        Notification = new WebpushNotification
                        {
                            Title = request.Notification?.Title,
                            Body = request.Notification?.Body,
                        },
                    },
                };

                BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(false);
            }
        }
        
        public static string GetHtmlContent(Notification noti)
        {
            string title = noti.Title ?? "Thông báo";
            string body = noti.Body ?? "Nội dung thông báo";

            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background-color: #ffffff;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            border-radius: 5px;
                        }}
                        .header {{
                            text-align: center;
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            border-radius: 5px 5px 0 0;
                        }}
                        .title {{
                            font-size: 24px;
                            font-weight: bold;
                            margin-bottom: 20px;
                        }}
                        .content {{
                            font-size: 16px;
                            line-height: 1.5;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>{title}</h2>
                        </div>
                        <div class='title'>
                            <h3>{title}</h3>
                        </div>
                        <div class='content'>
                            <p>{body}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return htmlContent;
        }

        private static Task InitializeFirebase()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                string projectId = EnvironmentExtension.GetProjectIdFirebase();
                string privateKeyId = EnvironmentExtension.GetPrivateKeyIdFirebase();
                string privateKey = EnvironmentExtension.GetPrivateKeyFirebase();
                string clientEmail = EnvironmentExtension.GetClientEmailFireBase();

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson($@"
        {{
            ""type"": ""service_account"",
            ""project_id"": ""{projectId}"",
            ""private_key_id"": ""{privateKeyId}"",
            ""private_key"": ""{privateKey}"",
            ""client_email"": ""{clientEmail}""
        }}")
                });
            }

            return Task.CompletedTask;
        }
    }
    public class Notification
    {
        public string? Title { get; set; }= null!;
        public string? Body { get; set; }= null!;
    }

    public class Registration
    {
        public string? Token { get; set; }= null!;
    }

    public class PriorityDto
    {
        public bool Priority { get; set; }
    }

    public class ListToken
    {
        public List<string>? Tokens { get; set; } = null!;
    }

    public class Request
    {
        public ListToken? ListToken { get; set; }= null!;
        public Notification? Notification { get; set; }= null!;
    }
}
