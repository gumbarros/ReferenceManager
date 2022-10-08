module ReferenceManager.Extensions

open System
open System.Text.RegularExpressions
open Spectre.Console

type String with
    member string.ReplaceWholeWord(find : String, replace: String) =
        let textToFind = String.Format(@"\b{0}\b", find)
        Regex.Replace(string, textToFind,replace)
        
type AnsiConsole with
    static member EmptyLine =
        AnsiConsole.WriteLine String.Empty