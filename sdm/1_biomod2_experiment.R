# Some experimentations with the biomod2 package in combination with 
# species data from obis and marspec data
# 
# Author: Samuel Bosch
###############################################################################

library(biomod2)
require('raster')

root = "Google Drive\\predictors\\1.tryout"

fucus = read.csv(paste(root, "2.processed\\fucus_serratus_obis_20130530_142854.csv", sep="\\"))

coord = fucus[c('longitude', 'latitude')] ## coordinates for fucus serratus occurrence records

marspec.dir = "data\\marspec\\MARSPEC_10m\\ascii"

marspec.vars = list.files(path=marspec.dir, pattern="\\.asc$", full.names=TRUE)

env = stack(marspec.vars)

biomod.data = BIOMOD_FormatingData(resp.var = rep(1, nrow(coord)), ## not entirely sure if this is needed
                                   expl.var = env, # explanatory variable(s)
                                   resp.xy = coord,
                                   resp.name ="fucus_serratus",
                                   PA.nb.rep = 10, # select pseudo absence locations 10 times
                                   PA.nb.absences = 10000, # 10000 pseudo absence locations
                                   PA.strategy = "random", # must be  ‘random’, ‘sre’, ‘disk’ or ‘user.defined’
                                   na.rm = TRUE) # remove points with missing environmental data 
