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
    public int? Page { get; set; } = 0;
    public int? TotalPages { get; set; } = 0;
    public IEnumerable<T> Data { get; set; }

    public static ApiResponses<T> Success(IEnumerable<T> data, int? totalCount = null, int? pageSize = null,
        int? page = null,
        int? totalPages = null)
    {
        return Create(
            data,
            StatusCode.SUCCESS,
            StatusCode.SUCCESS.ToString(),
            totalCount,
            pageSize,
            page,
            totalPages
        );
    }

    private static ApiResponses<T> Create(IEnumerable<T> data, StatusCode statusCode, string message,
        int? totalCount,
        int? pageSize,
        int? page,
        int? totalPages)
    {
        return new ApiResponses<T>
        {
            Data = data,
            StatusCode = statusCode,
            Message = message,
            TotalCount = totalCount,
            PageSize = pageSize,
            Page = page,
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
    // public ApiResponse()
    // {
    //     StatusCode = StatusCode.SUCCESS;
    //     Message = "Success";
    // }
    //
    // //
    // public object Result { get; set; }
    // public ApiResponse(object result)
    // {
    //     StatusCode = StatusCode.SUCCESS;
    //     Message = "Success";
    //     Result = result;
    // }

}

public class ApiExportResponse
{
    public ApiExportResponse(string message, object result, int statusCode = 200)
    {
        StatusCode = statusCode;
        Message = message;
        Result = result;
    }

    public ApiExportResponse(string message, int statusCode)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public ApiExportResponse(object result)
    {
        Result = result;
    }

    public ApiExportResponse(string message)
    {
        Message = message;
    }

    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "Thành công";
    public object Result { get; set; }
}