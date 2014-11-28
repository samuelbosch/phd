library(raster)

wd <- getwd()
setwd("D:/a/data/BioOracle_GLOBAL_RV")
asc <- list.files(pattern="[.]asc$")
for (f in asc) {
    r <- raster(f)
    writeRaster(r, gsub("[.]asc$", ".grd", f))
}
setwd(wd)
