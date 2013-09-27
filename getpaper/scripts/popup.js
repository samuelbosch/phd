function load(){
	var info = chrome.extension.getBackgroundPage().getTabInfo();
	if(info) {
		$("<a/>", { html:"View",href:info.link, target:"_blank"}).appendTo("#download");
		$("<br />").appendTo("#download");
		// create download link, file extension is .paper instead of .pdf to prevent chrome warnings
		$("<a/>", { html:"Download", href:info.link, download: info.name + ".paper", target:"_blank"}).appendTo("#download");
    }
}

$(function() {
   load();
});