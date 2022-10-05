module ReferenceManager.ListDependencies


open System.Collections.Generic
open Microsoft.Build.Construction
open Spectre.Console
open System.Linq
open System
open ReferenceManager.SolutionManager

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

    let dependencies =
        project.Items.Where (fun c ->
            c.ElementName.Equals("PackageReference")
            || c.ElementName.Equals("ProjectReference"))

    dependencies

let writeDependencies (dependencies: IEnumerable<ProjectItemElement>) =
    for dependency in dependencies do             
        AnsiConsole.Markup $"[green]-[/] {dependency.Include}"
        
        let versionMetadata =
            dependency.Metadata.FirstOrDefault(fun m -> m.Name = "Version")
            
        match versionMetadata with
        | null -> AnsiConsole.MarkupLine String.Empty
        | _ -> AnsiConsole.MarkupLine $" {versionMetadata.Value}"
        
        AnsiConsole.WriteLine String.Empty

let listDependenciesFromProject (solution: SolutionFile) =
    let selectedProject =
        getProjectPath (solution)
        |> ProjectRootElement.Open

    let dependencies =
        getDependencies selectedProject

    writeDependencies(dependencies)