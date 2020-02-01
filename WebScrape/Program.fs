open System
open System.IO
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

let baseUrl = "https://www.nplainfield.org"

type link = { 
    name : string
    url : string }

let response = 
    Http.RequestString
        (  baseUrl + "/apps/pages/index.jsp?uREC_ID=341383&type=d&termREC_ID=&pREC_ID=636149&hideMenu=1",
        httpMethod = "Get",
        headers = [UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36 Edg/79.0.309.71") ])

let findAllHrefs (results : HtmlDocument) =
    results.Descendants ["a"]
    |> Seq.choose ( fun x -> x.TryGetAttribute("href") |> Option.map(fun a ->  { name = x.InnerText(); url = a.Value() }))

let filterForMeetingMintes listOfHref =
    listOfHref
    |> Seq.filter(fun x -> x.name.Contains("Meeting Minutes"))

let callPages links =
    let isFullUrl (url : string) =
        url.Contains("http")

    let buildUrl url =
        match isFullUrl url with
        | true -> url
        | false -> baseUrl + url

    Seq.map(fun x -> {url = buildUrl x.url; name = x.name}) links

[<EntryPoint>]
let main argv =
    let result =
        File.ReadAllText("nplainfieldStart.txt")
        |> HtmlDocument.Parse
        |> findAllHrefs
        |> filterForMeetingMintes
        |> callPages


    Console.Write(result)
    Console.ReadLine() |> ignore
    0 // return an integer exit code
