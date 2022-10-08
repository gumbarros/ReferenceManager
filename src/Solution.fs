module ReferenceManager.Solution

open System.Collections.Generic
open ReferenceManager.Extensions
open ReferenceManager.Utils
open System.IO
open System.Linq
open Spectre.Console
open net.r_eg.MvsSln
open net.r_eg.MvsSln.Projects

type SolutionProject =
    { Name: string
      FullPath: string
      PackageReferences: IEnumerable<Item> }

type SolutionData =
    { Projects: IEnumerable<SolutionProject>
      Name: string
      Path: string }

let getSolutionProjectNames (solution: SolutionData) =
    solution.Projects.Select(fun p -> p.Name)
    
    
let writeSolutionProjects (solution: SolutionData) =
    let solutionTree =
        Tree($"[bold]{solution.Name}[/]")

    solutionTree.Style <- Style(Color.Green)
    solutionTree.AddNodes(getSolutionProjectNames solution)

    AnsiConsole.EmptyLine
    AnsiConsole.Write solutionTree
    AnsiConsole.EmptyLine

let promptSolution () : SolutionData =
    let path = promptPath ".sln"

    use sln =
        new Sln(path, SlnItems.All)

    let name = Path.GetFileNameWithoutExtension(path)

    let projects: IEnumerable<SolutionProject> =
        query {
            for project in sln.Result.Env.UniqueByGuidProjects do
                select
                    { Name = project.ProjectName
                      FullPath = project.ProjectFullPath
                      PackageReferences = project.GetPackageReferences().ToList() }
        }

    { Name = name
      Projects = projects.ToList()
      Path = path }