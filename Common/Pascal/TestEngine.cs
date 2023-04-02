namespace Pascal;
// 
public static class TestEngine
{
    public static void Test(string testName, Action action)
    {
        Console.WriteLine($"* Test: {testName}");
        try
        {
            action();
            Console.WriteLine($"[Pass] Test: {testName}");
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"[Error] Failed Test: {testName}");
            Console.WriteLine(e.ToString());
        }

    }
}