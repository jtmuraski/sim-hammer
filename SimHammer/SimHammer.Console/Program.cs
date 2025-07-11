using SimHammer.Console;
using Spectre.Console;

// See https://aka.ms/new-console-template for more information
AnsiConsole.Write(
    new FigletText("Sim Hammer")
        .Centered()
        .Color(Color.Red));

AnsiConsole.MarkupLine("Welcome to [bold red]Suim Hammer[/]!");
AnsiConsole.MarkupLine("This is a combat simulation tool for tabletop wargaming. View the menun below and select your choice");

var menuChoice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("What would you like to do?")
        .PageSize(10)
        .AddChoices(new[] { "Begin Simulation", "Exit" }));

switch(menuChoice)
{
    case("Begin Simulation"):
        ConductSimulation.BeginSimulation();
        break;
}
Console.ReadLine();
