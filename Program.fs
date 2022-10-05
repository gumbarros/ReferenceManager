module ReferenceManager.Program

open ReferenceManager.ListDependencies
open ReferenceManager.ReplaceDependency
open ReferenceManager.Utils
open ReferenceManager.SolutionManager

writeAppTitle()

let rec loadSolution() =
    let solution : SolutionData = promptSolution()
    
    writeSolutionProjects(solution)
    
    while true do 
        let selectedChoice = promptChoice()

        match selectedChoice with
        | 0 -> listDependenciesFromProject solution.File
        | 1 -> replaceDependencies solution.File ()
        | 2 -> loadSolution()
        | _ -> exit(1)

loadSolution()