namespace Demo.Shared.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Adds a range of keys with a particular value to the dictionary. Keys are only added if they do not already exist in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    /// <param name="dictionary">Dictionary to add the values to</param>
    /// <param name="keys">List of keys to add</param>
    /// <param name="value">Value to add</param>
    public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, List<TKey> keys, TValue value) where TKey : notnull
    {
        foreach (var key in keys)
        {
            if (dictionary.ContainsKey(key)) continue;
            dictionary.Add(key, value);
        }
    }
}
