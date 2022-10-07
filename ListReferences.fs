module ReferenceManager.ListReferences

open System.Collections.Generic
open Spectre.Console
open net.r_eg.MvsSln
open System.Linq
open ReferenceManager.Solution
open ReferenceManager.Extensions
open System

let promptProject (solution: SolutionData) =
    let selectionPrompt =
        SelectionPrompt<string>()

    selectionPrompt.Title <- "Select a project to list the references."

    AnsiConsole.Prompt<string>
    <| selectionPrompt.AddChoices(getSolutionProjectNames solution)

let writeReferences (references: IEnumerable<Projects.Item>) =
    for reference in references do
        AnsiConsole.Markup $"[green]-[/] {reference.evaluatedInclude}"

        let versionMetadata =
            reference.meta["Version"].evaluated

        match versionMetadata with
        | null -> AnsiConsole.MarkupLine String.Empty
        | _ -> AnsiConsole.MarkupLine $" {versionMetadata}"

        AnsiConsole.EmptyLine

let listReferencesFromProject (solution: SolutionData) =
    let projectName = promptProject solution

    let selectedProject =
        solution.Projects.First(fun p -> p.Name = projectName)

    let references =
        selectedProject.PackageReferences

    writeReferences references