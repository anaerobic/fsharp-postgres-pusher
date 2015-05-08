module JsonResult

open FSharp.Data

[<Literal>]
let resultSample = """
{ "groupName" : "Overall Checkpoint 1",
  "bib" : 4231,
  "time" : "00:00:02.8750000",
  "age" : 21 
}
"""

type result = JsonProvider<resultSample, RootName="result">
