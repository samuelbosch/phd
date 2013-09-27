
// send request to contentscript on a tab

function sendRequestToContent(tabId, request){
	chrome.tabs.sendRequest(tabId, request, function (response){
    // do something with response
  });
}
var tabs = [];
var activeTabId = -1;
function getTabInfo(){
  return tabs[activeTabId];
}
// store the currently active tabId
function tabActivated(activeInfo){
  activeTabId = activeInfo.tabId;
}

// Called when the url of a tab changes.
function tabUpdated(tabId, changeInfo, tab) {
  // if wiley article
  if (tab.url.indexOf('//onlinelibrary.wiley.com/doi/') > -1) {
    // parse the url
    var r = new RegExp(".*?/doi/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/");
    var arr = r.exec(tab.url)
    var paperLink = arr[0] + "pdf"
    var paperName = arr[1];
    
    // wiley shows pdf in iframe so we need to get the url for it
    $.get(paperLink, function( data ) {
      var match = data.match(/<iframe.*?src="(.*?)"/);
      if(match) { // if iframe not found then probably no access => can this be identified earlier ?
        paperLink = match[1];
        tabs[tabId] = { link: paperLink, name:paperName};
        // ... show the page action.  
        chrome.pageAction.show(tabId);
      }
      else {
        // TODO provide alternative => Google Scholar ?
      }
    });
  }
  else {
    tabs[tabId] = null;
    chrome.pageAction.hide(tabId);
  }

  // PLOS
  // sciencedirect
  // pubmed
  // SFX
  // show page action when open file is a pdf (detect document type ? or .pdf extension)
  // pdf's from google scholar (radd download link)
  // direct integration with web of knowledge ?
  // direct integration with Google Reader ?
  
  // integration with mendeley => indicate that you already have this pdf in your library

};


// // Called when the content script sends a request
// function contentRequest(request, sender, sendResponse) {
//   // Show the page action for the tab that the sender (content script)
//   // was on.
//   chrome.pageAction.show(sender.tab.id);

//   // Return nothing to let the connection be cleaned up.
//   sendResponse({});
// };


// listeners

// Listen tab updates
chrome.tabs.onUpdated.addListener(tabUpdated);
chrome.tabs.onActivated.addListener(tabActivated)

// Listen for the content script to send a message to the background page.
//chrome.extension.onRequest.addListener(onRequest);