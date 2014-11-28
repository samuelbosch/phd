#library("phyloclim")

input_dir <- "D:\\temp\\dictyota" #"C:\\Users\\UGent\\Documents\\biologie\\3 BA BIO\\bachelorproef\\deel 1\\dictyota.final\\model\\Dictyota_asc"
output_file <- "D:\\temp\\dictyota\\D_DI.txt" #"C:\\Users\\UGent\\Documents\\biologie\\3 BA BIO\\bachelorproef\\deel 1\\dictyota_final\\D_DI.txt"

D.humifusa_0 <- paste(input_dir, "Dictyota_humifusa_0.asc", sep="\\")
D.humifusa_1 <- paste(input_dir, "Dictyota_humifusa_1.asc", sep="\\")
D.humifusa_2 <- paste(input_dir, "Dictyota_humifusa_2.asc", sep="\\")
D.humifusa_3 <- paste(input_dir, "Dictyota_humifusa_3.asc", sep="\\")
D.humifusa_4 <- paste(input_dir, "Dictyota_humifusa_4.asc", sep="\\")

D.dichotoma1_0 <- paste(input_dir, "Dictyota_dichotoma1_0.asc", sep="\\")
D.dichotoma1_1 <- paste(input_dir, "Dictyota_dichotoma1_1.asc", sep="\\")
D.dichotoma1_2 <- paste(input_dir, "Dictyota_dichotoma1_2.asc", sep="\\")
D.dichotoma1_3 <- paste(input_dir, "Dictyota_dichotoma1_3.asc", sep="\\")
D.dichotoma1_4 <- paste(input_dir, "Dictyota_dichotoma1_4.asc", sep="\\")

D.bartayresiana_0 <- paste(input_dir, "Dictyota_bartayresiana_0.asc", sep="\\")
D.bartayresiana_1 <- paste(input_dir, "Dictyota_bartayresiana_1.asc", sep="\\")
D.bartayresiana_2 <- paste(input_dir, "Dictyota_bartayresiana_2.asc", sep="\\")
D.bartayresiana_3 <- paste(input_dir, "Dictyota_bartayresiana_3.asc", sep="\\")
D.bartayresiana_4 <- paste(input_dir, "Dictyota_bartayresiana_4.asc", sep="\\")

D.binghamiae_0 <- paste(input_dir, "Dictyota_binghamiae_0.asc", sep="\\")
D.binghamiae_1 <- paste(input_dir, "Dictyota_binghamiae_1.asc", sep="\\")
D.binghamiae_2 <- paste(input_dir, "Dictyota_binghamiae_2.asc", sep="\\")
D.binghamiae_3 <- paste(input_dir, "Dictyota_binghamiae_3.asc", sep="\\")
D.binghamiae_4 <- paste(input_dir, "Dictyota_binghamiae_4.asc", sep="\\")

D.adnata_0 <- paste(input_dir, "Dictyota_adnata_0.asc", sep="\\")
D.adnata_1 <- paste(input_dir, "Dictyota_adnata_1.asc", sep="\\")
D.adnata_2 <- paste(input_dir, "Dictyota_adnata_2.asc", sep="\\")
D.adnata_3 <- paste(input_dir, "Dictyota_adnata_3.asc", sep="\\")
D.adnata_4 <- paste(input_dir, "Dictyota_adnata_4.asc", sep="\\")

D.spiralis_0 <- paste(input_dir, "Dictyota_spiralis_0.asc", sep="\\")
D.spiralis_1 <- paste(input_dir, "Dictyota_spiralis_1.asc", sep="\\")
D.spiralis_2 <- paste(input_dir, "Dictyota_spiralis_2.asc", sep="\\")
D.spiralis_3 <- paste(input_dir, "Dictyota_spiralis_3.asc", sep="\\")
D.spiralis_4 <- paste(input_dir, "Dictyota_spiralis_4.asc", sep="\\")

D.crenulata_0 <- paste(input_dir, "Dictyota_crenulata_0.asc", sep="\\")
D.crenulata_1 <- paste(input_dir, "Dictyota_crenulata_1.asc", sep="\\")
D.crenulata_2 <- paste(input_dir, "Dictyota_crenulata_2.asc", sep="\\")
D.crenulata_3 <- paste(input_dir, "Dictyota_crenulata_3.asc", sep="\\")
D.crenulata_4 <- paste(input_dir, "Dictyota_crenulata_4.asc", sep="\\")

D.implexa_0 <- paste(input_dir, "Dictyota_implexa_0.asc", sep="\\")
D.implexa_1 <- paste(input_dir, "Dictyota_implexa_1.asc", sep="\\")
D.implexa_2 <- paste(input_dir, "Dictyota_implexa_2.asc", sep="\\")
D.implexa_3 <- paste(input_dir, "Dictyota_implexa_3.asc", sep="\\")
D.implexa_4 <- paste(input_dir, "Dictyota_implexa_4.asc", sep="\\")

