function load(){
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

    }
}

$(function() {
   load();
});