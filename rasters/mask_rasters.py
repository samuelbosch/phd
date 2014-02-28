import os

mask = r'D:\a\data\marspec\processed\bathy_5m_mask_250.tif'
source = r'D:\a\data\BioOracle_GLOBAL_RV'
destination = r'D:\a\data\BioOracle_GLOBAL_RV_min250'
with open('mask_rasters.bat', 'w') as bat:
    ## load OSGEO4W
    bat.write(r"""@echo off
rem Root OSGEO4W home dir
set OSGEO4W_ROOT=C:\OSGeo4W
rem Convert double backslashes to single
set OSGEO4W_ROOT=%OSGEO4W_ROOT:\\=\%
echo. & echo OSGEO4W home is %OSGEO4W_ROOT% & echo.

set PATH=%OSGEO4W_ROOT%\bin;%PATH%

rem Add application-specific environment settings
for %%f in ("%OSGEO4W_ROOT%\etc\ini\*.bat") do call "%%f"
""")
    bat.write('cd ' + os.getcwd() + '\n')
    for f in os.listdir(source)[:2]:
        if f.endswith('.asc'):
            infile = os.path.join(source, f)
            temptif = os.path.join(destination, f.replace('.asc', '.tif'))
            outfile = os.path.join(destination, f)
            bat.write('gdal_calc.py -A "%s" -B "%s" --calc="A*B" --outfile="%s" --NoDataValue=-9999\n' % (mask, infile, temptif))
            bat.write('gdal_translate -of AAIGrid "%s" "%s"\n' % (temptif,outfile))
            bat.write('del "%s"\n' % temptif)
            
