module ReferenceManager.ReplaceDependency

open System
open Microsoft.Build.Construction

let replaceDependencies(solution: SolutionFile) =
    ignore