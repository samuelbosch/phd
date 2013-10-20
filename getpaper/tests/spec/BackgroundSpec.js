// for more info on Jasmine see: http://pivotal.github.io/jasmine/

describe("Background", function() {

  function itShouldRecognize(msg, handler, urls) {
    it("should be able to recognize " + msg, function() {
      for (var i = 0; i < urls.length; i++) {
        parser.href = urls[i];
        expect(handler.match(parser)).toBeTruthy();
      };
    });
  }


  
  describe("pdf", function() {
    itShouldRecognize("a pdf link", handlers[0], ["http://www.geos.ed.ac.uk/~gisteac/gis_book_abridged/files/ch14.pdf"]);
    
    it("should not falsly recognize non pdf links", function() {
      parser.href = "https://login.ugent.be/login?service=https%3A%2F%2Fwebmail.ugent.be%2Fhorde%2FloginForm.php";
      expect(handlers[0].match(parser)).toBeFalsy();
    });
    it("handle pdf should call showPageAction", function() {
      spyOn(window, 'showPageAction');
      var url = "http://www.geos.ed.ac.uk/~gisteac/gis_book_abridged/files/ch14.pdf";
      parser.href = url;
      handlers[0].handle(parser, 1);
      expect(window.showPageAction).toHaveBeenCalledWith(1, url, "ch14", false, true);
    });
  });

  describe("Wiley", function() {
    var handler = handlers[1];

    itShouldRecognize("a wiley url", handler,["http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/abstract"]);

    itShouldRecognize("a doi wiley url", handler, ["http://doi.wiley.com/10.1111/j.0906-7590.2008.5203.x"]);

    it("should do an ajax call to the pdf url", function() {
      spyOn($, 'get');
      parser.href = "http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/abstract";
      handler.handle(parser, 1);
      expect($.get).toHaveBeenCalledWith("http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/pdf", jasmine.any(Function))
    });

    it("from doi url should do an ajax call to the pdf url", function() {
      spyOn($, 'get');
      parser.href = "http://doi.wiley.com/10.1111/j.0906-7590.2008.5203.x";
      handler.handle(parser, 1);
      expect($.get).toHaveBeenCalledWith("http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/pdf", jasmine.any(Function))
    });

    // can't use the following 2 tests because they contain Ajax calls to Wiley
    // XMLHttpRequest cannot load http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/pdf.
    // No 'Access-Control-Allow-Origin' header is present on the requested resource. Origin 'null' is therefore not allowed access. 
    // http://stackoverflow.com/questions/9310112/why-am-i-seeing-an-origin-is-not-allowed-by-access-control-allow-origin-error
    xit("handle accessible papers", function() {
      spyOn(window, 'showPageAction');
      parser.href = "http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/abstract";
      handler.handle(parser, 1);
      expect(window.showPageAction).toHaveBeenCalledWith(1, jasmine.any(String), "j.0906-7590.2008.5203.x", true, true);
    });

    xit("handle non accessible papers", function(){
      var url = "http://onlinelibrary.wiley.com/doi/10.1111/2041-210X.12071/abstract";
      parser.href = url;
      expect(handler.handle(parser, 1)).toEqual("WAITING FOR IMPLEMENTATION");
    });
  });

  describe("PLOS", function() {
    var handler = handlers[2];

    itShouldRecognize("different PLOS urls", handler, [
        "http://www.plosbiology.org/article/info%3Adoi%2F10.1371%2Fjournal.pbio.1001662;jsessionid=E6549AA0F55CA68AD9BBC6E9E33F5290",
        "http://www.plosone.org/article/info:doi/10.1371/journal.pone.0073810;jsessionid=3B91B94815A356C9473DCE5D48A71CD4",
        "http://www.plosmedicine.org/article/info%3Adoi%2F10.1371%2Fjournal.pmed.1001524",
        "http://www.ploscompbiol.org/article/info%3Adoi%2F10.1371%2Fjournal.pcbi.1003245;jsessionid=118C3011A3E0E4D0222202014BADFCA4",
        "http://www.plosgenetics.org/article/info%3Adoi%2F10.1371%2Fjournal.pgen.1003792;jsessionid=15EF7A3F094ABB7A2E535178E998099F",
        "http://www.plospathogens.org/article/info%3Adoi%2F10.1371%2Fjournal.ppat.1003655;jsessionid=69461A0DCB11A812822DDA36973B21AF",
        "http://www.plosntds.org/article/info%3Adoi%2F10.1371%2Fjournal.pntd.0002435;jsessionid=729BBE93303B8F4A992D92FEF54CA54E"
      ]);
    
    it("handle paper links with normal doi", function() {
      spyOn(window, 'showPageAction');
      parser.href = "http://www.plosbiology.org/article/info%3Adoi%2F10.1371%2Fjournal.pbio.1001662;jsessionid=E6549AA0F55CA68AD9BBC6E9E33F5290";
      handler.handle(parser, 1);
      expect(window.showPageAction).toHaveBeenCalledWith(1, "http://www.plosbiology.org/article/fetchObject.action?uri=info%3Adoi%2F10.1371%2Fjournal.pbio.1001662&representation=PDF", "journal.pbio.1001662", true, true);
    });

    it("handle paper links with escaped doi", function() {
      spyOn(window, 'showPageAction');
      parser.href = "http://www.plosone.org/article/info:doi/10.1371/journal.pone.0073810";
      handler.handle(parser, 1);
      expect(window.showPageAction).toHaveBeenCalledWith(1, "http://www.plosone.org/article/fetchObject.action?uri=info%3Adoi%2F10.1371%2Fjournal.pone.0073810&representation=PDF", "journal.pone.0073810", true, true);
    });
  });
  

  describe("ScienceDirect", function() {
    var handler = handlers[3];
    
    itShouldRecognize("a ScienceDirect url", handler, ["http://www.sciencedirect.com/science/article/pii/S0143622813002154"]);

    it("handle should message the contentscript and handle its result", function () {
      var expectedUrl = "http://www.sciencedirect.com/science/article/pii/S0143622813002154/pdfft?md5=921be4e2eaa176b73acfed7fa37c3e44&pid=1-s2.0-S0143622813002154-main.pdf";
      window.chrome = { tabs: { sendMessage : function (tabId, message, callback) {
        expect(tabId).toEqual(1);
        expect(message.action).toEqual("FindScienceDirectPdfLink");
        callback({url: expectedUrl});
      }}}


      spyOn(window, 'showPageAction');
      parser.href = "http://www.sciencedirect.com/science/article/pii/S0143622813002154";
      handler.handle(parser, 1);
      expect(window.showPageAction).toHaveBeenCalledWith(1, expectedUrl, "1-s2.0-S0143622813002154-main", true, true);
    });
  });

  describe("PubMed", function() {
    var handler = handlers[4];
    var url = "http://www.ncbi.nlm.nih.gov/pubmed/23940805";
    
    itShouldRecognize("a Pubmed url",handler, [url]);

  });

 describe("PubMed Central", function() {
    var handler = handlers[5];
    var url = "http://www.ncbi.nlm.nih.gov/pmc/articles/PMC3733920/";
    
    itShouldRecognize("a Pubmed Central url",handler, [url]);

  });

});