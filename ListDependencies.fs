module ReferenceManager.ListDependencies

open System.Collections.Generic
open SolutionUtils
open Microsoft.Build.Construction
open Spectre.Console
open System.Linq

let getProjectPath (solution: SolutionFile) =
    let selectionPrompt =
        SelectionPrompt<string>()

    selectionPrompt.Title <- "Select a project to list the dependencies."

    let projectName =
        AnsiConsole.Prompt<string>
        <| selectionPrompt.AddChoices(getSolutionProjectNames (solution))

    solution.ProjectsInOrder.First(fun p -> p.ProjectName = projectName)
        .AbsolutePath

let getDependencies (project: ProjectRootElement) =

    let dependencies = project.Items.Where(fun c -> c.ElementName.Equals("PackageReference"))

    dependencies

let writeDependencies (dependencies : IEnumerable<ProjectItemElement>) =
    for dependency in dependencies do
        let version = dependency.Metadata.First(fun m -> m.Name = "Version").Value
        AnsiConsole.MarkupLine $"[green]-[/] {dependency.Include} {version}"

let listDependenciesFromProject (solution: SolutionFile) =
    let selectedProject =
        getProjectPath (solution)
        |> ProjectRootElement.Open

    let dependencies =
        getDependencies selectedProject

    writeDependencies dependencies