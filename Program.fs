module ReferenceManager.Program

open ReferenceManager.References
open ReferenceManager.Utils
open ReferenceManager.SolutionManager

writeAppTitle()

let rec loadSolution() =
    let solution : SolutionData = promptSolution()
    
    writeSolutionProjects(solution)
    
    while true do 
        let selectedChoice = promptChoice()

        match selectedChoice with
        | 0 -> listReferencesFromProject solution.File
        | 1 -> replacePackageByProjectReference solution.File 
        | 2 -> loadSolution ()
        | _ -> exit(1)

loadSolution()