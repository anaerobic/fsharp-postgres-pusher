open System
open Npgsql

let insertInto connection commandString = 
    let commandString = sprintf commandString
    use command = new NpgsqlCommand(commandString, connection)
    command.ExecuteNonQuery()

let insertJsonInto connection table column value = 
    let commandString = sprintf "insert into %s (%s) values ('%s')" table column value
    use command = new NpgsqlCommand(commandString, connection)
    command.ExecuteNonQuery()

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

let rec readLine callback = 
    let line = Console.ReadLine()
    if (line <> null) then 
        callback line
        readLine callback

let usage = """
Usage: PostgresPusher [/s <server>] [/p <port>] [/u <userId>]
                      [/pw <password>] [/db <database>]
                      [/tbl <table>] [/col <column_for_value>]
/c <server> - Default: 127.0.0.1 The IP of the PostgreSQL server
/d <port>   - Default: 5432
/u <userId> - Default: postgres
/pw <password>
/db <database>
/tbl <table>
/col <column_for_value>
            """

[<EntryPoint>]
let main argv = 
    let args = parseArgs <| Array.toList argv <| Args defaults
    match args with
    | ParseError message -> 
        Console.WriteLine message
        Console.WriteLine usage
        match Environment.UserInteractive with
        | true -> 
            Console.WriteLine "Press any key to exit..."
            Console.ReadKey() |> ignore
        1
    | Args options ->
        let connString = sprintf "Server=%s;Port=%d;User Id=%s;Password=%s;Database=%s" options.server options.port options.userId options.pwd options.db
        use conn = new NpgsqlConnection(connString)
        try 
            conn.Open()
            readLine (fun line -> insertJsonInto conn options.table options.column line |> printfn "Inserted %d rows")
        finally
            conn.Close()
        0
