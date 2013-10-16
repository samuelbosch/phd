// TEST URLS
/*
chrome://extensions/ => reload extension
http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/abstract
http://www.plosbiology.org/article/info%3Adoi%2F10.1371%2Fjournal.pbio.1001662;jsessionid=E6549AA0F55CA68AD9BBC6E9E33F5290
http://www.plosone.org/article/info:doi/10.1371/journal.pone.0073810;jsessionid=3B91B94815A356C9473DCE5D48A71CD4
http://www.geos.ed.ac.uk/~gisteac/gis_book_abridged/files/ch14.pdf
*/
// send request to contentscript on a tab

var tabs = [];
var activeTabId = -1;


function sendRequestToContent(tabId, request){
	chrome.tabs.sendRequest(tabId, request, function (response){
    // do something with response
  });
}

function showPageAction(tabId, link, name, showView, showDownload){
  tabs[tabId] = { link: link, name:name, showView:showView, showDownload:showDownload};
  // ... show the page action.  
  chrome.pageAction.show(tabId);
}


function getTabInfo(){
  return tabs[activeTabId];
}
// store the currently active tabId
function tabActivated(activeInfo){
  activeTabId = activeInfo.tabId;
}

var parser = document.createElement('a'); // more info on the parser https://gist.github.com/jlong/2428561

// Called when the url of a tab changes.
function tabUpdated(tabId, changeInfo, tab) {
  parser.href = tab.url;
  for (var i = 0; i < handlers.length; i++) {
    var handler = handlers[i];
    if(handler.match(parser)) {
      handler.handle(parser,tabId)
      return;
    }
  };

  tabs[tabId] = null;
  chrome.pageAction.hide(tabId);
  
  // add to handlers:
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


chrome.webRequest.onHeadersReceived.addListener(
  // PLOS content-disposition fix
  function (details) {
    if(details.url.indexOf("&representation=PDF") > 0) {
      var headers = details.responseHeaders;
      for (var i = 0; i < headers.length; i++) {
        if(headers[i].name.toUpperCase() === "Content-disposition".toUpperCase()) {
          headers.splice(i, 1);
        }
      };
      return {
          responseHeaders: headers
      };
    }
  }, {
      urls: ["*://*/article*"]
  }, ["blocking", "responseHeaders"]
);


//Content-Type: application/octet-stream
//Content-Disposition: attachment; filename= "lecture07.pdf"