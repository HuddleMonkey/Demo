using Demo.Shared.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Dto.Results;

/// <summary>
/// Factory for creating a success/failed result
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <returns>Result with default data</returns>
    public static Result<T> Success<T>() => new(true, default, "");

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <param name="message">Message to return to the user</param>
    /// <returns>Result with default data and message</returns>
    public static Result<T> Success<T>(string message) => new(true, default, message);

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <param name="data">Data to return as the result</param>
    /// <returns>Result with data</returns>
    public static Result<T> Success<T>(T data) => new(true, data, "");

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <param name="data">Data to return as the result</param>
    /// <param name="message">Message to return to the user</param>
    /// <returns>Result with data</returns>
    public static Result<T> Success<T>(T data, string message) => new(true, data, message);

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <returns>Result</returns>
    public static Result<T> Failed<T>() => new(false, default, "");

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <param name="message">Message on why the result failed</param>
    /// <returns>Result with message</returns>
    public static Result<T> Failed<T>(string message) => new(false, default, message);

    /// <summary>
    /// Creates a result from JSON
    /// </summary>
    /// <typeparam name="T">Type of data to return as the result</typeparam>
    /// <param name="json">JSON to convert</param>
    /// <returns>Result with message</returns>
    public static Result<T> FromJson<T>(string? json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            return JsonSerializer.Deserialize<Result<T>>(json, Utils.JsonSerializerOptions) ?? Failed<T>();
        }

        return Failed<T>();
    }
}

/// <summary>
/// Result of an action
/// </summary>
/// <typeparam name="T">Type of data that is returned in the result</typeparam>
/// <param name="succeeded">Whether or not the action succeeded</param>
/// <param name="data">Result data</param>
/// <param name="message">Message</param>
public class Result<T>(bool succeeded, T? data, string message)
{
    /// <summary>
    /// Whether or not the action succeeded
    /// </summary>
    public bool Succeeded { get; set; } = succeeded;

    /// <summary>
    /// Whether or not the action failed
    /// </summary>
    [JsonIgnore]
    public bool Failed => !Succeeded;

    /// <summary>
    /// Result data
    /// </summary>
    public T? Data { get; set; } = data;

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = message;
}
