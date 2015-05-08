module ReadFromStandardIn

open System
open System.IO

[<Obsolete>]
let rec readLine callback = 
    let line = Console.ReadLine()
    if (line <> null) then 
        callback line
        readLine callback

let readIt (input : StreamReader) = 
    input |> Seq.unfold (fun reader -> 
                 match reader.ReadLine() with
                 | null -> 
                     reader.Dispose()
                     None
                 | line -> Some(line, reader))
