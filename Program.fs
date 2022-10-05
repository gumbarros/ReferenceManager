module ReferenceManager.Program

open System.IO
open Microsoft.Build.Construction
open Spectre.Console
open System
open ReferenceManager.ListDependencies
open SolutionUtils

let writeTitle() =
    AnsiConsole.Write(Rule())
    AnsiConsole.MarkupLine("[bold]Welcome to Reference Manager 🛠 !️[/]")
    AnsiConsole.Write(Rule())
    AnsiConsole.WriteLine String.Empty

let promptPath() =
    let pathPrompt = TextPrompt("What's your [green].sln[/] file path?")

    pathPrompt.PromptStyle <- "green"

    pathPrompt.Validator <-
        fun (path) ->
            if File.Exists(path) then
                ValidationResult.Success()
            else
                ValidationResult.Error("[red]Invalid path![/]")

    AnsiConsole.Prompt(pathPrompt)

let writeSolutionProjects(solutionName, solutionFile : SolutionFile) =
    let solutionTree =
        Tree($"[bold]{solutionName}[/]")

    solutionTree.Style <- Style(Color.Green)
    solutionTree.AddNodes(getSolutionProjectNames(solutionFile))

    AnsiConsole.WriteLine String.Empty
    AnsiConsole.Write solutionTree
    AnsiConsole.WriteLine String.Empty

writeTitle()
let path = promptPath()
let solutionName = Path.GetFileName(path)
let solutionFile = SolutionFile.Parse(path)

writeSolutionProjects(solutionName, solutionFile)

let action =
    AnsiConsole.Prompt(
        SelectionPrompt<string>()
            .AddChoices("List Dependencies from Project", "Replace Dependency")
    )

match action with
    | "List Dependencies from Project" -> listDependenciesFromProject solutionFile
    | _ -> printf "TODO"
    
let prompt = AnsiConsole.Prompt<string>(TextPrompt(String.Empty))