module JsonToNpgsql

open JsonResult
open Npgsql
open System.IO
open System

type dbTarget = 
    { table : string
      column : string }

let insertJsonInto conn table column json = 
    let value = json.ToString()
    let commandString = sprintf "insert into %s (%s) values ('%s')" table column value
    use command = new NpgsqlCommand(commandString, conn)
    command.ExecuteNonQuery()
    
let timeOnDate (tw : TextWriter) (date : DateTime) = tw.Write("{0:hh:mm:ss:fff} on {0:MM/dd/yy}", date)

let pushTo (connection:string) target (records : seq<string>) = 
    use conn = new NpgsqlConnection(connection)
    records
    |> Seq.map result.Parse
    |> Seq.iter (fun record -> 
           try 
               conn.Open()
               let rowsAffected = insertJsonInto conn target.table target.column record.JsonValue
               printfn "Inserted %d rows at %a" rowsAffected timeOnDate DateTime.Now
           finally
               conn.Close())
