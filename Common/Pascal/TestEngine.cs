using System.Diagnostics;

namespace Pascal;
// 
public static class TestEngine
{
    [DebuggerHidden]
    public static void Assert(string testName, Func<bool> condition)
    {
        bool result;
        try
        {
            result = condition.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
        if (result) Console.WriteLine($"[Pass] {testName}");
        else
        {
            var error = $"{testName} (false)";
            Console.WriteLine($"[Failed] {error}.");
            throw new TestFailedException(error);
        }
    }

    [DebuggerHidden]
    public static void AssertEquals<T>(string testName, Func<T> test, T expected)
    {
        // if (testName.IndexOf("{}") >= 0) testName = testName.Replace("{}", ToString(expected));
        var actual = default(T);
        try
        {
            actual = test.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
        if ((expected == null && actual == null)
        || (expected != null && expected.Equals(actual))) Console.WriteLine($"[Pass] {testName}: {actual}");
        else
        {
            var error = $"{testName} expected {expected}\nactual {actual}";
            Console.WriteLine($"[Failed] {error}");
            throw new TestFailedException(error);
        }
    }

    [DebuggerHidden]
    public static void AssertEquals<T>(string testName, Func<T> test, string expected)
    {
        var actual = default(T);
        try
        {
            actual = test.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
        if ((expected == null && actual == null)
        || (expected != null && actual != null && actual.ToString() == expected)) Console.WriteLine($"[Pass] {testName}: {actual}");
        else
        {
            var error = $"{testName} expected {expected}\nactual {actual}";
            Console.WriteLine($"[Failed] {error}");
            throw new TestFailedException(error);
        }
    }

    [DebuggerHidden]
    private static string ToString(object? o)
    {
        if (o is string s) return $"\"{s}\"";
        else return o?.ToString() ?? "null";
    }

    [DebuggerHidden]
    public static void Test(string testName, Action action)
    {
        Console.WriteLine($"* {testName}");
        try
        {
            action();
            Console.WriteLine($"[Pass] {testName}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            if (!(e is TestFailedException)) Console.WriteLine(e.ToString());
            throw;
        }
    }

    public class TestFailedException : Exception
    {
        public TestFailedException(string message) : base(message) { }
    }
}