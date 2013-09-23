 (function( outliers, $, undefined ) {

  outliers.map = null;
  outliers.presences = null;
  outliers.selected = null;
  var selectFeature = null;

    outliers.init = function() { // private function
      
      // if initializing for the second time then first destroy existing map
      if (outliers.map) { 
        outliers.map.destroy(); 
      }

      var map = new OpenLayers.Map('map');

      var wms = new OpenLayers.Layer.WMS(
        "OpenLayers WMS",
        "http://vmap0.tiles.osgeo.org/wms/vmap0",
        {'layers':'basic'} );

      var presences = new OpenLayers.Layer.Vector("Presences", {
        projection: "EPSG:4326",
        strategies: [new OpenLayers.Strategy.Cluster()],
        styleMap: new OpenLayers.StyleMap({
          "default": { 
            pointRadius: 3, 
            fillColor: "#ffcc66",
            fillOpacity: 0.8,
            strokeColor: "#cc6633",
            strokeWidth: 2,
            strokeOpacity: 0.4
          }
        }),
        renderers: ["Canvas", "SVG", "VML"]
      });

      var selected = new OpenLayers.Layer.Vector("Selected", {
        projection: "EPSG:4326",
        styleMap: new OpenLayers.StyleMap({
          "default": { 
            pointRadius: 3, 
            fillColor: "#ff0000",
            fillOpacity: 0.6,
            strokeColor: "#a60000",
            strokeWidth: 2,
            strokeOpacity: 0.6,
            graphicZIndex:10000
          }
        }),
        renderers: ["Canvas", "SVG", "VML"]
      });

      var half = (coord.length/2);
      var features = [];
      for (var x=0;x<half;x++) { 
        var y = x + half
        features.push(new OpenLayers.Feature.Vector(
            new OpenLayers.Geometry.Point(coord[x], coord[y]), {id:x+1}));// 1 based id
      }
      presences.addFeatures(features);

      // don't change the order otherwise the features from selected dissappear on zoom
      map.addLayer(selected);
      map.addLayer(presences);
      map.addLayer(wms);
      // put features from selected on top of the presences
      $("#" + selected.id).css('z-index',500);

      map.setCenter(new OpenLayers.LonLat(0, 0), 2);

      outliers.presences = presences;
      outliers.selected = selected;
      outliers.map = map;
    }
    
    outliers.selectPoint = function(which, removeAllFeatures) {
      removeAllFeatures = typeof removeAllFeatures !== 'undefined' ? removeAllFeatures : true;
      var f = new OpenLayers.Feature.Vector(outliers.presences.features[which-1].geometry);
      if(removeAllFeatures){
        outliers.selected.removeAllFeatures();
      }
      outliers.selected.addFeatures([f]);
      if(outliers.selected.features.length > 1) { 
        outliers.map.zoomToExtent(outliers.selected.getDataExtent());
      } else {
        outliers.map.setCenter(new OpenLayers.LonLat(f.geometry.x, f.geometry.y));
      }
      outliers.selected.redraw();
    }
    outliers.selectPoints = function(values) {
      if(values.length > 0){
        outliers.selectPoint(parseInt(values[0]), true);
        for(var i = 1; i < values.length; i++){
          outliers.selectPoint(parseInt(values[i]), false);
        }
      }
    }
    outliers.clearSelection = function(){
      outliers.selected.removeAllFeatures();
    }

    
  }( window.outliers = window.outliers || {}, jQuery ));


// TODO On click map show info about point