// message from background
chrome.runtime.onMessage.addListener(
  function(message, sender, sendResponse) {
    if (message.action === "FindScienceDirectPdfLink") {
      var url = null;
      var e = document.getElementById('pdfLink');
      if (e) {
        url = e.href;
      }
      sendResponse({url: url}); 
    }
    else if (message.action === "Discuss" && document.getElementById('getpaper.togetherjs') === null) {
      // first add TogetherJS configuration parameters
      var s = document.createElement('script');
      s.innerText = "TogetherJSConfig_autoStart = true;"
       + "TogetherJSConfig_dontShowClicks = true;"
       + "TogetherJSConfig_findRoom = {prefix: 'getpaper', max: 1000 };"
       + "TogetherJSConfig_suppressJoinConfirmation = true;"
       + "TogetherJSConfig_storagePrefix = '"+message.name+"';";
      // then load the script
      document.body.appendChild(s);
      s = document.createElement('script');
      s.id = 'getpaper.togetherjs';
      s.src = 'https://togetherjs.com/togetherjs.js';
      s.type = 'text/javascript';
      s.async = false;
      document.body.appendChild(s);
    }
});


// request from background page
// if (window == top) {
//   chrome.extension.onRequest.addListener(function(request, sender, sendResponse) {

//      // if requested to add buttons => then do it, main logic stays in background.js
    
    
//   });
// }

// var regex = /sandwich/;

// // Test the text of the body element against our regular expression.
// if (regex.test(document.body.innerText)) {
//   // The regular expression produced a match, so notify the background page.
//   chrome.extension.sendRequest({}, function(response) {});
// } else {
//   // No match was found.
// }