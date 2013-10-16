[<FunScript.JS>]
module Background

open FunScript
open FunScript.TypeScript

type c = Api<"../Typings/chrome.d.ts">
type chrome = c.chrome
type l = Api<"../Typings/lib.d.ts">

type tab = { link:string; name:string}

let mutable tabs = Map.empty<float, tab>
let mutable activeTabId = -1.0
//let tabActivated (

let parser = l.document.createElement("a")

let buildRegExp pattern ignoreCase = 
    let r = new l.RegExp()
    r.source <- pattern
    r.ignoreCase <- ignoreCase
    r

let plosHostR = buildRegExp "(?:www[.])?plos.*?[.]org" true // match plos style urls, ignore case
let wileyR = buildRegExp ".*?/doi/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/" true // match wiley style article urls, ignore case

let showPageAction tabId link name =
    tabs <- Map.add tabId { link=link; name=name } tabs
    chrome.pageAction.show(tabId)

let tabActivated (activeInfo:chrome.tabs.TabActiveInfo) = 
    activeTabId <- activeInfo.tabId

let tabUpdated (tabId:float) (changeInfo:chrome.tabs.TabChangeInfo) (tab:chrome.tabs.Tab) = 
    showPageAction tabId "" ""
//
//let removeContentDisposition (details:chrome.webRequest.OnHeadersReceivedDetails) =
//    if details.url.Contains("=PDF") then
//        

let main() = 
    chrome.tabs.onUpdated.addListener(new System.Func<float,chrome.tabs.TabChangeInfo,chrome.tabs.Tab,Unit>(tabUpdated))
    chrome.tabs.onActivated.addListener(new System.Func<chrome.tabs.TabActiveInfo,Unit>(tabActivated))
    //chrome.webRequest.onHeadersReceived.addListener(