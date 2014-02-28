@echo off
rem Set root OSGEO4W home dir
set OSGEO4W_ROOT=C:\OSGeo4W
rem Convert double backslashes to single
set OSGEO4W_ROOT=%OSGEO4W_ROOT:\\=\%
echo. & echo OSGEO4W home is %OSGEO4W_ROOT% & echo.

set PATH=%OSGEO4W_ROOT%\bin;%PATH%

rem Add application-specific environment settings
for %%f in ("%OSGEO4W_ROOT%\etc\ini\*.bat") do call "%%f"

cd /D D:\a\data\marspec
@echo on

rem Create virtual file with nodata set to nothing
gdalbuildvrt -srcnodata None -vrtnodata None processed\bathy_5m_no_null.vrt MARSPEC_5m\ascii\bathy_5m.asc
rem set value to 1 when value is nodata (avoid setting near-shore BIO-Oracle cells to NoData) or value is bigger then -250
gdal_calc.py -A processed\bathy_5m_no_null.vrt --calc="numpy.logical_or((A == -32768),( A >= -250))" --outfile=processed\bathy_5m_mask_250.tif --NoDataValue=-9999