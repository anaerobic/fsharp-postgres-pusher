module BulkCopyToNpgsql

open Npgsql

let bulkCopyStdInTo (connection:string) table stdin = 
    use conn = new NpgsqlConnection(connection)
    let commandString = sprintf "COPY %s FROM STDIN" table
    use command = new NpgsqlCommand(commandString, conn)
    let copyIn = new NpgsqlCopyIn(command, conn, stdin)
    try 
        copyIn.Start()
    with ex -> 
        try 
            copyIn.Cancel("Undo copy")
        with :? NpgsqlException as ex2 -> 
            match ex2.ToString().Contains("Undo copy") with
            | false -> failwithf "Failed to cancel copy: %A upon failure: %A" ex2 ex
