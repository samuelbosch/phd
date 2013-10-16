// for more info on Jasmine see: http://pivotal.github.io/jasmine/

describe("Background", function() {
  
  describe("pdf", function() {
    it("should be able to recognize a pdf link", function() {
      parser.href= "http://www.geos.ed.ac.uk/~gisteac/gis_book_abridged/files/ch14.pdf";
      expect(handlers[0].match(parser)).toBeTruthy();
    });
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

    it("should be able to recognize a wiley url", function() {
      parser.href = "http://onlinelibrary.wiley.com/doi/10.1111/j.0906-7590.2008.5203.x/abstract";
      expect(handler.match(parser)).toBeTruthy();
    });

    it("should be able to recognize a doi wiley url", function() {
      parser.href = "http://doi.wiley.com/10.1111/j.0906-7590.2008.5203.x";
      expect(handler.match(parser)).toBeTruthy();
    });

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

    it("should be able to recognize different PLOS urls", function() {
      var urls = [
        "http://www.plosbiology.org/article/info%3Adoi%2F10.1371%2Fjournal.pbio.1001662;jsessionid=E6549AA0F55CA68AD9BBC6E9E33F5290",
        "http://www.plosone.org/article/info:doi/10.1371/journal.pone.0073810;jsessionid=3B91B94815A356C9473DCE5D48A71CD4",
        "http://www.plosmedicine.org/article/info%3Adoi%2F10.1371%2Fjournal.pmed.1001524",
        "http://www.ploscompbiol.org/article/info%3Adoi%2F10.1371%2Fjournal.pcbi.1003245;jsessionid=118C3011A3E0E4D0222202014BADFCA4",
        "http://www.plosgenetics.org/article/info%3Adoi%2F10.1371%2Fjournal.pgen.1003792;jsessionid=15EF7A3F094ABB7A2E535178E998099F",
        "http://www.plospathogens.org/article/info%3Adoi%2F10.1371%2Fjournal.ppat.1003655;jsessionid=69461A0DCB11A812822DDA36973B21AF",
        "http://www.plosntds.org/article/info%3Adoi%2F10.1371%2Fjournal.pntd.0002435;jsessionid=729BBE93303B8F4A992D92FEF54CA54E"
      ];
      for (var i = 0; i < urls.length; i++) {
        parser.href = urls[i]
        expect(handler.match(parser)).toBeTruthy();
      };
    });
    
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
});