D.ceylanica5_0 <- paste(input_dir, "Dictyota_ceylanica5_0.asc", sep="\\")
D.ceylanica5_1 <- paste(input_dir, "Dictyota_ceylanica5_1.asc", sep="\\")
D.ceylanica5_2 <- paste(input_dir, "Dictyota_ceylanica5_2.asc", sep="\\")
D.ceylanica5_3 <- paste(input_dir, "Dictyota_ceylanica5_3.asc", sep="\\")
D.ceylanica5_4 <- paste(input_dir, "Dictyota_ceylanica5_4.asc", sep="\\")

D.fasciola_0 <- paste(input_dir, "Dictyota_fasciola_0.asc", sep="\\")
D.fasciola_1 <- paste(input_dir, "Dictyota_fasciola_1.asc", sep="\\")
D.fasciola_2 <- paste(input_dir, "Dictyota_fasciola_2.asc", sep="\\")
D.fasciola_3 <- paste(input_dir, "Dictyota_fasciola_3.asc", sep="\\")
D.fasciola_4 <- paste(input_dir, "Dictyota_fasciola_4.asc", sep="\\")

D.mediterranea_0 <- paste(input_dir, "Dictyota_mediterranea_0.asc", sep="\\")
D.mediterranea_1 <- paste(input_dir, "Dictyota_mediterranea_1.asc", sep="\\")
D.mediterranea_2 <- paste(input_dir, "Dictyota_mediterranea_2.asc", sep="\\")
D.mediterranea_3 <- paste(input_dir, "Dictyota_mediterranea_3.asc", sep="\\")
D.mediterranea_4 <- paste(input_dir, "Dictyota_mediterranea_4.asc", sep="\\")

D.friabilis1_0 <- paste(input_dir, "Dictyota_friabilis1_0.asc", sep="\\")
D.friabilis1_1 <- paste(input_dir, "Dictyota_friabilis1_1.asc", sep="\\")
D.friabilis1_2 <- paste(input_dir, "Dictyota_friabilis1_2.asc", sep="\\")
D.friabilis1_3 <- paste(input_dir, "Dictyota_friabilis1_3.asc", sep="\\")
D.friabilis1_4 <- paste(input_dir, "Dictyota_friabilis1_4.asc", sep="\\")

D.intermedia_0 <- paste(input_dir, "Dictyota_intermedia_0.asc", sep="\\")
D.intermedia_1 <- paste(input_dir, "Dictyota_intermedia_1.asc", sep="\\")
D.intermedia_2 <- paste(input_dir, "Dictyota_intermedia_2.asc", sep="\\")
D.intermedia_3 <- paste(input_dir, "Dictyota_intermedia_3.asc", sep="\\")
D.intermedia_4 <- paste(input_dir, "Dictyota_intermedia_4.asc", sep="\\")

D.cyanoloma_0 <- paste(input_dir, "Dictyota_cyanoloma_0.asc", sep="\\")
D.cyanoloma_1 <- paste(input_dir, "Dictyota_cyanoloma_1.asc", sep="\\")
D.cyanoloma_2 <- paste(input_dir, "Dictyota_cyanoloma_2.asc", sep="\\")
D.cyanoloma_3 <- paste(input_dir, "Dictyota_cyanoloma_3.asc", sep="\\")
D.cyanoloma_4 <- paste(input_dir, "Dictyota_cyanoloma_4.asc", sep="\\")

D.stolonifera_0 <- paste(input_dir, "Dictyota_stolonifera_0.asc", sep="\\")
D.stolonifera_1 <- paste(input_dir, "Dictyota_stolonifera_1.asc", sep="\\")
D.stolonifera_2 <- paste(input_dir, "Dictyota_stolonifera_2.asc", sep="\\")
D.stolonifera_3 <- paste(input_dir, "Dictyota_stolonifera_3.asc", sep="\\")
D.stolonifera_4 <- paste(input_dir, "Dictyota_stolonifera_4.asc", sep="\\")

D.liturata_0 <- paste(input_dir, "Dictyota_liturata_0.asc", sep="\\")
D.liturata_1 <- paste(input_dir, "Dictyota_liturata_1.asc", sep="\\")
D.liturata_2 <- paste(input_dir, "Dictyota_liturata_2.asc", sep="\\")
D.liturata_3 <- paste(input_dir, "Dictyota_liturata_3.asc", sep="\\")
D.liturata_4 <- paste(input_dir, "Dictyota_liturata_4.asc", sep="\\")

D.grossedentata_0 <- paste(input_dir, "Dictyota_grossedentata_0.asc", sep="\\")
D.grossedentata_1 <- paste(input_dir, "Dictyota_grossedentata_1.asc", sep="\\")
D.grossedentata_2 <- paste(input_dir, "Dictyota_grossedentata_2.asc", sep="\\")
D.grossedentata_3 <- paste(input_dir, "Dictyota_grossedentata_3.asc", sep="\\")
D.grossedentata_4 <- paste(input_dir, "Dictyota_grossedentata_4.asc", sep="\\")

