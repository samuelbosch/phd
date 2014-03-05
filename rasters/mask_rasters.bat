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
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\alk_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\alk_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\alk_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\alk_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\alk_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\calcite_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\calcite_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\calcite_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\calcite_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\calcite_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\chlo_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\chlo_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\chlo_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\chlo_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_range.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_range.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\chlo_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\CloudFr_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\CloudFr_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\CloudFr_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\CloudFr_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\da_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\da_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\da_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\da_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\dissox_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\dissox_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\dissox_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\dissox_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\dissox_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\eudepth_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\eudepth_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\eudepth_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\eudepth_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\eudepth_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\nitrate_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\nitrate_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\nitrate_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\nitrate_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\nitrate_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\PAR3_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\PAR3_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\PAR3_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\PAR3_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\PAR3_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\par_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\par_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\par_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\par_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\phos_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\phos_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\phos_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\phos_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\phos_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\ph_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\ph_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\ph_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\ph_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\ph_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sal_mean_an.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_an.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_an.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_an.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_an.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sal_mean_au.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_au.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_au.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_au.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_au.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sal_mean_sp.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_sp.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_sp.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_sp.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_sp.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sal_mean_su.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_su.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_su.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_su.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_su.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sal_mean_wi.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_wi.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_wi.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_wi.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sal_mean_wi.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\silica_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\silica_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\silica_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\silica_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\silica_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\SolIns_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\SolIns_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\SolIns_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\SolIns_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sst_max.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_max.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_max.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_max.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_max.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sst_mean.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_mean.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_mean.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_mean.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_mean.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sst_min.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_min.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_min.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_min.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_min.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\sst_range.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_range.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_range.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_range.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\sst_range.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\BioOracle_GLOBAL_RV\waveheight.asc" --calc="A*B" --outfile="D:\a\data\BioOracle_GLOBAL_RV_30s_min250\waveheight.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\waveheight.tif" "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\waveheight.asc"
del "D:\a\data\BioOracle_GLOBAL_RV_30s_min250\waveheight.tif"
