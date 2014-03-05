import gzip

max_depth = -250

header = """ncols        4320
nrows        2160
xllcorner    -180.000000000000
yllcorner    -90.000000000000
cellsize     0.083333333333
NODATA_value -32768
"""
bathy = r'D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc.gz'
output = r'D:\a\data\marspec\processed\bathy_5m_mask_30s_250.asc'

def get_new_row_values(b):
    new_row = ['-32768'] * 4320
    for z in range(10):
        l = b.readline()
        l = l.strip('\n ').split()
        for i, v in enumerate(l):
            if int(v) >= max_depth:
                new_row[i/10] = '1'
    return new_row

with open(output, 'w') as out:
    out.write(header)
    with gzip.open(bathy) as b:
        for i in range(6):
            b.readline() ## skip header
        for i in range(2160):
            new = get_new_row_values(b)
            out.write(' '.join(map(str,new)) + '\n')
