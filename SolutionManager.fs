module ReferenceManager.SolutionManager

open Microsoft.Build.Construction
open ReferenceManager.Utils
open System
open System.IO
open Spectre.Console

type SolutionData =
    { Name: string
      Path: string
      File: SolutionFile }

let getSolutionProjectNames (solution: SolutionFile) =
    query {
        for project in solution.ProjectsInOrder do
            where (project.RelativePath.EndsWith("proj"))
            select project.ProjectName
    }

let getSolutionNameFromPath (path: string) =
    Path
        .GetFileName(path)
        .Replace(".sln", String.Empty)

let writeSolutionProjects (solution: SolutionData) =
    let solutionTree =
        Tree($"[bold]{solution.Name}[/]")

    solutionTree.Style <- Style(Color.Green)
    solutionTree.AddNodes(getSolutionProjectNames (solution.File))

    AnsiConsole.WriteLine String.Empty
    AnsiConsole.Write solutionTree
    AnsiConsole.WriteLine String.Empty

let promptSolution () : SolutionData=
    let path = promptPath (".sln")
    let file = SolutionFile.Parse(path)
    let name = getSolutionNameFromPath path
    {Name = name;Path=path;File = file}