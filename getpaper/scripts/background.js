var paperLink;


// send request to contentscript on a tab

function sendRequestToContent(request){
	chrome.tabs.sendRequest(tabId, request, function (response){});
}



// Called when the url of a tab changes.
function tabsUpdated(tabId, changeInfo, tab) {
  // if wiley article
  if (tab.url.indexOf('//onlinelibrary.wiley.com/doi/') > -1) {
    // ... show the page action.
    chrome.pageAction.show(tabId);

    // todo add an extra link to the page for direct download !
    paperLink = "http://www.google.com/"
  }
  else {
  	paperLink = null;
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
chrome.tabs.onUpdated.addListener(tabsUpdated);

// Listen for the content script to send a message to the background page.
chrome.extension.onRequest.addListener(onRequest);