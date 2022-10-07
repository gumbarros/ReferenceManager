module ReferenceManager.ReplaceReference

open System
open System.Collections.Generic
open System.IO
open Spectre.Console
open net.r_eg.MvsSln
open ReferenceManager.Utils
open ReferenceManager.Solution
open System.Linq
open net.r_eg.MvsSln.Core
open net.r_eg.MvsSln.Core.ObjHandlers
open net.r_eg.MvsSln.Core.SlnHandlers
open ReferenceManager.Extensions
open net.r_eg.MvsSln.Extensions

let validateIfReferenceExists (solution: SolutionData, reference: string) =

    let projects = solution.Projects

    if projects
        .SelectMany(fun p -> p.PackageReferences)
           .Any(fun r -> r.evaluatedInclude = reference) then
        ValidationResult.Success()
    else
        ValidationResult.Error()

let promptReference (solution: SolutionData) =

    let prompt =
        TextPrompt<string>(
            "Enter the [Green]PackageReference[/] name:",
            ValidationErrorMessage = "PackageReference does not exist.",
            Validator = fun d -> validateIfReferenceExists (solution, d)
        )

    AnsiConsole.Prompt<string>(prompt)

let getValidProjects (projects: IEnumerable<IXProject>, packageReferenceName: string) =
    projects
        .Where(fun p ->
            p
                .GetPackageReferences()
                .Any(fun r -> r.evaluatedInclude = packageReferenceName))
        .DistinctBy(fun p -> p.ProjectName)

let addProjectReference(project:IXProject, path: string) =
    project.AddItem("ProjectReference",project.ProjectFullPath.MakeRelativePath path)

let setReferenceAtProjectFiles
    (
        projects: IEnumerable<IXProject>,
        packageReferenceName: string,
        projectReferencePath: string
    ) =

    let validProjects =
        getValidProjects (projects, packageReferenceName)

    for project in validProjects do
        let isRemoved =
            project.RemovePackageReference packageReferenceName

        if isRemoved then
            AnsiConsole.MarkupLine "[Red]PackageReference removed.[/]"

        let isAdded = addProjectReference(project, projectReferencePath)

        if isAdded then
            AnsiConsole.MarkupLine "[Green]ProjectReference added.[/]"
        
        project.Save()
        
let fixSolutionFile(path: string) =
    let mutable text = File.ReadAllText(path)
    let text = text.ReplaceWholeWord("EndProjec", "EndProject")
    File.WriteAllText(path, text)

let saveProjectAtSolution (sln: Sln, solutionPath: string, projectName: string, projectPath: string) =
    let projectItem =
        ProjectItem(sln.Result.ProjectConfigs.First().PGuid, projectName, ProjectType.CsSdk, projectPath)

    let projects =
        sln.Result.ProjectItems.Append(projectItem)

    let handlers =
        Dictionary<Type, HandlerValue>()

    handlers.Add(typedefof<LProject>, HandlerValue(WProject(projects, sln.Result.ProjectDependencies)))
    use writer = new SlnWriter(solutionPath, handlers)

    writer.Write(sln.Result.Map)
    
    writer.Dispose()
    sln.Dispose()
    
    //Workaround MvsSln issue.
    fixSolutionFile(solutionPath)
    
    AnsiConsole.MarkupLine $"[Green]Added {projectName} to the solution.[/]"
    AnsiConsole.EmptyLine

let replacePackageByProjectReference (solution: SolutionData) =
    let packageReferenceName: string =
        promptReference solution

    let projectReferencePath =
        promptPath "ProjectReference"
    
    AnsiConsole.EmptyLine
    
    use sln = new Sln(solution.Path, SlnItems.All)

    let projects =
        sln.Result.Env.LoadMinimalProjects()

    setReferenceAtProjectFiles (projects, packageReferenceName, projectReferencePath)

    saveProjectAtSolution (sln, solution.Path, packageReferenceName, projectReferencePath)