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
    MULTI_STATUS = 207,
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
    public const string InvalidUsernameOrPassword = "Sai tên người dùng hoặc mật khẩu";
    public const string AccountNotActivated = "Tài khoản chưa được kích hoạt";
    public const string AccountIsLoggedInOnAnotherDevice = "Tài khoản đang đăng nhập trên thiết bị khác";
    public const string Unauthorized = "Không xác định được người dùng";
    public const string Forbidden = "Không có quyền truy cập";
    public const string NotActive = "Tài khoản chưa được kích hoạt";

    public const string ChooseFile = "Lỗi file";
    public const string NotFound = "Không tìm thấy nội dung";

    public const string ServerError = "Có lỗi xảy ra";
    public const string RefreshTokenNotFound = "Không tìm thấy Refresh Token";
}