D.acutiloba1_0 <- paste(input_dir, "Dictyota_acutiloba1_0.asc", sep="\\")
D.acutiloba1_1 <- paste(input_dir, "Dictyota_acutiloba1_1.asc", sep="\\")
D.acutiloba1_2 <- paste(input_dir, "Dictyota_acutiloba1_2.asc", sep="\\")
D.acutiloba1_3 <- paste(input_dir, "Dictyota_acutiloba1_3.asc", sep="\\")
D.acutiloba1_4 <- paste(input_dir, "Dictyota_acutiloba1_4.asc", sep="\\")

D.ciliolata_0 <- paste(input_dir, "Dictyota_ciliolata_0.asc", sep="\\")
D.ciliolata_1 <- paste(input_dir, "Dictyota_ciliolata_1.asc", sep="\\")
D.ciliolata_2 <- paste(input_dir, "Dictyota_ciliolata_2.asc", sep="\\")
D.ciliolata_3 <- paste(input_dir, "Dictyota_ciliolata_3.asc", sep="\\")
D.ciliolata_4 <- paste(input_dir, "Dictyota_ciliolata_4.asc", sep="\\")

niches <- c(D.humifusa_0,D.humifusa_1,D.humifusa_2,D.humifusa_3,D.humifusa_4,D.dichotoma1_0,D.dichotoma1_1,D.dichotoma1_2,D.dichotoma1_3,D.dichotoma1_4,D.bartayresiana_0,D.bartayresiana_1,D.bartayresiana_2,D.bartayresiana_3,D.bartayresiana_4,D.binghamiae_0,D.binghamiae_1,D.binghamiae_2,D.binghamiae_3,D.binghamiae_4,D.adnata_0,D.adnata_1,D.adnata_2,D.adnata_3,D.adnata_4,D.spiralis_0,D.spiralis_1,D.spiralis_2,D.spiralis_3,D.spiralis_4,D.crenulata_0,D.crenulata_1,D.crenulata_2,D.crenulata_3,D.crenulata_4,D.implexa_0,D.implexa_1,D.implexa_2,D.implexa_3,D.implexa_4,D.ceylanica5_0,D.ceylanica5_1,D.ceylanica5_2,D.ceylanica5_3,D.ceylanica5_4,D.fasciola_0,D.fasciola_1,D.fasciola_2,D.fasciola_3,D.fasciola_4,D.mediterranea_0,D.mediterranea_1,D.mediterranea_2,D.mediterranea_3,D.mediterranea_4,D.friabilis1_0,D.friabilis1_1,D.friabilis1_2,D.friabilis1_3,D.friabilis1_4,D.intermedia_0,D.intermedia_1,D.intermedia_2,D.intermedia_3,D.intermedia_4,D.cyanoloma_0,D.cyanoloma_1,D.cyanoloma_2,D.cyanoloma_3,D.cyanoloma_4,D.stolonifera_0,D.stolonifera_1,D.stolonifera_2,D.stolonifera_3,D.stolonifera_4,D.liturata_0,D.liturata_1,D.liturata_2,D.liturata_3,D.liturata_4,D.grossedentata_0,D.grossedentata_1,D.grossedentata_2,D.grossedentata_3,D.grossedentata_4,D.acutiloba1_0,D.acutiloba1_1,D.acutiloba1_2,D.acutiloba1_3,D.acutiloba1_4,D.ciliolata_0,D.ciliolata_1,D.ciliolata_2,D.ciliolata_3,D.ciliolata_4)
#niches <- c(D.humifusa_0,D.humifusa_1)

read.ascii <- function (fname) {
  t = file(fname, "r")
  l5 = readLines(t, n = 6)
  map = scan(t, as.numeric(0), quiet = TRUE)
  close(t)
  map[map == -9999] = NA 
  map2 <- map[!is.na(map)] # remove NA values
  map2
}

weighted.ascii <- function(fname) {
  print(fname)
  x <- read.ascii(fname)
  xSUM <- sum(x)
  x <- x/xSUM
  x
}

enm <- function (x_weighted, y_weighted) {
  xx <- x_weighted
  yy <- y_weighted
  D <- 1 - 0.5 * sum(abs(xx - yy))
  H <- sqrt(sum((sqrt(xx) - sqrt(yy))^2))
  I <- 1 - H^2 * 0.5
  c(D = D, I = I)
}

weighted.niches <- vector("list", length(niches))
for(i in 1:length(niches)){
  weighted.niches[[i]] <- weighted.ascii(niches[[i]])
}

#weighted.niches <- lapply(niches, weighted.ascii)

nspec <- length(niches)
DI <- matrix(nrow = nspec, ncol = nspec)
rownames(DI) <- colnames(DI) <- niches
for (i in 1:(nspec - 1)){
  X <- weighted.niches[[i]]
  for (j in (i + 1):nspec){
    Y <- weighted.niches[[j]]
    dhi <- enm(X, Y)
    DI[i, j] <- dhi[1]
    DI[j, i] <- dhi[2]
  }
}

write.table(DI, file=output_file)
