using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Dapper;
using Npgsql;

namespace Postgres.Caching;

/// <summary>
/// Defines the contract for a generic caching service.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves and deserializes a cached value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the deserialized cached value or default(T) if not found or deserialization fails.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes and stores a value in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the object to store.</typeparam>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cached value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implements the generic ICacheService using PostgreSQL as the backing store.
/// </summary>
public class PostgresCacheService : ICacheService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly JsonSerializerOptions _serializerOptions; // Optional: For custom serialization settings

    public PostgresCacheService(NpgsqlDataSource dataSource, JsonSerializerOptions? serializerOptions = null)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions(); // Use default options if none provided
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var jsonString = await connection.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(
                "SELECT value::text FROM cache WHERE key = @Key",
                new { Key = key },
                cancellationToken: cancellationToken)
            );

        if (string.IsNullOrEmpty(jsonString))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonString, _serializerOptions);
        }
        catch (JsonException ex)
        {
            // Consider logging this error
            Console.WriteLine($"Error deserializing JSON for key {key} to type {typeof(T).Name}: {ex.Message}");
            return default; // Return default value if deserialization fails
        }
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        string jsonString;
        try
        {
            jsonString = JsonSerializer.Serialize(value, _serializerOptions);
        }
        catch (JsonException ex)
        {
            // Handle serialization error (e.g., log and throw or return)
            Console.WriteLine($"Error serializing value for key {key} of type {typeof(T).Name}: {ex.Message}");
            throw; // Re-throw or handle appropriately
        }

        await connection.ExecuteAsync(
             new CommandDefinition(
                 """
                 INSERT INTO cache(key, value)
                 VALUES (@Key, @Value::jsonb)
                 ON CONFLICT (key) DO UPDATE
                 SET value = excluded.value;
                 """,
                 new { Key = key, Value = jsonString },
                 cancellationToken: cancellationToken)
             );
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
             new CommandDefinition(
                 "DELETE FROM cache WHERE key = @Key",
                 new { Key = key },
                 cancellationToken: cancellationToken)
             );
    }
} 