module ReferenceManager.SolutionUtils

open Microsoft.Build.Construction

let getSolutionProjectNames (solution : SolutionFile) =
    query {
        for project in solution.ProjectsInOrder do
            where (project.RelativePath.EndsWith("proj"))
            select project.ProjectName
    }