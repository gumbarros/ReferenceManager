module ReferenceManager.Program

open Microsoft.Build.Locator
open ReferenceManager.Utils
open ReferenceManager.Solution
open ReferenceManager.ListReferences
open ReferenceManager.ReplaceReference
open System

try
    MSBuildLocator.RegisterDefaults() |> ignore
with
| :? ArgumentException -> setMSBuildPathFromAppSettings()

writeAppTitle()

let rec loadSolution() =
    
    let solution : SolutionData = promptSolution()

    writeSolutionProjects(solution)
    
    while true do 
        let selectedChoice = promptChoice()

        match selectedChoice with
        | 0 -> listReferencesFromProject solution
        | 1 -> replacePackageByProjectReference solution 
        | 2 -> loadSolution()
        | _ -> exit(1)

loadSolution()