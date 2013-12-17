"""
TODO:

- track progress instead of starting a new
- sleep between retries
- fine tune the thread pool size

"""

import subprocess
from collections import deque
from multiprocessing import Pool

counts = deque([])

def executequeries(id):
    try:
        print "start execute [%s]" % id
        result = subprocess.check_output(["psql", "-d", "obis", "-c", "SELECT qc.execute_queries(Array["+id+"])"])
        c = result.split('\n')[2:-3][0].strip()
        if c.isdigit():
            counts.append(int(c))
        print "processed %s rows [total: %i]" % (c, sum(counts))
        return -1
    except:
        print "failed [%s] will retry later" % id
        return id # for re scheduling

def run(p, ids):
    ids = [x for x in ids if x > 0]
    if len(ids) > 0:
        print "start run with %i ids" % len(ids)
        return p.map(executequeries, ids)
    else: 
        return []

if __name__ == '__main__':
    ids = subprocess.check_output(["psql", "-d", "obis", "-c", "SELECT r.imis_dasid FROM obis.resources r"])
    ids = [x.strip() for x in ids.split('\n')[2:-3]]

    p = Pool(3)
    failed_ids = run(p,ids)
    # retry 5 times
    failed_ids = run(p,failed_ids)
    failed_ids = run(p,failed_ids)
    failed_ids = run(p,failed_ids)
    failed_ids = run(p,failed_ids)
    failed_ids = run(p,failed_ids)