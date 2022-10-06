module ReferenceManager.Utils

open System
open System.IO
open Spectre.Console

let writeAppTitle () =
    AnsiConsole.Write(Rule())
    AnsiConsole.MarkupLine("[bold]Welcome to Reference Manager 🛠 !️[/]")
    AnsiConsole.Write(Rule())
    AnsiConsole.WriteLine String.Empty

let pathExists (path: string) =
    if File.Exists(path) then
        ValidationResult.Success()
    else
        ValidationResult.Error("[red]Invalid path![/]")

let promptPath (subject: string) =
    let prompt =
        TextPrompt($"What's your [green]{subject}[/] file path?", PromptStyle = "green", Validator = pathExists)

    AnsiConsole.Prompt(prompt)

type Choice = { Id: int; Description: string }

let promptChoice () =
    AnsiConsole
        .Prompt(
            SelectionPrompt<Choice>()
                .AddChoices(
                    { Id = 0
                      Description = "List dependencies from a project" },
                    { Id = 1
                      Description = "Replace a PackageReference with ProjectReference" },
                    { Id = 2
                      Description = "Load another [Green].sln[/]" },
                    { Id = -1; Description = "Exit" }
                )
                .UseConverter(fun c -> c.Description)
        )
        .Id
