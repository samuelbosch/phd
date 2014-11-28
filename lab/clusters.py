
genes = {}

with open('Assembly_cdhit') as fasta:
    lines = fasta.readlines()
    gene = None
    for line in lines:
        if line.startswith(">"):
            gene = line.split()[0].replace('>', '')
            genes[gene] = [line]
        else:
            genes[gene].append(line)

with open('Clusters') as clusters:
    for index, cluster in enumerate(clusters.readlines()):
        gene_cluster = cluster.strip('\n').split()
        if len(gene_cluster) > 1:
            with open("assembly_cluster_%s.fasta" % index, 'w') as f:
                for gene in gene_cluster:
                    f.writelines(genes[gene])

import os

with open('mafft_cluster.sh', 'w') as sh:
    for f in os.listdir('.'):
        if f.startswith('assembly_cluster'):
            sh.write('mafft %s bla bla bla\n' % f)
