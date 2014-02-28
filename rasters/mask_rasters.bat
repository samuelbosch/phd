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
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2100\salinity.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2100\salinity.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2100\salinity.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2100\salinity.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2100\salinity.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2100\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_max.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_max.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2100\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_mean.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_mean.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2100\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_min.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_min.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2100\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_range.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_range.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2100\sst_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2200\salinity.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2200\salinity.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2200\salinity.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2200\salinity.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2200\salinity.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2200\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_max.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_max.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2200\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_mean.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_mean.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2200\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_min.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_min.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A1B\2200\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_range.tif" "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_range.asc"
del "D:\a\data\BioOracle_scenarios_min250\A1B\2200\sst_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A2\2100\salinity.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A2\2100\salinity.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A2\2100\salinity.tif" "D:\a\data\BioOracle_scenarios_min250\A2\2100\salinity.asc"
del "D:\a\data\BioOracle_scenarios_min250\A2\2100\salinity.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A2\2100\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_max.tif" "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_max.asc"
del "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A2\2100\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_mean.tif" "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_mean.asc"
del "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A2\2100\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_min.tif" "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_min.asc"
del "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\A2\2100\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_range.tif" "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_range.asc"
del "D:\a\data\BioOracle_scenarios_min250\A2\2100\sst_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2100\salinity.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2100\salinity.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2100\salinity.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2100\salinity.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2100\salinity.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2100\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_max.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_max.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2100\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_mean.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_mean.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2100\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_min.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_min.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2100\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_range.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_range.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2100\sst_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2200\salinity.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2200\salinity.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2200\salinity.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2200\salinity.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2200\salinity.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2200\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_max.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_max.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2200\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_mean.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_mean.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2200\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_min.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_min.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_250.tif" -B "D:\a\data\BioOracle_scenarios\B1\2200\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_range.tif" "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_range.asc"
del "D:\a\data\BioOracle_scenarios_min250\B1\2200\sst_range.tif"
