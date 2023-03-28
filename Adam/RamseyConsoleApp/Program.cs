// See https://aka.ms/new-console-template for more information
using RamseyLibrary;

Console.WriteLine("Enter Max 'On' clique");
var maxCliqueOnString = Console.ReadLine();

if (!int.TryParse(maxCliqueOnString, out var maxCliqueOn))
{
    Console.WriteLine("Invalid Response");
    return;
}

Console.WriteLine("Enter Max 'Off' clique");
var maxCliqueOffString = Console.ReadLine();
if (!int.TryParse(maxCliqueOffString, out var maxCliqueOff))
{
    Console.WriteLine("Invalid Response");
    return;
}

Console.WriteLine("Enter vertex count");
var vertexCountString = Console.ReadLine();
if (!int.TryParse(vertexCountString, out var vertexCount))
{
    Console.WriteLine("Invalid Response");
    return;
}

Console.WriteLine("Find all solutions? (Y/N)");

var findAllSolutionsString = Console.ReadLine()?.ToUpperInvariant();

bool? findAllSolutions = (findAllSolutionsString == "N") ? false : (findAllSolutionsString == "Y") ? true : null;
if (findAllSolutions is null)
{
    Console.WriteLine("Invalid Response");
    return;
}

var ramseyConfig = new RamseyConfig(vertexCount, maxCliqueOn, maxCliqueOff, findAllSolutions ?? false);
var ramsey = new Ramsey(ramseyConfig);

var description = $"R({ramseyConfig.MaxCliqueOn},{ramseyConfig.MaxCliqueOff}). Vertex Count: {ramseyConfig.VertexCount}";
var timeTaken = string.Format("{0:0.00}s", ramsey.TimeTaken.TotalMilliseconds / 1000);

if (ramsey.IsSuccess)
{
    foreach (var solution in ramsey.Solutions)
    {
        Console.WriteLine($"{description}. Solution = {solution}. Found in {timeTaken}");
    }
}
else
{
    Console.WriteLine($"{description}. No solution Found in {timeTaken}");
}

Console.WriteLine("Press 'return' to end");
Console.ReadLine();
