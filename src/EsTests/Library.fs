namespace EsTests

open System
open HttpClient
open Newtonsoft.Json

module EventStore =
  type Envelope = {
    EventId:String
    EventType:String
    Data:String
    Metadata:String
  }

  let settings = Newtonsoft.Json.JsonSerializerSettings()
  settings.NullValueHandling <- Newtonsoft.Json.NullValueHandling.Ignore

  let postEvent uri (usr, password) stream  (envelopes:Envelope seq)=
                                                        createRequest Post <| uri+"streams/" + stream
                                                        |> withHeader (ContentType "application/vnd.eventstore.events+json")
                                                        |> withHeader (Accept "application/vnd.eventstore.atom+json")
                                                        |> withBasicAuthentication usr password
                                                        |> withBody ((envelopes, settings) |> Newtonsoft.Json.JsonConvert.SerializeObject)

  let readStream uri (usr, password) stream =
                        createRequest Get <| (uri + "streams/"+stream)
                        |> withHeader (Accept "application/vnd.eventstore.atom+json")
                        |> withBasicAuthentication usr password

  let wrap ev =
      {
          EventId = System.Guid.NewGuid().ToString()
          EventType = ev.GetType().FullName
          Data = (ev,settings)|> Newtonsoft.Json.JsonConvert.SerializeObject
          Metadata = ""
      }
