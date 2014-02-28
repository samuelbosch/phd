@echo off
rem Root OSGEO4W home dir
set OSGEO4W_ROOT=C:\OSGeo4W
rem Convert double backslashes to single
set OSGEO4W_ROOT=%OSGEO4W_ROOT:\\=\%
echo. & echo OSGEO4W home is %OSGEO4W_ROOT% & echo.

set PATH=%OSGEO4W_ROOT%\bin;%PATH%

rem Add application-specific environment settings
for %%f in ("%OSGEO4W_ROOT%\etc\ini\*.bat") do call "%%f"
cd D:\a\Google Drive\code\rasters
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_GLOBAL_RV\alk_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_min250\alk_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_min250\alk_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_min250\alk_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_min250\alk_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_GLOBAL_RV\calcite_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_min250\calcite_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_min250\calcite_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_min250\calcite_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_min250\calcite_mean.tif"
