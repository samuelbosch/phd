$(function() {
	var link = chrome.extension.getBackgroundPage().paperLink;
	if(link !== null){
		$("<a/>", { html:"Download paper",href:link}).appendTo("#download");
	}
});