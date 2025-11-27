using Products.Application.Commons;
using Products.Application.Dto_s.Products;

namespace Products.Application.Results;

public class ProductServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public List<string>? Errors { get; init; }
    public ServiceErrorCode ErrorCode { get; init; }

    private ProductServiceResult(bool isSuccess, T? value = default, List<string>? errors = null,
        ServiceErrorCode errorCode = ServiceErrorCode.None)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
        ErrorCode = errorCode;
    }

    public static ProductServiceResult<T?> Success(T value) => new(true, value);

    public static ProductServiceResult<T?> Failure(List<string> errors, ServiceErrorCode errorCode)
        => new(false, default, errors, errorCode);
}

public class ProductServiceResult
{
    public bool IsSuccess { get; init; }
    public List<string>? Errors { get; init; }
    public ServiceErrorCode ErrorCode { get; init; }

    private ProductServiceResult(bool isSuccess, List<string>? errors = null,
        ServiceErrorCode errorCode = ServiceErrorCode.None)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        ErrorCode = errorCode;
    }

    public static ProductServiceResult Success() => new(true);

    public static ProductServiceResult Failure(List<string> errors, ServiceErrorCode errorCode)
        => new(false, errors, errorCode);
}