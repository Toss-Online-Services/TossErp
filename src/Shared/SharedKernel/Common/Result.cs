namespace TossErp.SharedKernel.Common;

/// <summary>
/// Base result class for operations that can succeed or fail
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// Generic result class for operations that return a value
/// </summary>
public class Result<T> : Result
{
    protected internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }
}

/// <summary>
/// Extensions for working with results
/// </summary>
public static class ResultExtensions
{
    public static Result<TOutput> Map<TInput, TOutput>(this Result<TInput> result, Func<TInput, TOutput> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(mapper(result.Value!))
            : Result.Failure<TOutput>(result.Error!);
    }

    public static async Task<Result<TOutput>> MapAsync<TInput, TOutput>(this Result<TInput> result, Func<TInput, Task<TOutput>> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(await mapper(result.Value!))
            : Result.Failure<TOutput>(result.Error!);
    }
}
