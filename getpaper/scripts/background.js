
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
var parser = document.createElement('a');
var plosHostR = new RegExp() //= new RegExp("(.*?plos.*?[.org]/article/)(info%3Adoi%2F10[.][0-9]{4,}%2F.*?);")
var wileyR = new RegExp(".*?/doi/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/");
// parser.href = "http://example.com:3000/pathname/?search=test#hash";
 
// parser.protocol; // => "http:"
// parser.hostname; // => "example.com"
// parser.port;     // => "3000"
// parser.pathname; // => "/pathname/"
// parser.search;   // => "?search=test"
// parser.hash;     // => "#hash"
// parser.host;     // => "example.com:3000"

function showPageAction(tabId, link, name){
  tabs[tabId] = { link: link, name:name};
  // ... show the page action.  
  chrome.pageAction.show(tabId);
}

//http://www.plosbiology.org/article/fetchObject.action?uri=info%3Adoi%2F10.1371%2Fjournal.pbio.1001662&representation=PDF
// Called when the url of a tab changes.
function tabUpdated(tabId, changeInfo, tab) {
  parser.href = tab.url;

  // if wiley article
  if (parser.hostname === "onlinelibrary.wiley.com" && parser.pathname.indexOf('/doi/') === 0) {
    // parse the url
    
    var arr = wileyR.exec(tab.url);
    var paperLink = arr[0] + "pdf";
    var paperName = arr[1];
    
    // wiley shows pdf in iframe so we need to get the url for it
    $.get(paperLink, function( data ) {
      var match = data.match(/<iframe.*?src="(.*?)"/);
      if(match) { // if iframe not found then probably no access => can this be identified earlier ?
        paperLink = match[1];
        showPageAction(tabId, paperLink, paperName);
      }
      else {
        // TODO provide alternative => Google Scholar ?
      }
    });
  }
  else if (plosHostR.exec(parser.hostname)) {
    var splittedPath = parser.pathname.split("/");
    var article, articleName;
    if(splittedPath[1] === "article"){
      if(splittedPath[2] === "info:doi"){
        //info:doi/10.1371/journal.pone.0073810;jsessionid=3B91B94815A356C9473DCE5D48A71CD4
        articleName = splittedPath[4].split(";")[0];
        article = "info%3Adoi%2F" + splittedPath[3] + "%2F" + articleName;
      } else if (splittedPath[2].indexOf("info%3Adoi") == 0) {
        //info%3Adoi%2F10.1371%2Fjournal.pbio.1001662;jsessionid=E6549AA0F55CA68AD9BBC6E9E33F5290
        article = splittedPath[2].split(";")[0]; // strip session
        splittedArticle = article.split("%2F");
        articleName = splittedArticle[splittedArticle.length-1]; // name is last part 
      }
    }
    if(article && articleName) {
      var link = parser.protocol +"//"+ parser.host + "/article/fetchObject.action?uri=" + article + "&representation=PDF";
      showPageAction(tabId, link, articleName);
    }
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