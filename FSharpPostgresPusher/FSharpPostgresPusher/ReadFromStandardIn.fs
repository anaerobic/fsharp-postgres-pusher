module ReadFromStandardIn

open System
open System.IO

let readIt (input : StreamReader) = 
    input |> Seq.unfold (fun reader -> 
                 match reader.ReadLine() with
                 | null -> 
                     reader.Dispose()
                     None
                 | line -> Some(line, reader))
