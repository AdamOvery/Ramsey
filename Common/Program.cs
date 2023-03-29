// See https://aka.ms/new-console-template for more information
using Pascal;
using RamseyLibrary;

internal class Program
{
    private static void Main(string[] args)
    {
        // HOSTNAME=
        var vars = Environment.GetEnvironmentVariables();

        if (Environment.GetEnvironmentVariable("USER") == "pascal")
        {
            PascalProgram.PascalMain();
        }
        else
        {
            AdamProgram.AdamMain();
        }
    }
}