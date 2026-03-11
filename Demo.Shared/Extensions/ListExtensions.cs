namespace Demo.Shared.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Flattens a list of objects with children of the same type
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="source">List of parent objects</param>
    /// <param name="childPropertySelector">Child property selector</param>
    /// <returns>List of flattened objects</returns>
    public static List<T> Flatten<T>(this List<T> source, Func<T, List<T>> childPropertySelector)
    {
        var list = new List<T>();

        foreach (var item in source)
        {
            list.AddRange(item.Flatten(childPropertySelector));
        }

        return list;
    }

    /// <summary>
    /// Flatten's an object with children of the same type
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="source">Parent object</param>
    /// <param name="childPropertySelector">Child property selector</param>
    /// <returns>List of flattened objects</returns>
    public static IEnumerable<T> Flatten<T>(this T source, Func<T, IEnumerable<T>> childPropertySelector)
    {
        yield return source;

        foreach (var child in childPropertySelector(source))
        {
            foreach (var relative in Flatten(child, childPropertySelector))
            {
                yield return relative;
            }
        }
    }
}
