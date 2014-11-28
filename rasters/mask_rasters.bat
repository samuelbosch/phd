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
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\bathy_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\bathy_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\bathy_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\bathy_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\bathy_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo01_aspect_EW_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo01_aspect_EW_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo01_aspect_EW_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo01_aspect_EW_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo01_aspect_EW_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo02_aspect_NS_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo02_aspect_NS_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo02_aspect_NS_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo02_aspect_NS_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo02_aspect_NS_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo03_plan_curvature_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo03_plan_curvature_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo03_plan_curvature_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo03_plan_curvature_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo03_plan_curvature_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo04_profile_curvature_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo04_profile_curvature_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo04_profile_curvature_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo04_profile_curvature_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo04_profile_curvature_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo05_dist_shore_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo05_dist_shore_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo05_dist_shore_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo05_dist_shore_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo05_dist_shore_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo06_bathy_slope_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo06_bathy_slope_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo06_bathy_slope_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo06_bathy_slope_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo06_bathy_slope_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo07_concavity_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo07_concavity_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo07_concavity_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo07_concavity_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo07_concavity_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo08_sss_mean_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo08_sss_mean_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo08_sss_mean_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo08_sss_mean_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo08_sss_mean_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo09_sss_min_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo09_sss_min_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo09_sss_min_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo09_sss_min_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo09_sss_min_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo10_sss_max_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo10_sss_max_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo10_sss_max_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo10_sss_max_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo10_sss_max_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo11_sss_range_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo11_sss_range_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo11_sss_range_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo11_sss_range_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo11_sss_range_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo12_sss_variance_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo12_sss_variance_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo12_sss_variance_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo12_sss_variance_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo12_sss_variance_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo13_sst_mean_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo13_sst_mean_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo13_sst_mean_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo13_sst_mean_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo13_sst_mean_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo14_sst_min_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo14_sst_min_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo14_sst_min_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo14_sst_min_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo14_sst_min_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo15_sst_max_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo15_sst_max_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo15_sst_max_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo15_sst_max_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo15_sst_max_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo16_sst_range_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo16_sst_range_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo16_sst_range_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo16_sst_range_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo16_sst_range_5m.tif"
gdal_calc.py -A "D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc" -B "D:\a\data\marspec\MARSPEC_5m\ascii\biogeo17_sst_variance_5m.asc" --calc="A*B" --outfile="D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo17_sst_variance_5m.tif" --NoDataValue=-9999
gdal_translate -of AAIGrid "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo17_sst_variance_5m.tif" "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo17_sst_variance_5m.asc"
del "D:\a\data\marspec\MARSPEC_5m\ascii_30s_min250\biogeo17_sst_variance_5m.tif"
