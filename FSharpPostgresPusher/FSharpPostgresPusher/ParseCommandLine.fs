module ParseCommandLine

type args = 
    { server : string
      port : int
      userId : string
      pwd : string
      db : string
      table : string
      column : string }

let defaults = 
    { server = "127.0.0.1"
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
        | "/s" :: tail -> parseArgs tail.Tail <| Args { prev with server = tail.Head }
        | "/p" :: tail -> parseArgs tail.Tail <| Args { prev with port = int32 tail.Head }
        | "/u" :: tail -> parseArgs tail.Tail <| Args { prev with userId = tail.Head }
        | "/pw" :: tail -> parseArgs tail.Tail <| Args { prev with pwd = tail.Head }
        | "/db" :: tail -> parseArgs tail.Tail <| Args { prev with db = tail.Head }
        | "/tbl" :: tail -> parseArgs tail.Tail <| Args { prev with table = tail.Head }
        | "/col" :: tail -> parseArgs tail.Tail <| Args { prev with column = tail.Head }
        | [] -> defaults
        | _ -> ParseError("Invalid parameter: " + lst.Head)