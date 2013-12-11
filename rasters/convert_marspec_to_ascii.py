"""
Small script to generate gdal commands for the conversion
of marspec data to ASCII (for usage with e.g. Maxent)
Output file can then be executed with e.g. OSGeo4w or directly if you have gdal_translate in your path

additionally you can rename the grids to some more clear names
"""

import os

root = r"D:\a\data\marspec\MARSPEC_30s\Temperature_30s"
output = os.path.join(root, 'ascii')

def create_bat():
    with open('convert_marspec_to_ascii.bat', 'w') as bat:
        for r, dirs, files in os.walk(root):
            contains_adf = reduce(lambda b, p: b or os.path.splitext(p)[1] == '.adf', files, False)
            if contains_adf:
                dirname = os.path.split(r)[1]
                bat.write('gdal_translate -of AAIGrid %s %s.asc\n' % (r, os.path.join(output,dirname)))

def rename_biogeo_grids():
    """ rename e.g. biogeo01... to biogeo01_aspect_EW... """
    d = { '01': 'aspect_EW',
          '02': 'aspect_NS',
          '03': 'plan_curvature',
          '04': 'profile_curvature',
          '05': 'dist_shore',
          '06': 'bathy_slope',
          '07': 'concavity',
          '08': 'sss_mean',
          '09': 'sss_min',
          '10': 'sss_max',
          '11': 'sss_range',
          '12': 'sss_variance',
          '13': 'sst_mean',
          '14': 'sst_min',
          '15': 'sst_max',
          '16': 'sst_range',
          '17': 'sst_variance' }
    for f in os.listdir(output):
        if f.startswith('biogeo'):
            number = f[6:8] ## right after biogeo
            extrainfo = d[number]
            if f.find(extrainfo) < 0:
                old = os.path.join(output, f)
                new = os.path.join(output, 'biogeo' + number + '_' + extrainfo + f[8:])
                os.rename(old, new)
            
                        

if __name__ == '__main__':
    create_bat()
    ## rename_biogeo_grids
