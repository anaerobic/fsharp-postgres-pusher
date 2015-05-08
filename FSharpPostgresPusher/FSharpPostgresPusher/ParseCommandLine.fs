module ParseCommandLine

type ImportMethod = 
    | BulkCopy = 0
    | Insert = 2

let getImportMethod value = ImportMethod.Parse(typeof<ImportMethod>, value) :?> ImportMethod

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
        | "/i" :: tail -> parseArgs tail.Tail <| Args { prev with importMethod = getImportMethod tail.Head }
        | "/s" :: tail -> parseArgs tail.Tail <| Args { prev with server = tail.Head }
        | "/p" :: tail -> parseArgs tail.Tail <| Args { prev with port = int32 tail.Head }
        | "/u" :: tail -> parseArgs tail.Tail <| Args { prev with userId = tail.Head }
        | "/pw" :: tail -> parseArgs tail.Tail <| Args { prev with pwd = tail.Head }
        | "/db" :: tail -> parseArgs tail.Tail <| Args { prev with db = tail.Head }
        | "/tbl" :: tail -> parseArgs tail.Tail <| Args { prev with table = tail.Head }
        | "/col" :: tail -> parseArgs tail.Tail <| Args { prev with column = tail.Head }
        | [] -> defaults
        | _ -> ParseError("Invalid parameter: " + lst.Head)
