# Some experimentations with the biomod2 package in combination with 
# species data from obis and marspec data
# 
# Author: Samuel Bosch
###############################################################################

require("biomod2")
require("raster")

root = "Google Drive\\predictors\\1.tryout"

fucus = read.csv(paste(root, "2.processed\\fucus_serratus_obis_20130530_142854.csv", sep="\\"))

coord = fucus[c('longitude', 'latitude')] ## coordinates for fucus serratus occurrence records

points = SpatialPoints(coord)

marspec.dir = "data\\marspec\\MARSPEC_10m\\ascii"

marspec.vars = list.files(path=marspec.dir, pattern="\\.asc$", full.names=TRUE)

#env = stack(marspec.vars)

env = stack (paste(marspec.dir, "bathy_10m.asc", sep="/"), paste(marspec.dir, "biogeo13_sst_mean_10m.asc", sep="/"))

respName = "fucus.serratus"

biomod.data = BIOMOD_FormatingData(resp.var = points,
                                   expl.var = env, # explanatory variable(s)
                                   resp.name =respName,
                                   resp.xy=coord, # locations
                                   PA.nb.rep = 1, # number of pseudo absence samplings
                                   PA.nb.absences = 10000, # 10000 pseudo absence locations
                                   PA.strategy = "random", # must be  ‘random’, ‘sre’, ‘disk’ or ‘user.defined’
                                   na.rm = TRUE) # remove points with missing environmental data 

# TODO review the maxent options
biomod.options = BIOMOD_ModelingOptions(
  MAXENT = list(path_to_maxent.jar = "D:/a/prog/maxent",
                maximumiterations = 200,
                visible=FALSE,
                linear=TRUE,
                quadratic = TRUE,
                product = TRUE,
                threshold = TRUE,
                hinge = TRUE,
                lq2lqptthreshold = 80,
                l2lqthreshold = 10,
                hingethreshold = 15,
                beta_threshold = -1,
                beta_categorical = -1,
                beta_lqp = -1,
                beta_hinge = -1,
                defaultprevalence = 0.5
    ))

wd = getwd()
setwd("D:/a/Google Drive/predictors/1.tryout/3.output")

modelId = paste("fucus_Serratus",format(Sys.time(), format="%Y%m%d_%H%M%S"),sep="_")

biomod.model = BIOMOD_Modeling(
            data=biomod.data,
            models=c('RF', 'MAXENT'),
            models.options = biomod.options,
            NbRunEval=1, 
            DataSplit=50, 
            Prevalence=0.5, 
            VarImport=3, 
            models.eval.meth = c('TSS','ROC'), 
            SaveObj = TRUE, 
            rescal.all.models = TRUE, 
            do.full.models = FALSE, 
            modeling.id = modelId
        )

## get model evaluations
capture.output(getModelsEvaluations(biomod.model), 
        file=file.path(respName, paste(modelId,"_formal_models_evaluation.txt", sep="")))
## get variable importance
capture.output(getModelsVarImport(biomod.model), 
        file=file.path(respName, paste(modelId,"_formal_models_variables_importance.txt", sep="")))

## Make projections on current variable 

biomod.proj = BIOMOD_Projection( 
        modeling.output = biomod.model,
        new.env = env, 
        proj.name = 'current', 
        selected.models = 'all', 
        binary.meth = 'TSS', 
        compress = "gzip", 
        build.clamping.mask = FALSE, 
        keep.in.memory = FALSE,
        output.format = '.img')

setwd(wd)