#load "Scripts/load-project-debug.fsx"

open HttpClient

open System
open System.Net
open System.Text
open System.Net

open EsTests

let esUri = Uri "http://localhost:2113/"

let es = esUri.ToString()
let credentials = ("admin","changeit")

let reader = EventStore.readStream es  credentials

let eventUploader = EventStore.postEvent es credentials

let all = reader "$streams" |> getResponse
printfn "%s" all.EntityBody.Value


type OrderCollected = {
  ItemIds : string List
  At      : DateTime
  AgentId : string
}

type AgentEvents =
| OrderCollected of OrderCollected


let sampleAgentEvent = {
  ItemIds = {'a'..'z'} |> Seq.map (string) |> Seq.toList
  AgentId = "3551"
  At = DateTime.Now}

{0..100}
|> Seq.iter
      (fun i-> eventUploader
                          "agent-3551"
                          [{
                              EventStore.EventId=Guid.NewGuid().ToString("N")
                              EventStore.Data = sampleAgentEvent|>Json.serialize
                              EventStore.EventType="AgentEvents.OrderCollected"
                              EventStore.Metadata =""
                          }]
                          |> getResponseCode
                          |> printfn "%d"
        )

let agentStream = reader "agent-3551/0/forward/100?embed=tryharder" |> getResponseBody
printfn "%s" agentStream
