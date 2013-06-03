import csv, os

inputd=r'..\..\predictors\1.tryout\1.input'
outputd=r'..\..\predictors\1.tryout\2.processed'

def get_species(fname='fucus_serratus_obis_20130530_142854.csv'):
    with open(os.path.join(inputd, fname), 'rb') as f:
        r = csv.reader(f, delimiter=',', quotechar='"')
        data = list(r)
    return data[0], data[1:]

def subset(arr, indices):
    return [arr[i] for i in indices]

def filter_columns(header, data, keepcolumns=['sname', 'longitude', 'latitude']):
    """ return a header and data with only the columns you want to keep """
    indices = [header.index(column) for column in keepcolumns]
    h = subset(header, indices)
    data = (subset(row, indices) for row in data)
    return h, data

def save_species(header, data, fname='fucus_serratus_obis_20130530_142854.csv', delimiter=',', quotechar='"'):
    with open(os.path.join(outputd, fname), 'wb') as f:
        w = csv.writer(f, quotechar='"')
        w.writerow(header)
        [w.writerow(row) for row in data]

def transform_species():
    h, d = get_species()
    h, d = filter_columns(h, d)
    h[h.index('sname')] = 'species' ## rename to species
    save_species(h, d)

import unittest
class Test(unittest.TestCase):

    def test_subset(self):
        self.assertEqual(subset([1,2,3], [0,2]), [1,3])

if __name__ == '__main__':
    unittest.main()
