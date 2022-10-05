module ReferenceManager.Utils

open System
open System.IO
open Spectre.Console

let writeAppTitle () =
    AnsiConsole.Write(Rule())
    AnsiConsole.MarkupLine("[bold]Welcome to Reference Manager 🛠 !️[/]")
    AnsiConsole.Write(Rule())
    AnsiConsole.WriteLine String.Empty

let promptPath (fileExtension : string) =
    let pathPrompt =
        TextPrompt($"What's your [green]{fileExtension}[/] file path?")

    pathPrompt.PromptStyle <- "green"

    pathPrompt.Validator <-
        fun (path) ->
            if File.Exists(path) then
                ValidationResult.Success()
            else
                ValidationResult.Error("[red]Invalid path![/]")

    AnsiConsole.Prompt(pathPrompt)

type Choice = { Id: int; Description: string }

let promptChoice () =
    AnsiConsole
        .Prompt(
            SelectionPrompt<Choice>()
                .AddChoices(
                    { Id = 0
                      Description = "List Dependencies from Project" },
                    { Id = 1
                      Description = "Replace PackageReference with ProjectReference" },
                    { Id = 2
                      Description = "Load another [Green].sln[/]" },
                    { Id = -1
                      Description = "Exit" }
                )
                .UseConverter(fun c -> c.Description)
        )
        .Id
