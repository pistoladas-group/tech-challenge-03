namespace TechNews.Common.Library.Extensions;

public static class IEnumerableExtensions
{
    public static T Random<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        var r = new Random();
        var list = enumerable as IList<T> ?? enumerable.ToList();

        if (list.Count == 0)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        return list[r.Next(0, list.Count)];
    }
}
