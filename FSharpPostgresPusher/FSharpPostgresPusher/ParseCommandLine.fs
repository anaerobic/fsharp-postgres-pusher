module ParseCommandLine

type ImportMethod = 
    | BulkCopy
    | Insert

let getImportMethod (value:string) = 
    let lower = value.ToLower()
    match lower with
    | "bulkcopy" -> Some BulkCopy
    | "insert" -> Some Insert
    | _ -> None

type args = 
    { importMethod : ImportMethod
      server : string
      port : int
      userId : string
      pwd : string
      db : string
      table : string
      column : string }

let defaults = 
    { importMethod = ImportMethod.Insert
      server = "127.0.0.1"
      port = 5432
      userId = "postgres"
      pwd = ""
      db = ""
      table = ""
      column = "" }

type parseResult = 
    | Args of args
    | ParseError of string

let rec parseArgs (lst : string list) (defaults : parseResult) : parseResult = 
    match defaults with
    | ParseError message -> defaults
    | Args prev -> 
        match lst with
        | "/i" :: tail -> parseArgs tail.Tail <| 
                                        match getImportMethod tail.Head with
                                        | Some x -> Args { prev with importMethod = x }
                                        | None -> ParseError(sprintf "Invalid parameter: %A" tail.Head)
        | "/s" :: tail -> parseArgs tail.Tail <| Args { prev with server = tail.Head }
        | "/p" :: tail -> parseArgs tail.Tail <| Args { prev with port = int32 tail.Head }
        | "/u" :: tail -> parseArgs tail.Tail <| Args { prev with userId = tail.Head }
        | "/pw" :: tail -> parseArgs tail.Tail <| Args { prev with pwd = tail.Head }
        | "/db" :: tail -> parseArgs tail.Tail <| Args { prev with db = tail.Head }
        | "/tbl" :: tail -> parseArgs tail.Tail <| Args { prev with table = tail.Head }
        | "/col" :: tail -> parseArgs tail.Tail <| Args { prev with column = tail.Head }
        | [] -> defaults
        | _ -> ParseError("Invalid parameter: " + lst.Head)
