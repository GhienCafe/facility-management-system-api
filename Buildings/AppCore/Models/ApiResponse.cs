namespace AppCore.Models;

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; }

    public static ApiResponse<T> Success(T result)
    {
        return Create(result, StatusCode.SUCCESS, StatusCode.SUCCESS.ToString());
    }

    private static ApiResponse<T> Create(T data, StatusCode statusCode, string message)
    {
        return new ApiResponse<T>
        {
            Data = data,
            StatusCode = statusCode,
            Message = message,
        };
    }

    public static ApiResponse<T> Failed(string errorMessage, StatusCode statusCode = StatusCode.BAD_REQUEST)
    {
        return new ApiResponse<T>
        {
            Message = errorMessage,
            StatusCode = statusCode
        };
    }
}

public class ApiResponses<T> : ApiResponse
{
    public int? TotalCount { get; set; } = 0;
    public int? PageSize { get; set; } = 0;
    public int? Offset { get; set; } = 0;
    public int? TotalPages { get; set; } = 0;
    public IEnumerable<T> Data { get; set; }

    public static ApiResponses<T> Success(IEnumerable<T> data, int? totalCount = null, int? pageSize = null,
        int? offset = null,
        int? totalPages = null)
    {
        return Create(
            data,
            StatusCode.SUCCESS,
            StatusCode.SUCCESS.ToString(),
            totalCount,
            pageSize,
            offset,
            totalPages
        );
    }

    private static ApiResponses<T> Create(IEnumerable<T> data, StatusCode statusCode, string message,
        int? totalCount,
        int? pageSize,
        int? offset,
        int? totalPages)
    {
        return new ApiResponses<T>
        {
            Data = data,
            StatusCode = statusCode,
            Message = message,
            TotalCount = totalCount,
            PageSize = pageSize,
            Offset = offset,
            TotalPages = totalPages
        };
    }

    public static ApiResponses<T> Fail(IEnumerable<T> data, StatusCode statusCode, string message)
    {
        return new ApiResponses<T>
        {
            Data = data,
            StatusCode = statusCode,
            Message = message
        };
    }
}

public class ApiResponse
{
    public StatusCode StatusCode { get; set; }

    public string Message { get; set; }

    public static ApiResponse Success()
    {
        return Created(StatusCode.SUCCESS, "Success");
    }

    public static ApiResponse Success(string message)
    {
        return Created(StatusCode.SUCCESS, message);
    }

    public static ApiResponse Failed()
    {
        return Created(StatusCode.BAD_REQUEST, "Failed");
    }
    public static ApiResponse Failed(string errorMessage, StatusCode statusCode = StatusCode.BAD_REQUEST)
    {
        return new ApiResponse
        {
            Message = errorMessage,
            StatusCode = statusCode
        };
    }
    
    public static ApiResponse Created(StatusCode statusCode, string message)
    {
        return new ApiResponse
        {
            StatusCode = statusCode,
            Message = message,
        };
    }

    public static ApiResponse Created(string message)
    {
        return new ApiResponse
        {
            StatusCode = StatusCode.CREATED,
            Message = message,
        };
    }
}