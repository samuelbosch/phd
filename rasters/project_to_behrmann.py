"""
Small script to generate gdal_warp commands
for projecting rasters to the Behrmann projection
to be able to run the generated bat file you should have gdalwarp in your path or run it from an OSGeo4W Shell
"""

import os

root = r"D:\a\data\marspec\MARSPEC_10m\ascii"
output = os.path.abspath(os.path.join(root, r'..\ascii_equalarea'))


def create_bat():
    proj = "+proj=cea +lon_0=0 +lat_ts=30 +x_0=0 +y_0=0 +datum=WGS84 +ellps=WGS84 +units=m +no_defs"
    with open('project_to_behrmann.bat', 'w') as bat:
        for r, dirs, files in os.walk(root):
            for f in files:
                n, ext = os.path.splitext(f)
                if ext == '.asc':
                    ## output of ascii files from gdalwarp is not supported
                    temptiff = os.path.join(output, n + '.tiff')
                    bat.write('gdalwarp -of GTiff -multi -t_srs "%s" "%s" "%s"\n' % (proj, os.path.join(r, f), temptiff))
                    ## convert output tiff to ascii
                    bat.write('gdal_translate -of AAIGrid "%s" "%s"\n' % (temptiff, os.path.join(output,f)))
                    ## delete temp file
                    bat.write('del "%s"\n'%temptiff)


if __name__ == '__main__':
    create_bat()
