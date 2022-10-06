module ReferenceManager.References

open System.Collections.Generic
open Microsoft.Build.Construction
open Microsoft.Build.Evaluation
open Spectre.Console
open System.Linq
open System
open ReferenceManager.SolutionManager
open ReferenceManager.Utils

let promptProjectPath (solution: SolutionFile) =
    let selectionPrompt =
        SelectionPrompt<string>()

    selectionPrompt.Title <- "Select a project to list the references."

    let projectName =
        AnsiConsole.Prompt<string>
        <| selectionPrompt.AddChoices(getSolutionProjectNames (solution))

    solution.ProjectsInOrder.First(fun p -> p.ProjectName = projectName)
        .AbsolutePath

let getReferences (project: ProjectRootElement) =
    project.Items.Where (fun c ->
        c.ElementName.Equals("PackageReference")
        || c.ElementName.Equals("ProjectReference"))

let getReferenceByName (project: ProjectRootElement, referenceName: string) =
    getReferences(project).FirstOrDefault(fun d -> d.Include = referenceName)
    
let writeReferences (references: IEnumerable<ProjectItemElement>) =
    for reference in references do
        AnsiConsole.Markup $"[green]-[/] {reference.Include}"

        let versionMetadata =
            reference.Metadata.FirstOrDefault(fun m -> m.Name = "Version")

        match versionMetadata with
        | null -> AnsiConsole.MarkupLine String.Empty
        | _ -> AnsiConsole.MarkupLine $" {versionMetadata.Value}"

        AnsiConsole.WriteLine String.Empty

let listReferencesFromProject (solution: SolutionFile) =
    let selectedProject =
        promptProjectPath (solution)
        |> ProjectRootElement.Open

    let references =
        getReferences selectedProject

    writeReferences (references)

let containsReference (project: ProjectRootElement, reference: string) =
    let references = getReferences project

    references.Any(fun d -> d.Include = reference)

let validateIfReferenceExists (solution: SolutionFile, reference: string) =

    let projects = getSolutionProjects solution

    if projects.Any(fun p -> containsReference (p, reference)) then
        ValidationResult.Success()
    else
        ValidationResult.Error()

let promptReference (solution: SolutionFile) =
    let prompt =
        TextPrompt<string>(
            "Enter the [Green]PackageReference[/] name",
            ValidationErrorMessage = "PackageReferences does not exist.",
            Validator = fun d -> validateIfReferenceExists(solution,d)
        )

    AnsiConsole.Prompt<string>(prompt)

let replaceReference(solution:SolutionFile, referenceName: string, packageReferencePath: string) =
    for project in getSolutionProjects(solution).Where(fun p -> containsReference(p, referenceName)) do
        let reference = getReferenceByName(project,referenceName)
        project.RemoveChild(reference)
       


let replacePackageByProjectReference (solution: SolutionFile) =
    let referenceName : string = promptReference solution
    
    let path = promptPath "ProjectReference"
    
    replaceReference(solution,referenceName, path)