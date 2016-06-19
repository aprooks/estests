namespace EsTests

open System
open HttpClient
open Newtonsoft.Json

module Json =
   let settings = Newtonsoft.Json.JsonSerializerSettings()
   settings.NullValueHandling <- Newtonsoft.Json.NullValueHandling.Ignore

   let serialize obj = (obj,settings)|> Newtonsoft.Json.JsonConvert.SerializeObject

   let deserialize<'a> input = Newtonsoft.Json.JsonConvert.DeserializeObject<'a> input

module EventStore =
  type EventData = {
    EventId:String
    EventType:String
    Data:String
    Metadata:String
  }

  let postEvent uri (usr, password) stream  (envelopes:EventData seq)=
                                                        createRequest Post <| uri+"streams/" + stream
                                                        |> withHeader (ContentType "application/vnd.eventstore.events+json")
                                                        |> withHeader (Accept "application/vnd.eventstore.atom+json")
                                                        |> withBasicAuthentication usr password
                                                        |> withBody (Json.serialize envelopes)

  let readStream uri (usr, password) stream =
                        createRequest Get <| (uri + "streams/"+stream)
                        |> withHeader (Accept "application/vnd.eventstore.atom+json")
                        |> withBasicAuthentication usr password

  let wrap ev =
      {
          EventId = System.Guid.NewGuid().ToString()
          EventType = ev.GetType().Name
          Data = Json.serialize ev
          Metadata = ""
      }
