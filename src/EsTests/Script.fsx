#load "Scripts/load-project-debug.fsx"

open HttpClient

open System
open System.Net
open System.Text

open EsTests

let esUri = "http://localhost:2113/"
let credentials = ("admin","changeit")

let reader = EventStore.readStream esUri credentials

let eventUploader = EventStore.postEvent esUri credentials

let all = reader "$all" |> getResponse
all.EntityBody


type TestEvent = {
    Data:string
  }

eventUploader "test2" [EventStore.wrap {Data="Hello world"}] |> getResponse
