function startTogetherJs(tabId, name) {
  chrome.tabs.sendMessage(tabId, {action: 'Discuss', name: name}, null);
}
function load() {
  debugger;
  var info = chrome.extension.getBackgroundPage().getTabInfo();
  if(info) {
    if (info.showView) {
      $("<a/>", { html:"View",href:info.link, target:"_blank"}).appendTo("#download");
    }
    if (info.showView && info.showDownload) {
      $("<br />").appendTo("#download");
    }
    if (info.showDownload) {
      // create download link, file extension is .paper instead of .pdf to prevent chrome warnings
      $("<a/>", { html:"Download", href:info.link, download: info.name + ".paper"}).appendTo("#download");
    }
    if (info.showView || info.showDownload) {
      $("<br />").appendTo("#download");
      $("<a/>", { html:"Discuss", href:"#"}).click(function(){ startTogetherJs(info.tabId, info.name)}).appendTo("#download");
    }
  }
}

$(function() {
   load();
});