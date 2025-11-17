namespace Toss.Application.Common.Models;

/// <summary>
/// Represents the result of an operation, indicating success or failure with error messages.
/// </summary>
/// <remarks>
/// This is a common pattern for operations that can fail without throwing exceptions,
/// particularly useful for:
/// <list type="bullet">
/// <item><description>User management operations (create, delete, update)</description></item>
/// <item><description>Validation failures that should be communicated to the user</description></item>
/// <item><description>Business rule violations that are expected scenarios</description></item>
/// </list>
/// Use <see cref="Success"/> for successful operations and <see cref="Failure"/> for failed ones.
/// </remarks>
public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Gets an array of error messages if the operation failed.
    /// Empty array if <see cref="Succeeded"/> is true.
    /// </summary>
    public string[] Errors { get; init; }

    /// <summary>
    /// Creates a successful result with no errors.
    /// </summary>
    /// <returns>A result indicating success.</returns>
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed result with one or more error messages.
    /// </summary>
    /// <param name="errors">The error messages describing what went wrong.</param>
    /// <returns>A result indicating failure with error details.</returns>
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}
