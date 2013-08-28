gdalwarp -of GTiff -multi -t_srs "+proj=cea +lon_0=0 +lat_ts=30 +x_0=0 +y_0=0 +datum=WGS84 +ellps=WGS84 +units=m +no_defs" "D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc" "D:\a\data\marspec\MARSPEC_30s\ascii_equalarea\bathy_30s.tiff"
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_30s\ascii_equalarea\bathy_30s.tiff" "D:\a\data\marspec\MARSPEC_30s\ascii_equalarea\bathy_30s.asc"
del "D:\a\data\marspec\MARSPEC_30s\ascii_equalarea\bathy_30s.tiff"
