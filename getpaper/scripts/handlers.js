var handlers = [{ /* pdf */
  type: "pdf",
  match: function (parser) {
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
  handle: function(parser, tabId) {
    var url = parser.href;
    var start = url.lastIndexOf("/") + 1;
    var name = url.substring(start, url.length-4 /*remove .pdf extension*/);
    showPageAction(tabId, url, name, false, true);  
  }
}, {
  type: "Wiley",
  //_reg: new RegExp(".*?/10[.][0-9]{4,}(?:[.][0-9]+)*/(.*?)/", "i"), // match wiley style article urls, ignore case
  _reg: new RegExp(".*?/(10[.][0-9]{4,}(?:[.][0-9]+)*)/(.*?)/", "i"), // match wiley style article urls, ignore case
  match: function (parser) {
    return (parser.hostname === "onlinelibrary.wiley.com" && parser.pathname.indexOf('/doi/') === 0) || (parser.hostname === "doi.wiley.com" && parser.pathname.indexOf('/10.') === 0);
  },
  handle: function(parser, tabId) {
    // parse the url
    var arr = this._reg.exec(parser.href + '/'); // make sure it ends with /
    //var paperLink = arr[0] + "pdf";
    //var paperName = arr[1];
    var paperLink = "http://onlinelibrary.wiley.com/doi/" + arr[1] + "/" + arr[2] + "/pdf";
    var paperName = arr[2];
    
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
  type: "PLOS",
  _reg: new RegExp("(?:www[.])?plos.*?[.]org", "i"), // match the different plos journals, ignore case
  match: function (parser) {
    return this._reg.exec(parser.hostname);
  },
  handle: function(parser, tabId) {
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
}, {
  type: "ScienceDirect",
  match: function (parser) {
    return parser.hostname === "www.sciencedirect.com" && parser.pathname.toLowerCase().indexOf("/science/article/pii/") === 0;
  },
  handletxt: function (text, tabId) {
    var pdflinkreg = new RegExp('pdfurl="(.*?://www\.sciencedirect\.com/science/article/pii/.*?pid=(.*?)\.pdf)"', 'i');
    var arr = pdflinkreg.exec(response.divhtml);
    if (arr) {
      showPageAction(tabId, arr[1], arr[2], true, true);
    } else if (text.match("ShoppingCartURL")) {
      // TODO provide alternative
    } else {
      // show warning => nothing found, something went wrong => auto report this url
    }
  },
  handle: function (parser, tabId) {
    chrome.tabs.sendMessage(tabId, {action:"FindScienceDirectPdfLink"}, function (response){
      if (response && response.url) {
        var hasAccess = response.url.indexOf("ShoppingCartURL") < 0;
        if (hasAccess) {
          var articleName = response.url.substring(response.url.indexOf("&pid=")+5).slice(0,-4);
          showPageAction(tabId, response.url,articleName, true,true);
        } else {
          // TODO provide alternative => Google Scholar ?
        }
      }
    });
  }
}, {
  type: "PubMed Central",
  _reg: new RegExp('^/pmc/articles/PMC[0-9]*/?.*?', 'i'),
  handletxt: function (text, tabId) {
    var pdflinkreg = new RegExp('href="(/pmc/articles/PMC[0-9]*/pdf/(.*?).pdf)"', 'i');
    var arr = pdflinkreg.exec(text);
    showPageAction(tabId, arr[1], arr[2],true,true);
  },
  match: function (parser) {
    return parser.hostname === "www.ncbi.nlm.nih.gov" && this._reg.exec(parser.pathname);
  },
  handle: function (parser, tabId) {
    chrome.tabs.sendMessage(tabId, {action: "FindPubMedCentralFormatsDiv"}, function (response) {
      if (response && response.divhtml) {
        this.handletxt(response.divhtml, tabId);
      }
    });
  }
}];

function handleMeta(parser, tabId) {
  for (var i = 0; i < handlers.length; i++) {
    var h = handlers[i];
    if (h.match(parser)) {
      if (h.handletxt) {
        $.get(parser.href, function( data ) {
          h.handletxt(data,tabId);
        });
      }
    } else {
      h.handle(parser, tabId);
    }
  };
}

var metahandlers = [{
  type: "PubMed",
  _reg: new RegExp("^/pubmed/[0-9]*", "i"),
  match: function (parser) {
    return parser.hostname === "www.ncbi.nlm.nih.gov" && this._reg.exec(parser.pathname);
  },
  handle: function (parser, tabId) {
    chrome.tabs.sendMessage(tabId, {action:"FindPubMedArticleLink"}, function (response) {
      if (response && response.url) {

        parser.href = response.url;

        handleMeta(parser, tabId);
      }
    });
  }
}];