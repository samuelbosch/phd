import os

root= r'D:\a\data\marspec\MARSPEC_10m\ascii'
inr = os.path.join(root, 'bathy_10m.asc')
outr = os.path.join(root, 'bathy_10m_plus_300.asc')

def calc(x, nodata):
    if x == nodata:
        return x
    elif x >= -300:
        return 1
    else:
        return 0
    

def notempty(x):
    return x != ''

with open(inr, 'r') as f:
    with open(outr, 'w') as r:
        ## read header and write to output
        header = [f.readline() for i in xrange(6)]
        for line in header:
            r.write(line)
        ## parse header
        header = dict([filter(notempty, x.strip().split(' ')) for x in header])
        nodata = int(header['NODATA_value'])

        for i in xrange(int(header['nrows'])):
            line = f.readline()
            calcvalue = ' '.join([str(calc(int(x), nodata)) for x in line.strip().split(' ')])
            r.write(calcvalue + '\n')
