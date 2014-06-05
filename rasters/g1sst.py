from datetime import date
from pydap.client import open_url
import numpy
import numpy as N

def explore():

    d = open_url('http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc')

    ##v = d["SST"]
    ##
    ##v["lon"]
    ##
    ##v["SST"].shape
    ##
    ##s = v["SST"]
    ##
    ##print 
    ##(1, 16000, 36000)

    ##a = d.SST[0,12000:13000,18000:19000] # time:0, lat: 40-50N, lon 0-10E

    sst = d['SST']

    ##grid = sst[0:1:0,0:1:10,0:1:3]

    ##grid = sst[0:1:0,100:1:110,0:1:3]
    # generated http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc.dods?SST.SST[0:1:0][100:110:0][0:3:0]
    # which is not correct, it should be:
    # http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc.ascii?SST[0:1:0][0:1:10][0:1:3]

    ##grid = sst.SST[d.time==0, (d.lat >= 100) & (d.lat <= 110), (d.lon >= 0) & (d.lon <= 3)]



    from pydap.client import open_dods

    d2 = open_dods('http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc.dods?SST[0:1:0][0:1:10][0:1:3]')

    ##http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc.ascii?SST[0:1:0][1000:1:1010][1000:1:1003]

    #d = open_url('http://coastwatch.pfeg.noaa.gov/erddap/griddap/erdTAssh1day')
    ##start = end = datetime(2010, 5, 19, 12, 0, 0)
    ##ssh = d.ssh.ssh[
    ##        (d.time >= coards.format(start, d.time.units)) & (d.time <= coards.format(end, d.time.units)),
    ##        (d.altitude == 0),
    ##        (d.latitude >= 17) & (d.latitude <= 32),
    ##        (d.longitude >= 260) & (d.longitude <= 281)]


    #http://ourocean.jpl.nasa.gov:8080/thredds/dodsC/g1sst/sst_20130526.nc.ascii?SST[0:1:0][100:1:110][0:1:3]

    suffix = "2013/147/20130527-JPL_OUROCEAN-L4UHfnd-GLOB-v01-fv01_0-G1SST.nc.bz2"
    d = open_url("http://podaac-opendap.jpl.nasa.gov/opendap/allData/ghrsst/data/L4/GLOB/JPL_OUROCEAN/G1SST/" + suffix)



    ##g = d.analysed_sst[0, 12000:12010, 18000:18010]

    g.lat.data
    g.lon.data
    t = g.analysed_sst.data

    ##lats = d.lat[1200:1210]
    ##lons = d.lon[18000:18010]

def degree_to_index(degree, name, maxvalue):
    if degree > maxvalue or degree < -maxvalue:
        raise ValueError("%s should be between -%d and %d" % (name, maxvalue, maxvalue))
    return int(round((degree+maxvalue) * 100))

def get_lon(degree):
    return degree_to_index(degree, "Longitude", 180)

def get_lat(degree):
    return degree_to_index(degree, "Latitude", 80)


def get_sst(day, minlat, maxlat, minlon, maxlon):
    sst, lat, lon = get_sst_with_coor(day, minlat, maxlat, minlon, maxlon)
    return sst

def get_sst_with_coor(day, minlat, maxlat, minlon, maxlon):
    suffix = "-JPL_OUROCEAN-L4UHfnd-GLOB-v01-fv01_0-G1SST.nc.bz2"
    daynumber = day.timetuple().tm_yday
    dayformatted = day.strftime("%Y%m%d")
    suffix = str(day.year) + ("/%03d" % daynumber) + "/" + day.strftime("%Y%m%d") + suffix
    url = "http://podaac-opendap.jpl.nasa.gov/opendap/allData/ghrsst/data/L4/GLOB/JPL_OUROCEAN/G1SST/" + suffix
    d = open_url(url)
    minlat = get_lat(minlat)
    maxlat = get_lat(maxlat)
    minlon = get_lon(minlon)
    maxlon = get_lon(maxlon)
    print url + " [%d:%d, %d:%d]" % (minlat,maxlat, minlon,maxlon)
    g = d.analysed_sst[0,minlat:maxlat, minlon:maxlon]
    sst = g.analysed_sst.data
    sst_masked = numpy.ma.masked_values(sst, -32768)
    
    return sst_masked, g.lat, g.lon ##masked numpy array

def point_distance(p1, p2):
    import math
    d = math.sqrt((p1[0]-p2[0])**2 + (p1[1]-p2[1])**2)
    return d

def get_edge_temp_with_lat_lon(sst, lats, lons, edge_lat, edge_lon):
    edge = (edge_lat, edge_lon)
    rows = xrange(sst.shape[1])
    cols = xrange(sst.shape[2])

    result = (None, None, None)
    dist = 10**10
    for i in rows:
        m = sst.mask[0][i]
        if False in m: ## not masked
            for j in cols:
                if not m[j]: ## not masked
                    temp, lat, lon = sst.data[0][i][j], lats.data[i], lons.data[j]
                    newdist = point_distance(edge, (lat,lon))
                    if newdist < dist:
                        dist = newdist
                        result = temp, lat, lon
    return result

def scale_temp(sst):
    return sst * 0.01 ## rescale to real temperature values

