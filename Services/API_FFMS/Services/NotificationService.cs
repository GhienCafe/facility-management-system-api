using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Repositories;
using AppCore.Extensions;
namespace API_FFMS.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

public interface INotificationService : IBaseService
{
    Task SendSingleMessage(NotificationDto noti, RegistrationDto registrationToken);
    Task SendMultipleMessages(RequestDto request);
}
public class NotificationService : BaseService, INotificationService
{
    
    public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    // NotificationService.cs
    public async Task SendSingleMessage(NotificationDto noti, RegistrationDto registrationToken)
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
            ""private_key_id"": ""{privateKeyId}"", // Sử dụng giá trị từ biến môi trường
            ""private_key"": ""{privateKey}"",
            ""client_email"": ""{clientEmail}""
        }}")
            });

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
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Error sending message: " + response);
                throw new Exception("Server error for not valid sent message");
            }

            Console.WriteLine("Successfully sent message: " + response);
        }
        else
        {
            // FirebaseApp.DefaultInstance đã được khởi tạo, bạn có thể sử dụng nó để gửi thông báo.
            FirebaseMessaging messaging = FirebaseMessaging.DefaultInstance;

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
            };

            string response = await messaging.SendAsync(message).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Error sending message: " + response);
                throw new Exception("Server error for not valid sent message");
            }

            Console.WriteLine("Successfully sent message: " + response);
        }
    }
    
    public async Task SendMultipleMessages(RequestDto request)
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
            ""private_key_id"": ""{privateKeyId}"", // Sử dụng giá trị từ biến môi trường
            ""private_key"": ""{privateKey}"",
            ""client_email"": ""{clientEmail}""
        }}")
            });
            var message = new MulticastMessage()
            {
                Tokens = request.ListToken.Tokens, // Sử dụng danh sách tokens từ ListToken
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
            };

            BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(false);

            if (response.FailureCount > 0)
            {
                Console.WriteLine($"Failed to send {response.FailureCount} messages");

                // Xử lý danh sách các registration tokens không thành công
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        string errorToken = request.ListToken.Tokens[i];
                        Console.WriteLine($"Failed to send message to token: {errorToken}");
                    }
                }

                throw new Exception("Server error for not valid sent message");
            }
        }
        else
        {
            var message = new MulticastMessage()
            {
                Tokens = request.ListToken.Tokens, // Sử dụng danh sách tokens từ ListToken
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
            };

            BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(false);

            if (response.FailureCount > 0)
            {
                Console.WriteLine($"Failed to send {response.FailureCount} messages");

                // Xử lý danh sách các registration tokens không thành công
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        string errorToken = request.ListToken.Tokens[i];
                        Console.WriteLine($"Failed to send message to token: {errorToken}");
                    }
                }

                throw new Exception("Server error for not valid sent message");
            }
        }
    }
}