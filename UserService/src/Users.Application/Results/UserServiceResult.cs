using Users.Application.Commons;

namespace Users.Application.Results;

public class UserServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public List<string>? Errors { get; init; }
    public ServiceErrorCode ErrorCode { get; init; }

    private UserServiceResult(bool isSuccess, T? value = default, List<string>? errors = null,
        ServiceErrorCode errorCode = ServiceErrorCode.None)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
        ErrorCode = errorCode;
    }

    public static UserServiceResult<T?> Success(T value) => new(true, value);

    public static UserServiceResult<T?> Failure(List<string> errors, ServiceErrorCode errorCode)
        => new(false, default, errors, errorCode);
}

public class UserServiceResult
{
    public bool IsSuccess { get; init; }
    public List<string>? Errors { get; init; }
    public ServiceErrorCode ErrorCode { get; init; }

    private UserServiceResult(bool isSuccess, List<string>? errors = null,
        ServiceErrorCode errorCode = ServiceErrorCode.None)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        ErrorCode = errorCode;
    }

    public static UserServiceResult Success() => new(true);

    public static UserServiceResult Failure(List<string> errors, ServiceErrorCode errorCode)
        => new(false, errors, errorCode);
}