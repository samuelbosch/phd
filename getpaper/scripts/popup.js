function load(){
	var link = chrome.extension.getBackgroundPage().paperLink;
	var name = chrome.extension.getBackgroundPage().paperName;
	if(link) {
		$("<a/>", { html:"View",href:link, target:"_blank"}).appendTo("#download");
		$("<br />").appendTo("#download");
		// create download link, file extension is .paper instead of .pdf to prevent chrome warnings
		$("<a/>", { html:"Download", href:link, download: name + ".paper", target:"_blank"}).appendTo("#download");
    }
}

$(function() {
   load();
});