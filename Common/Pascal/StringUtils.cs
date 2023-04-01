using System.Text;

namespace Pascal;

static class StringUtils
{
    public static string Join1(this IEnumerable<int>? arr, string separator = " ")
    {
        if (arr == null) return string.Empty;
        var result = new StringBuilder();
        var first = true;
        foreach (int i in arr)
        {
            if (first) first = false;
            else result.Append(separator);

            result.Append(i);
        }
        return result.ToString();
    }
}