def get_days(start=date(2012,04,1), end= date(2013,05,1), day=15):
    ## http://stackoverflow.com/a/4040204/477367
    dt1 = start ## start
    dt2 = end ## end
    start_month=dt1.month
    end_months=(dt2.year-dt1.year)*12 + dt2.month+1
    dates=[date(year=yr, month=mn, day=day) for (yr, mn) in (
          ((m - 1) / 12 + dt1.year, (m - 1) % 12 + 1) for m in range(start_month, end_months)
      )]
    return dates

def print_avg_temp(days, minlat, maxlat, minlon, maxlon):
    for day in days:
        sst = get_sst(day, minlat, maxlat, minlon, maxlon)
        avg = float(scale_temp(sst.mean()))
        print "{}{:10.2f}".format(str(day), avg)

def export_csv(days, minlat, maxlat, minlon, maxlon, points, name='output.csv', image=True):
    with open(name, 'w') as f:
        f.write('point;day;temp;lat;lon\n')
        for day in days:
            sst, lats, lons = get_sst_with_coor(day, minlat, maxlat, minlon, maxlon)
            for index, point in enumerate(points):
                temp, lat, lon = get_edge_temp_with_lat_lon(sst, lats, lons, point[0], point[1])
                f.write('{};{};{:.2f};{:.3f};{:.3f}\n'.format(str(index),str(day), scale_temp(temp), lat,lon))
            if image:
                save_img(sst, name="output_"+str(day)+".png")

def save_img(sst, name="test.png"):
    import scipy.misc

##    arr = sst[0][::-1] ## take first element in array and then reverse (otherwise the world is upside down
##    arr = arr + 10
##    img = scipy.misc.toimage(arr, high=40, low=-10, mode='F')
##    img.save(name)
##    scipy.misc.imsave(name, arr)

    s = sst
    ## colormap
    m = N.empty((256,3), dtype=N.uint8) 
    m[:,0] = [255]*256
    m[:,1] = N.arange(0,256,dtype=N.uint8)[::-1]
    m[:,2] = [0]*256
    m[0,:] = [0,0,0] ## set to 0 zero for first value (NAN)

    arr = sst[0][::-1] ## take first element in array and then reverse (otherwise the world is upside down
    img = scipy.misc.toimage(arr, high=255, low=0,cmax=s.max(), cmin=s.min(), pal=m, mode='L')
    img.save(name)

import unittest
class TestAll(unittest.TestCase):

    def test_get_lat(self):
        self.assertRaises(ValueError, get_lat, (-80.1))
        self.assertRaises(ValueError, get_lat, (80.1))
        self.assertEqual(get_lat(-80), 0)
        self.assertEqual(get_lat(0), 8000)
        self.assertEqual(get_lat(40), 12000)
        self.assertEqual(get_lat(80), 16000)

    def test_get_lon(self):
        self.assertRaises(ValueError, get_lon, (-180.1))
        self.assertRaises(ValueError, get_lon, (180.1))
        self.assertEqual(get_lon(-180), 0)
        self.assertEqual(get_lon(0), 18000)
        self.assertEqual(get_lon(40), 22000)
        self.assertEqual(get_lon(180), 36000)

    def test_get_sst(self):
##        get_sst(date(2013, 5, 26), 0,0.01, 0,0.01)
##        get_sst(date(2012, 5, 26), 0,0.01, 0,0.01)
##        get_sst(date(2011, 5, 26), 0,0.01, 0,0.01)      
        pass

def get_fsteen_data():
    ## point 1: 43.3286 5.0967
    days = [
        date(2012, 5, 28),
        date(2012, 6, 19),
        date(2012, 7, 10),
        date(2012, 8, 22),
        date(2012, 9, 15),
        date(2012, 10, 25),
        date(2012, 11, 20),
        date(2012, 12, 15),
        date(2013, 1, 30),
        date(2013, 2, 19),
        date(2013, 3, 15),
        date(2013, 4, 15)]
    export_csv(days, minlat=43.26, maxlat=43.35, minlon=5.06, maxlon=5.13, name="output point1 43.3286 5.0967.csv", points=[(43.3286, 5.0967)])
    ## point 2: 43.2096 5.4205 
    days = [
        date(2012, 4, 18),
        date(2012, 5, 23),
        date(2012, 6, 20),
        date(2012, 7, 11),
        date(2012, 8, 24),
        date(2012, 9, 15),
        date(2012, 10, 26),
        date(2012, 11, 22),
        date(2012, 12, 4),
        date(2013, 1, 22),
        date(2013, 2, 20),
        date(2013, 3, 27),
        date(2013, 4, 16)]
    export_csv(days, minlat=43.14, maxlat=43.23, minlon=5.39, maxlon=5.45, name="output point2 43.2096 5.4205.csv", points=[(43.2096, 5.4205)])

if __name__ == '__main__':
    unittest.main()
    
##suffix = "2013/147/20130527-JPL_OUROCEAN-L4UHfnd-GLOB-v01-fv01_0-G1SST.nc.bz2"
##d = open_url("http://podaac-opendap.jpl.nasa.gov/opendap/allData/ghrsst/data/L4/GLOB/JPL_OUROCEAN/G1SST/" + suffix)
    
##s = get_sst(date(2013,5,26), 35,45, -10,10)
##save_img(s)
