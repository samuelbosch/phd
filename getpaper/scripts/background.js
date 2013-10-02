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

function isPdf(url) {
  // check if ends with .pdf
  var start = url.length -4
  var result = start > -1 && 
    /* . */ (url.charCodeAt(start) === 46) && 
    /* p */ (((url.charCodeAt(++start) - 80) |  32) === 32) && 
    /* d */ (((url.charCodeAt(++start) - 68) |  32) === 32) && 
    /* f */ (((url.charCodeAt(++start) - 70) |  32) === 32); 
  return result;
}

function handlePdf(parser, tabId, url) {
  var start = url.lastIndexOf("/") + 1;
  var name = url.substring(start, url.length-4 /*remove .pdf extension*/);
  showPageAction(tabId, url, name, false, true);  
}

/* Wiley */
var wileyR = new RegExp(".*?/doi/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/", "i"); // match wiley style article urls, ignore case
function isWiley(parser) {
  return parser.hostname === "onlinelibrary.wiley.com" && parser.pathname.indexOf('/doi/') === 0
}
function handleWiley(parser, tabId, url) {
  // parse the url
  var arr = wileyR.exec(url);
  var paperLink = arr[0] + "pdf";
  var paperName = arr[1];
  
  // wiley shows pdf in iframe so we need to get the url for it
  $.get(paperLink, function( data ) {
    var match = data.match(/<iframe.*?src="(.*?)"/);
    if(match) { // if iframe not found then probably no access => can this be identified earlier ?
      paperLink = match[1];
      showPageAction(tabId, paperLink, paperName,true,true);
    }
    else {
      // TODO provide alternative => Google Scholar ?
    }
  });
}

/* PLOS */
var plosHostR = new RegExp("(?:www[.])?plos.*?[.]org", "i"); // match the different plos journals, ignore case
function isPlos(parser) {
  return plosHostR.exec(parser.hostname);
}
function handlePlos(parser, tabId, url) {
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
    showPageAction(tabId, link, articleName,true,true);
  }
}

var parser = document.createElement('a'); // more info on the parser https://gist.github.com/jlong/2428561

// Called when the url of a tab changes.
function tabUpdated(tabId, changeInfo, tab) {
  if (isPdf(tab.url)) {
    handlePdf(parser, tabId, tab.url);
    return;
  }

  parser.href = tab.url;

  if (isWiley(parser)) {
    handleWiley(parser, tabId, tab.url);
  }
  else if (isPLos(parser)) {
    handlePlos(parser, tabId, tab.url)
  }
  else {
    tabs[tabId] = null;
    chrome.pageAction.hide(tabId);
  }

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