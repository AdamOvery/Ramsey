

public interface INode
{
    int id { get; set; }
    List<INode> adjacentNodes { get; set; }

}

public static class INodes
{

    public static bool IsOrdered(this IEnumerable<INode> source)
    {
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext()) return true;

        int prevId = iterator.Current.id;
        while (iterator.MoveNext())
        {
            var currentId = iterator.Current.id;
            if (currentId <= prevId) return false;
            else prevId = currentId;
        }
        return true;
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static void ForEachIndexed<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in source)
        {
            action(item, index);
            index++;
        }
    }
}