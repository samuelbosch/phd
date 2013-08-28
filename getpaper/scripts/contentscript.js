
// request from background page
if (window == top) {
  chrome.extension.onRequest.addListener(function(req, sender, sendResponse) {

  	// if requested to add buttons => then do it, main logic stays in background.js

    sendResponse({});
  });
}

// var regex = /sandwich/;

// // Test the text of the body element against our regular expression.
// if (regex.test(document.body.innerText)) {
//   // The regular expression produced a match, so notify the background page.
//   chrome.extension.sendRequest({}, function(response) {});
// } else {
//   // No match was found.
// }