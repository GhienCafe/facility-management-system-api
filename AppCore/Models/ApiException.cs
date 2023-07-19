using System.Runtime.Serialization;

namespace AppCore.Models;

[Serializable]
public class ApiException : Exception
{
    protected ApiException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
        serializationInfo, streamingContext)
    {
    }

    public ApiException(string message, StatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
        Message = string.IsNullOrEmpty(message) ? StatusCode.ToString() : message;
    }

    public ApiException(string message, StatusCode statusCode, object result) : base(message)
    {
        StatusCode = statusCode;
        Message = string.IsNullOrEmpty(message) ? StatusCode.ToString() : message;
        Data = result;
    }

    public ApiException(StatusCode statusCode) : base(statusCode.ToString())
    {
        StatusCode = statusCode;
        Message = MessageKey.ServerError;
    }

    public ApiException(string message) : base(message)
    {
        StatusCode = StatusCode.SERVER_ERROR;
        Message = string.IsNullOrEmpty(message) ? StatusCode.ToString() : message;
    }

    public ApiException() : base(StatusCode.SERVER_ERROR.ToString())
    {
    }

    public StatusCode StatusCode { get; set; } = StatusCode.SERVER_ERROR;
    public override string Message { get; } = MessageKey.ServerError;
    public new object Data { get; }
}

public enum StatusCode
{
    SUCCESS = 200,
    CREATED = 201,
    BAD_REQUEST = 400,
    UNAUTHORIZED = 401,
    FORBIDDEN = 403,
    NOT_FOUND = 404,
    NOT_ACTIVE = 405,
    CHANGE_PASSWORD = 406,
    NOT_VERIFY = 407,
    TIME_OUT = 408,
    ALREADY_EXISTS = 409,
    UNPROCESSABLE_ENTITY = 422,
    SERVER_ERROR = 500,
}

public class MessageKey
{
    public const string InvalidUsernameOrPassword = "system_message_invalid_username_or_password";
    public const string AccountNotActivated = "system_message_account_not_activated";
    public const string AccountIsLoggedInOnAnotherDevice = "system_message_account_is_logged_in_on_another_device";
    public const string Unauthorized = "system_message_unauthorized";
    public const string Forbidden = "system_message_forbidden";
    public const string NotActive = "system_message_not_active";


    public const string ChooseFile = "system_message_choose_file";
    public const string NotFound = "system_message_not_found";
    
    public const string ServerError = "system_message_server_error";
    public const string RefreshTokenNotFound = "system_message_refresh_token_not_found";
}