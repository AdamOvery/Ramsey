namespace Pascal;
// 
public static class TestEngine
{
    public static void AssertTrue(string testName, Func<bool> condition)
    {
        bool result;
        try
        {
            result = condition.Invoke();
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
        if (result) Console.WriteLine($"[Pass] {testName}");
        else Console.WriteLine($"[Error] Failed {testName} returned false.");
    }

    public static void AssertEquals<T>(string testName, Func<T> test, T expected)
    {
        var actual = default(T);
        try
        {
            actual = test.Invoke();
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
        if ((expected == null && actual == null)
        || (expected != null && expected.Equals(actual))) Console.WriteLine($"[Pass] {testName}: {actual}");
        else Console.WriteLine($"[Error] Failed {testName}: expected {expected}, actual {actual}");
    }



    public static void Test(string testName, Action action)
    {
        Console.WriteLine($"* {testName}");
        try
        {
            action();
            Console.WriteLine($"[Pass] {testName}");
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"[Error] Failed {testName}");
            Console.WriteLine(e.ToString());
            throw;
        }
    }
}