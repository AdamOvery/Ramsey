// See https://aka.ms/new-console-template for more information
using Pascal;

internal class Program
{
    private static void Main(string[] args)
    {
        var vars = Environment.GetEnvironmentVariables();

        if (Environment.GetEnvironmentVariable("USER") == "pascal")
        {
            PascalProgram.PascalMain();
        }
        else
        {
            AdamProgram.AdamMain();
        }

        Menu();
    }

    private static void Menu()
    {
        while (true)
        {
            Console.WriteLine(@"______                               ");
            Console.WriteLine(@"| ___ \                              ");
            Console.WriteLine(@"| |_/ /__ _ _ __ ___  ___  ___ _   _ ");
            Console.WriteLine(@"|    // _` | '_ ` _ \/ __|/ _ \ | | |");
            Console.WriteLine(@"| |\ \ (_| | | | | | \__ \  __/ |_| |");
            Console.WriteLine(@"\_| \_\__,_|_| |_| |_|___/\___|\__, |");
            Console.WriteLine(@"                                __/ |");
            Console.WriteLine(@"                               |___/ ");
            Console.WriteLine(@"[A]   Adam");
            Console.WriteLine(@"[P]   Pascal");
            Console.WriteLine(@"[Esc] Exit");
            Console.Write("> ");
            var input = Console.ReadKey().KeyChar.ToString().ToUpper();
            Console.WriteLine();

            switch (input)
            {
                case "A":
                    AdamProgram.AdamMain();
                    break;
                case "P":
                    PascalProgram.PascalMain();
                    break;
                case "\x1B":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }
}