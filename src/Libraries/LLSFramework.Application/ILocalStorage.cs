namespace LLSFramework.Application;

/// <summary>
/// Defines an abstraction for asynchronous local storage operations.
/// </summary>
public interface ILocalStorage
{
    /// <summary>
    /// Asynchronously retrieves a value of type <typeparamref name="T"/> from local storage by key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The unique key identifying the stored value.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the value if found; otherwise, <c>null</c>.
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Asynchronously stores a value of type <typeparamref name="T"/> in local storage under the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The unique key under which to store the value.</param>
    /// <param name="value">The value to store.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Asynchronously deletes the value associated with the specified key from local storage.
    /// </summary>
    /// <param name="key">The unique key identifying the value to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string key);
}