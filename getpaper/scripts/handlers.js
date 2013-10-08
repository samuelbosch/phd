var handlers = [{ /* pdf */
  this.type: "pdf",
  this.match : function (parser) {
    // check if ends with .pdf
	var url = parser.href;
    var start = url.length -4
    var result = start > -1 && 
    /* . */ (url.charCodeAt(start) === 46) && 
    /* p */ (((url.charCodeAt(++start) - 80) |  32) === 32) && 
    /* d */ (((url.charCodeAt(++start) - 68) |  32) === 32) && 
    /* f */ (((url.charCodeAt(++start) - 70) |  32) === 32); 
    return result;
  },
  this.handle: function(parser, tabId) {
	var url = parser.href;
    var start = url.lastIndexOf("/") + 1;
    var name = url.substring(start, url.length-4 /*remove .pdf extension*/);
    showPageAction(tabId, url, name, false, true);  
  }
}, {
  this.type: "Wiley",
  this._reg = new RegExp(".*?/doi/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/", "i"), // match wiley style article urls, ignore case
  this.match : function (parser) {
    return parser.hostname === "onlinelibrary.wiley.com" && parser.pathname.indexOf('/doi/') === 0
  },
  this.handle: function(parser, tabId) {
    // parse the url
    var arr = this._reg.exec(parser.href);
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
}, {
  this.type: "PLOS",
  this._reg = new RegExp("(?:www[.])?plos.*?[.]org", "i"), // match the different plos journals, ignore case
  this.match: function (parser) {
    return this._reg.exec(parser.hostname);
  },
  this.handle: function(parser, tabId) {
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
}];