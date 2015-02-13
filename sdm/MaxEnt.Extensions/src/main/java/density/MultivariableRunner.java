/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package density;

import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.ThreadFactory;
import java.util.concurrent.TimeUnit;
import javax.swing.JOptionPane;

/**
 *
 * @author swbosch
 */
public class MultivariableRunner extends Runner {

    /* replace below code in order to increase the speed of calculation of multiple models for the same species:
    
     INPUT:
     - swd file with
     species points + data for all features
     background points + data for all features
     - predictor sets (as e.g. integer index arrays or bitmap arrays)
    
     OUTPUT:
     for each predictor set:
     - AUC_train
     - AUC_test
    
     */
    HashMap<String, Method> methods;
    ArrayList<String[]> correlationGroups;
    int subsetSize;

    public MultivariableRunner(String speciesFile, String backgroundFile, String outputDir) {
        super(getParameters(speciesFile, backgroundFile, outputDir));
        // initialize methods hash
        Method[] ms = this.getClass().getSuperclass().getDeclaredMethods();
        methods = new HashMap<String, Method>(ms.length);
        for (Method m : ms) {
            m.setAccessible(true);
            String mname = m.getName() + m.getParameterTypes().length;
            if (methods.containsKey(mname)) {
                Utils.echoln("duplicate mname " + mname);
            } else {
                methods.put(mname, m);
            }
        }

        this.params.setSelections(); // select all layers and species
        // use subset of layers
        //this.params.layers = new String[]{ "calcite_mean", "chlo_mean", "da_mean", "dissox_mean" }; 
    }

    static Params getParameters(String speciesFile, String backgroundFile, String outputDir) {
        Params params = new Params();
        params.setSamplesfile(speciesFile);
        params.setEnvironmentallayers(backgroundFile);
        params.setOutputdirectory(outputDir);
        params.setRandomtestpoints(50); // random test sample percentage

        params.setThreads(1);//Runtime.getRuntime().availableProcessors());
        params.setThreads(Runtime.getRuntime().availableProcessors());

        params.setOutputgrids(false); // no pictures, no grid file output
        params.setVisible(false);
        params.setWarnings(false);
        params.setPrefixes(false);
        params.setPictures(false);
        params.setRandomseed(false);
        params.setResponsecurves(false);
        params.setWriteclampgrid(false);
        params.setWritemess(false);
        params.setPlots(false);
        params.setJackknife(false);
        params.setAskoverwrite(false);
        params.setAutorun(true);
        params.setVerbose(false);
        params.setHinge(false); // hinge features are complex AND slow (10x slower with hinge features)
        params.setAllowpartialdata(false);
        return params;
    }

    public void run(ArrayList<String[]> correlationGroups, int subsetSize) {
        //this.start2();
        this.subsetSize = subsetSize;
        this.correlationGroups = correlationGroups;

//        params.togglelayerselected("da_mean");
//        
//        params.layers = new String[]{ "da_mean", "phos_mean", "dissox_mean"};
//        this.correlationGroups = new ArrayList<String[]> ();
//        this.correlationGroups.add(new String[]{"da_mean"});
//        this.correlationGroups.add(new String[]{"phos_mean"});
//        this.correlationGroups.add(new String[]{"dissox_mean"});
        this.start_refactored();
        this.end();

        //this.start_original();
        //this.end();
    }

// <editor-fold defaultstate="collapsed" desc="private fields and methods reflection">
//    void setThisStartedHtmlPictureSection(boolean v) {
//        try {
//            Field f = this.getClass().getSuperclass().getField("startedPictureHtmlSection");
//            f.setAccessible(true);
//        } catch (Exception ex) {
//        }
//    }
//    boolean gridsFromFile() {
//        try {
//            Method m = methods.get("gridsFromFile0");
//            return (Boolean) m.invoke(this);
//        } catch (Exception ex) {
//        }
//        return true;
//    }
//    GridSet initializeGrids() {
//        try {
//            Method m = methods.get("initializeGrids0");
//            return (GridSet) m.invoke(this);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//
//    Feature[] makeFeatures(Feature[] features) {
//        try {
//            Method m = methods.get("makeFeatures1");
//            return (Feature[]) m.invoke(this, features);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//
//    Feature[] getTrueBaseFeatures(Feature[] features) {
//        try {
//            Method m = methods.get("getTrueBaseFeatures1");
//            return (Feature[]) m.invoke(this, features);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//
//    void writeLog() {
//        try {
//            Method m = methods.get("writeLog0");
//            m.invoke(this);
//        } catch (Exception ex) {
//        }
//    }
//
//    Sample[] withAllData(Feature[] f, Sample[] ss) {
//        try {
//            Method m = methods.get("withAllData2");
//            return (Sample[]) m.invoke(this, f, ss);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//
//    void maybeReplicateHtml(String theSpecies, Feature[] baseFeatures) {
//        try {
//            Method m = methods.get("maybeReplicateHtml2");
//            m.invoke(this, theSpecies, baseFeatures);
//        } catch (Exception ex) {
//        }
//    }
//
//    Feature[] featuresWithSamples(Feature[] f, Sample[] ss) {
//        try {
//            Method m = methods.get("featuresWithSamples2");
//            return (Feature[]) m.invoke(this, f, ss);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//
//    double getTestGain(FeaturedSpace X) {
//        try {
//            Method m = methods.get("getTestGain1");
//            return (Double) m.invoke(this, X);
//        } catch (Exception ex) {
//        }
//        return Double.NaN;
//    }
//
//    void startHtmlPage() {
//        try {
//            Method m = methods.get("startHtmlPage0");
//            m.invoke(this);
//        } catch (Exception ex) {
//        }
//    }
//
//    double[][] writeCumulativeIndex(double[] weights, String raw2cumfile, FeaturedSpace X, double auc, double trainauc, Feature[] baseFeatures, double entropy) throws IOException {
//        try {
//            Method m = methods.get("writeCumulativeIndex7");
//            return (double[][]) m.invoke(this, weights, raw2cumfile, X, auc, trainauc, baseFeatures, entropy);
//        } catch (Exception ex) {
//            if (ex instanceof IOException) {
//                throw ((IOException) ex);
//            }
//        }
//        return null;
//    }
//
//    void writeSampleAverages(Feature[] baseFeatures, Sample[] samples) throws IOException {
//        try {
//            Method m = methods.get("writeSampleAverages2");
//            m.invoke(this, baseFeatures, samples);
//        } catch (Exception ex) {
//            if (ex instanceof IOException) {
//                throw ((IOException) ex);
//            }
//        }
//    }
//
//    void createProfiles(final Feature[] baseFeatures, String lambdafile, final Sample[] ss) throws IOException {
//        try {
//            Method m = methods.get("createProfiles3");
//            m.invoke(this, baseFeatures, lambdafile, ss);
//        } catch (Exception ex) {
//            if (ex instanceof IOException) {
//                throw ((IOException) ex);
//            }
//        }
//    }
//
//    void writeContributions(double[][] contributions, String msg) {
//        try {
//            Method m = methods.get("writeContributions2");
//            m.invoke(this, contributions, msg);
//        } catch (Exception ex) {
//        }
//    }
//
//    void writeSummary(MaxentRunResults res, double testGain, double auc, double aucSD, double trainauc, CsvWriter writer, Feature[] baseFeatures, double[][] jackknifeGain, double entropy, double prevalence, double[] permcontribs) {
//        try {
//            Method m = methods.get("writeSummary11");
//            m.invoke(this, res, testGain, auc, aucSD, trainauc, writer, baseFeatures, jackknifeGain, entropy, prevalence, permcontribs);
//        } catch (Exception ex) {
//        }
//    }
//
//    void writeHtmlDetails(MaxentRunResults res, double testGain, double auc, double aucSD, double trainauc) {
//        try {
//            Method m = methods.get("writeHtmlDetails5");
//            m.invoke(this, res, testGain, auc, aucSD, trainauc);
//        } catch (Exception ex) {
//        }
//    }
//
//    PrintWriter getHtmlout(){
//        try {
//            Field f = this.getClass().getSuperclass().getField("htmlout");
//            f.setAccessible(true);
//            return (PrintWriter)f.get(this);
//        } catch (Exception ex) {
//        }
//        return null;
//    }
//    </editor-fold>
    void sInit() {
        Utils.applyStaticParams(this.params);
        if (this.params.layers == null) {
            this.params.setSelections();
        }
        if (this.cv() && this.replicates() > 1 && this.params.getRandomtestpoints() != 0) {
            Utils.warn2("Resetting random test percentage to zero because cross-validation in use", "skippingHoldoutBecauseCV");
            this.params.setRandomtestpoints(0);
        }
        if (this.subsample() && this.replicates() > 1 && this.params.getint("randomTestPoints") <= 0 && !this.is("manualReplicates")) {
            this.popupError("Subsampled replicates require nonzero random test percentage", null);
            return;
        }
        if (!this.cv() && this.replicates() > 1 && !this.params.getboolean("randomseed") && !this.is("manualReplicates")) {
            Utils.warn2("Setting randomseed to true so that replicates are not identical", "settingrandomseedtrue");
            this.params.setValue("randomseed", true);
        }
        if (this.outDir() == null || this.outDir().trim().equals("")) {
            this.popupError("An output directory is needed", null);
            return;
        }
        if (!new File(this.outDir()).exists()) {
            this.popupError("Output directory does not exist", null);
            return;
        }
        if (!this.biasFile().equals("") && this.gridsFromFile()) {
            this.popupError("Bias grid cannot be used with SWD-format background", null);
            return;
        }
        if (this.is("perSpeciesResults") && this.replicates() > 1) {
            Utils.warn2("PerSpeciesResults is not supported with replicates>1, setting perSpeciesResults to false", "unsettingPerSpeciesResults");
            this.params.setValue("perSpeciesResults", false);
        }
        try {
            Utils.openLog(this.outDir(), this.params.getString("logFile"));
        } catch (IOException e) {
            this.popupError("Error opening log file", e);
            return;
        }
        Utils.startTimer();
        Utils.echoln(new Date().toString());
        Utils.echoln("MaxEnt version " + Utils.version);
        Utils.interrupt = false;
        if (this.threads() > 1) {
            this.parallelRunner = new ParallelRun(this.threads());
        }
        Thread.currentThread().setPriority(4);
        if (this.params.layers == null || this.params.layers.length == 0) {
            this.popupError("No environmental layers selected", null);
            return;
        }
        if (this.params.species.length == 0) {
            this.popupError("No species selected", null);
            return;
        }
        if (Utils.progressMonitor != null) {
            Utils.progressMonitor.setMaximum(100);
        }
        Utils.generator = new Random(this.params.isRandomseed() ? System.currentTimeMillis() : 0L);
        this.gs = this.initializeGrids();
        if (Utils.interrupt || this.gs == null) {
            return;
        }
    }

    SampleSet2 sCreateSampleset2() {
        final SampleSet2 sampleSet2 = this.gs.train;
        if (this.projectionLayers().length() > 0) {
            final String[] dirs = this.projectionLayers().trim().split(",");
            this.projectPrefix = new String[dirs.length];
            for (int i = 0; i < this.projectPrefix.length; ++i) {
                this.projectPrefix[i] = new File(dirs[i].trim()).getPath();
            }
        }
        if (!this.testSamplesFile().equals("")) {
            this.testSampleSet = this.gs.test;
        }
        if (this.is("removeDuplicates")) {
            sampleSet2.removeDuplicates(this.gridsFromFile() ? null : this.gs.getDimension());
        }
        return sampleSet2;
    }

    boolean sCheckLambdaExists(File lf, Feature[] baseFeatures) {
        if (lf.exists()) {
            if (this.is("skipIfExists")) {
                if (this.is("appendtoresultsfile")) {
                    this.maybeReplicateHtml(this.theSpecies, this.getTrueBaseFeatures(baseFeatures));
                }
                return true;
            } else if (this.is("askoverwrite") && Utils.topLevelFrame != null) {
                final Object[] options = {"Skip", "Skip all", "Redo", "Redo all"};
                final int val = JOptionPane.showOptionDialog(Utils.topLevelFrame, "Output file exists for " + this.theSpecies, "File already exists", -1, 2, null, options, options[0]);
                switch (val) {
                    case 1: {
                        this.params.setValue("skipIfExists", true);
                    }
                    case 0: {
                        if (this.is("appendtoresultsfile")) {
                            this.maybeReplicateHtml(this.theSpecies, this.getTrueBaseFeatures(baseFeatures));
                        }
                        return true;
                    }
                    case 3: {
                        this.params.setValue("askoverwrite", false);
                        break;
                    }
                }
            }
        }
        return false;
    }

    boolean sSpeciesProject(String lambdafile, Sample[] ss, FeaturedSpace X, boolean explain, Feature[] baseFeaturesNoBias, String suffix) {
        this.projectedGrids = new ArrayList();
        if (this.projectPrefix != null && this.is("outputGrids")) {
            for (int j = 0; j < this.projectPrefix.length; ++j) {
                if (Utils.interrupt) {
                    return false;
                }
                String prefix = new File(this.projectPrefix[j]).getName();
                if (prefix.endsWith(".csv")) {
                    prefix = prefix.substring(0, prefix.length() - 4);
                }
                final boolean isFile = new File(this.projectPrefix[j]).isFile();
                final File ff = new File(this.outDir(), this.theSpecies + "_" + prefix + (isFile ? ".csv" : suffix));
                final File ffclamp = new File(this.outDir(), this.theSpecies + "_" + prefix + "_clamping" + (isFile ? ".csv" : suffix));
                try {
                    final Project proj2 = new Project(this.params);
                    proj2.needLayers = this.allLayers;
                    proj2.doProject(lambdafile, this.projectPrefix[j], ff.getPath(), this.cumulative(), false, this.is("writeClampGrid") ? ffclamp.getPath() : ((String) null));
                    if (Utils.interrupt) {
                        return false;
                    }
                    this.projectedGrids.add("<a href = \"" + ff.getName() + "\">The model applied to the environmental layers in " + this.projectPrefix[j] + "</a>");
                    if (this.is("pictures") && !isFile) {
                        this.makePicture(ff.getPath(), ss, X.testSamples, this.projectPrefix[j]);
                        this.makeExplain(explain, ff, lambdafile, this.theSpecies + "_" + prefix + "_explain.bat", new File(this.projectPrefix[j]).getAbsolutePath());
                        if (this.is("writeClampGrid")) {
                            this.makePicture(ffclamp.getPath(), new Sample[0], new Sample[0], this.projectPrefix[j], true);
                        }
                        this.makeNovel(baseFeaturesNoBias, this.projectPrefix[j], new File(this.outDir(), this.theSpecies + "_" + prefix + "_novel" + suffix).getPath());
                    }
                    if (this.applyThresholdValue != -1.0 && !isFile) {
                        new Threshold().applyThreshold(ff.getPath(), this.applyThresholdValue);
                    }
                } catch (IOException e7) {
                    this.popupError("Error projecting", e7);
                    return false;
                }
            }
        }
        return !Utils.interrupt;
    }

    Sample[] sGetSamples(Feature[] baseFeatures) {
        Sample[] sss = this.sampleSet.getSamples(this.theSpecies);
        if (!this.params.allowpartialdata()) {
            sss = this.withAllData(baseFeatures, sss);
        }
        final Sample[] ss = sss;
        if (ss.length == 0) {
            Utils.warn2("Skipping " + this.theSpecies + " because it has 0 training samples", "skippingBecauseNoTrainingSamples");
            return null;
        }
        if (this.testSampleSet != null) {
            final int len = this.testSampleSet.getSamples(this.theSpecies).length;
            if (len == 0) {
                Utils.warn2("Skipping " + this.theSpecies + " because it has 0 test samples", "skippingBecauseNoTestSamples");
                return null;
            }
        }
        return ss;
    }

    // new code
    private void safePrintln(String s) {
        synchronized (System.out) {
            System.out.println(s);
        }
    }

    // thread local version of the features
    static class FeaturesFactory {
        static Feature[] baseFeaturesWithSamples =null;
        static MultivariableRunner runner = null;
        private static final ThreadLocal<Feature[]> features = new ThreadLocal<Feature[]>();
        
        public static Feature[] getFeatures(){
            if(features.get() == null){
                features.set(runner.makeFeatures(baseFeaturesWithSamples));
            }
            return features.get();
        }
    }

    // new code
    class RunSubset implements Runnable {

        private final String[] names;
        private final Sample[] ss;
        private final Sample[] testSamples;
        private final MultivariableRunner runner;

        public RunSubset(String[] names, Sample[] ss, Sample[] testSamples, MultivariableRunner runner) {
            this.names = Arrays.copyOf(names, names.length);
            this.ss = ss;
            this.testSamples = testSamples;
            this.runner = runner;
        }

        // new code
        public Feature[] subsetFeatures(Feature[] features, String[] names) {
            final ArrayList<Feature> subset = new ArrayList<Feature>(features.length);
            for (Feature f : features) {
                String a;
                String b = "";
                if (f.name.contains("*")) {
                    a = f.name.split("\\*")[0];
                    b = f.name.split("\\*")[1];
                } else {
                    a = f.name.replaceFirst("\\^2", "").replaceFirst("__rev", "");
                }
                for (String name : names) {
                    if (a.equals(name)) {
                        a = "";
                    }
                    if (b.equals(name)) {
                        b = "";
                    }
                }
                if (a.length() == 0 && b.length() == 0) {
                    subset.add(f);
                }
            }
            return subset.toArray(new Feature[subset.size()]);
        }

        public void run() {
            Feature[] subset = subsetFeatures(FeaturesFactory.getFeatures(), names);
            final MaxentRunResults res = this.runner.maxentRun(subset, ss, testSamples);

            for (String s : names) {
                Utils.echo(s);
                Utils.echo(" ");
            }
            Utils.echoln();
            for (Feature f : subset) {
                Utils.echo(f.name);
                Utils.echo(" ");
            }
            Utils.echoln();
            final FeaturedSpace X = res.X;
            res.removeBiasDistribution();
            final DoubleIterator backgroundIterator = null;
            final double auc = X.getAUC(backgroundIterator, X.testSamples);
            final double aucSD = X.aucSD;
            final double trainauc = X.getAUC(backgroundIterator, X.samples);
            Utils.echoln("AUC_TEST " + auc + " AUC_SD " + aucSD + " AUC_TRAIN " + trainauc);

            String cols = "";
            for (String s : names) {
                cols += s + "|";
            }
            safePrintln(cols + "," + trainauc + "," + auc + "," + aucSD);
            // TODO maybe write lambda's
        }
    }

    // new code
    void runFeatureCombinations(Sample[] ss, Feature[] features, int subsetSize, Feature[] baseFeaturesWithSamples) {
        final Sample[] testSamples = (Sample[]) ((this.testSampleSet != null) ? this.testSampleSet.getSamples(this.theSpecies) : null);
        // increases speed but is accurracy still ok ?
        //this.params.setMaximumiterations(500/2);
        final MultiCombinationGenerator gen = new MultiCombinationGenerator(correlationGroups, subsetSize);
        Utils.echoln("Run maxent multiple subsets " + gen.getTotal());

        // DISABLE logging
        PrintStream log = Utils.logOut;
        Utils.logOut = null;

        safePrintln("names,AUC_train,AUC_test,AUC_SD");

        for (Feature f : features) {
            Utils.echo(f.name);
            Utils.echo(" ");
        }
        Utils.echoln();

        FeaturesFactory.runner = this;
        FeaturesFactory.baseFeaturesWithSamples = baseFeaturesWithSamples;
        ExecutorService exec = Executors.newFixedThreadPool(this.params.getThreads());
        ArrayList<Future> futures = new ArrayList<Future>();
        try {
            try {
                while (gen.hasMore()) {
                    String[] names = gen.getNext();
                    Future f = exec.submit(new RunSubset(names, ss, testSamples, this));
                    futures.add(f);

                    // limit amount of submitted runnables
                    if (futures.size() > 60) {
                        for (int i = futures.size() - 1; i >= 0; i--) {
                            if (futures.get(i).isDone()) {
                                futures.remove(i);
                            }
                        }
                        if (futures.size() > 80){
                            // block current thread for 30 seconds
                            exec.awaitTermination(30L, TimeUnit.SECONDS);
                        }
                    }
                }
            } finally {
                exec.shutdown();
            }
            // check if all tasks have finished
            boolean done = exec.awaitTermination(10L, TimeUnit.MINUTES);
            System.err.println(done ? "Finished all" : "Some left");
        } catch (InterruptedException ex) {
            safePrintln(ex.toString());
        }
        Utils.echoln("multiple subsets done " + gen.getTotal());
        // ENABLE logging again
        Utils.logOut = log;
    }

    boolean sProcessSpecies(Sample[] ss, Feature[] baseFeatures, boolean addSamplesToFeatures, Feature[] features) {

        final String suffix = this.outputFileType();
        final File f = new File(this.outDir(), this.theSpecies + suffix);
        final File lf = new File(this.outDir(), this.theSpecies + ".lambdas");

        Utils.reportMemory("getSamples");
        if (sCheckLambdaExists(lf, baseFeatures)) { // if already exists and can't overwrite then skip to next species
            return true;
        }
        Feature[] baseFeaturesWithSamples = baseFeatures;
        if (addSamplesToFeatures) {
            features = null;
            baseFeaturesWithSamples = this.featuresWithSamples(baseFeatures, ss);
            if (baseFeaturesWithSamples == null) {
                return true;
            }
            features = this.makeFeatures(baseFeaturesWithSamples);
        }
        final Feature[] baseFeaturesNoBias = this.getTrueBaseFeatures(baseFeaturesWithSamples);
        if (Utils.interrupt) {
            return false;
        }
        Utils.reportDoing(this.theSpecies + ": ");
        this.contributions = null;

        final MaxentRunResults res = this.maxentRun(features, ss, (Sample[]) ((this.testSampleSet != null) ? this.testSampleSet.getSamples(this.theSpecies) : null));

        if (res == null) {
            return false;
        }

        // NEW CODE
        runFeatureCombinations(ss, features, this.subsetSize, baseFeaturesWithSamples); /* one from every corrrelation group */

        Utils.echoln("Resulting gain: " + res.gain);
        final FeaturedSpace X = res.X;
        res.removeBiasDistribution();
        final boolean gsfromfile = this.gs instanceof GridSetFromFile;
        final boolean writeRaw2cumfile = true;
        final DoubleIterator backgroundIterator = null;
        final double auc = X.getAUC(backgroundIterator, X.testSamples);
        final double aucSD = X.aucSD;
        final double trainauc = X.getAUC(backgroundIterator, X.samples);
        this.aucmax = X.aucmax;
        if (backgroundIterator != null) {
            X.setDensityNormalizer(backgroundIterator);
        }
        final String lambdafile = lf.getAbsolutePath();
        final double entropy = X.getEntropy();
        final double prevalence = X.getPrevalence(this.params);
        try {
            X.writeFeatureWeights(lambdafile);
        } catch (IOException e4) {
            this.popupError("Error writing feature weights file", e4);
            return false;
        }
        final double testGain = (this.testSampleSet == null) ? 0.0 : this.getTestGain(X);
        this.startHtmlPage();
        if (writeRaw2cumfile) {
            this.raw2cumfile = raw2cumfile(lambdafile);
            try {
                final double[] weights = (backgroundIterator == null) ? X.getWeights() : backgroundIterator.getvals(X.densityNormalizer);
                this.writeCumulativeIndex(weights, this.raw2cumfile, X, auc, trainauc, baseFeaturesNoBias, entropy);
            } catch (IOException e5) {
                this.popupError("Error writing raw-to-cumulative index file", e5);
                return false;
            }
        }
        this.writtenGrid = null;
        if (Utils.interrupt) {
            return false;
        }
        boolean explain = true;
        for (final String s : res.featureTypes) {
            if (s.equals("product")) {
                explain = false;
            }
        }
        this.startedPictureHtmlSection = false;
        if (this.is("outputGrids")) {
            final String filename = gsfromfile ? new File(this.outDir(), this.theSpecies + ".csv").getPath() : f.getPath();
            try {
                final Project proj = new Project(this.params);
                proj.entropy = entropy;
                if (gsfromfile) {
                    proj.doProject(lambdafile, (GridSetFromFile) this.gs, filename, this.cumulative(), false);
                } else {
                    proj.doProject(lambdafile, this.environmentalLayers(), filename, this.cumulative(), false, null);
                }
                proj.entropy = -1.0;
                if (Utils.interrupt) {
                    return false;
                }
                this.writtenGrid = filename;
                if (this.is("pictures") && !gsfromfile) {
                    this.makePicture(f.getPath(), ss, X.testSamples, null);
                }
                this.makeExplain(explain, f, lambdafile, this.theSpecies + "_explain.bat", new File(this.environmentalLayers()).getAbsolutePath());
                if (this.applyThresholdValue != -1.0 && !gsfromfile) {
                    new Threshold().applyThreshold(f.getPath(), this.applyThresholdValue);
                }
            } catch (IOException e6) {
                this.popupError("Error writing output file " + new File(filename).getName(), e6);
                return false;
            }
        }
        if (Utils.interrupt) {
            return false;
        }
        if (!sSpeciesProject(lambdafile, ss, X, explain, baseFeaturesNoBias, suffix)) {
            return false;
        }
        try {
            this.writeSampleAverages(baseFeaturesWithSamples, ss);
        } catch (IOException e8) {
            this.popupError("Error writing file", e8);
            return false;
        }
        if (this.is("responsecurves")) {
            try {
                this.createProfiles(baseFeaturesWithSamples, lambdafile, ss);
            } catch (IOException e8) {
                this.popupError("Error writing response curves for " + this.theSpecies, e8);
                return false;
            }
        }
        if (Utils.interrupt) {
            return false;
        }
        double[] permcontribs = null;
        try {
            permcontribs = new PermutationImportance().go(baseFeatures, ss, lambdafile);
        } catch (IOException e6) {
            this.popupError("Error computing permutation importance for " + this.theSpecies, e6);
            return false;
        }
        this.writeContributions(new double[][]{this.contributions, permcontribs}, "");
        final double[][] jackknifeGain = (double[][]) ((this.is("jackknife") && baseFeaturesNoBias.length > 1) ? this.jackknifeGain(baseFeaturesWithSamples, ss, X.testSamples, res.gain, testGain, auc) : null);
        this.writeSummary(res, testGain, auc, aucSD, trainauc, this.results, baseFeaturesNoBias, jackknifeGain, entropy, prevalence, permcontribs);
        this.writeHtmlDetails(res, testGain, auc, aucSD, trainauc);
        this.htmlout.close();
        this.maybeReplicateHtml(this.theSpecies, baseFeaturesNoBias);

        return !Utils.interrupt;
    }

    public synchronized void start_refactored() {
        sInit();
        SampleSet2 sampleSet2 = sCreateSampleset2();
        if (Utils.interrupt) {
            return;
        }
        final Feature[] baseFeatures = (Feature[]) ((this.gs == null) ? null : this.gs.toFeatures());
        this.coords = this.gs.getDimension().coords;
        if (baseFeatures == null || baseFeatures.length == 0 || baseFeatures[0].n == 0) {
            this.popupError("No background points with data in all layers", null);
            return;
        }
        this.samplesAddedToFeatures = this.is("addSamplesToBackground") && (sampleSet2.samplesHaveData || this.gs instanceof Extractor);
        final boolean addSamplesToFeatures = this.samplesAddedToFeatures;
        if (addSamplesToFeatures) {
            Utils.echoln("Adding samples to background in feature space");
        }
        Feature[] features = null;
        if (!addSamplesToFeatures) {
            features = this.makeFeatures(baseFeatures);
            if (Utils.interrupt) {
                return;
            }
        }
        this.sampleSet = sampleSet2;
        this.speciesCount = new HashMap<String, Integer>();
        if (this.replicates() > 1 && !this.is("manualReplicates")) {
            if (this.cv()) {
                for (final String s : this.sampleSet.getNames()) {
                    this.speciesCount.put(s, this.sampleSet.getSamples(s).length);
                }
                this.testSampleSet = this.sampleSet.splitForCV(this.replicates());
            } else {
                this.sampleSet.replicate(this.replicates(), this.bootstrap());
            }
            final ArrayList<String> torun = new ArrayList<String>();
            for (final String s2 : this.sampleSet.getNames()) {
                if (s2.matches(".*_[0-9]+$")) {
                    torun.add(s2);
                }
            }
            this.params.species = torun.toArray(new String[0]);
        }
        if (this.testSamplesFile().equals("") && this.params.getint("randomTestPoints") != 0) {
            final SampleSet train = null;
            if (!this.is("randomseed")) {
                Utils.generator = new Random(11111L);
            }
            this.testSampleSet = this.sampleSet.randomSample(this.params.getint("randomTestPoints"));
        }
        if (Utils.interrupt) {
            return;
        }
        this.writeLog();
        if (!this.is("perSpeciesResults")) {
            try {
                this.results = new CsvWriter(new File(this.outDir(), "maxentResults.csv"), this.is("appendtoresultsfile"));
            } catch (IOException e2) {
                this.popupError("Problem opening results file", e2);
                return;
            }
        }
        for (int sample = 0; sample < this.params.species.length; ++sample) {
            this.theSpecies = this.params.species[sample];
            if (Utils.interrupt) {
                return;
            }
            if (this.is("perSpeciesResults")) {
                try {
                    this.results = new CsvWriter(new File(this.outDir(), this.theSpecies + "Results.csv"));
                } catch (IOException e3) {
                    this.popupError("Problem opening " + this.theSpecies + " results file", e3);
                    return;
                }
            }
            Sample[] ss = sGetSamples(baseFeatures);
            if (ss == null) {
                continue;
            }
            boolean stop = !sProcessSpecies(ss, baseFeatures, addSamplesToFeatures, features);
            if (stop) {
                return;
            }
        }
        if (this.threads() > 1) {
            this.parallelRunner.close();
        }
    }

    public synchronized void start_original() {
        Utils.applyStaticParams(this.params);
        if (this.params.layers == null) {
            this.params.setSelections();
        }
        if (this.cv() && this.replicates() > 1 && this.params.getRandomtestpoints() != 0) {
            Utils.warn2("Resetting random test percentage to zero because cross-validation in use", "skippingHoldoutBecauseCV");
            this.params.setRandomtestpoints(0);
        }
        if (this.subsample() && this.replicates() > 1 && this.params.getint("randomTestPoints") <= 0 && !this.is("manualReplicates")) {
            this.popupError("Subsampled replicates require nonzero random test percentage", null);
            return;
        }
        if (!this.cv() && this.replicates() > 1 && !this.params.getboolean("randomseed") && !this.is("manualReplicates")) {
            Utils.warn2("Setting randomseed to true so that replicates are not identical", "settingrandomseedtrue");
            this.params.setValue("randomseed", true);
        }
        if (this.outDir() == null || this.outDir().trim().equals("")) {
            this.popupError("An output directory is needed", null);
            return;
        }
        if (!new File(this.outDir()).exists()) {
            this.popupError("Output directory does not exist", null);
            return;
        }
        if (!this.biasFile().equals("") && this.gridsFromFile()) {
            this.popupError("Bias grid cannot be used with SWD-format background", null);
            return;
        }
        if (this.is("perSpeciesResults") && this.replicates() > 1) {
            Utils.warn2("PerSpeciesResults is not supported with replicates>1, setting perSpeciesResults to false", "unsettingPerSpeciesResults");
            this.params.setValue("perSpeciesResults", false);
        }
        try {
            Utils.openLog(this.outDir(), this.params.getString("logFile"));
        } catch (IOException e) {
            this.popupError("Error opening log file", e);
            return;
        }
        Utils.startTimer();
        Utils.echoln(new Date().toString());
        Utils.echoln("MaxEnt version " + Utils.version);
        Utils.interrupt = false;
        if (this.threads() > 1) {
            this.parallelRunner = new ParallelRun(this.threads());
        }
        Thread.currentThread().setPriority(4);
        if (this.params.layers == null || this.params.layers.length == 0) {
            this.popupError("No environmental layers selected", null);
            return;
        }
        if (this.params.species.length == 0) {
            this.popupError("No species selected", null);
            return;
        }
        if (Utils.progressMonitor != null) {
            Utils.progressMonitor.setMaximum(100);
        }
        Utils.generator = new Random(this.params.isRandomseed() ? System.currentTimeMillis() : 0L);
        this.gs = this.initializeGrids();
        if (Utils.interrupt || this.gs == null) {
            return;
        }
        final SampleSet2 sampleSet2 = this.gs.train;
        if (this.projectionLayers().length() > 0) {
            final String[] dirs = this.projectionLayers().trim().split(",");
            this.projectPrefix = new String[dirs.length];
            for (int i = 0; i < this.projectPrefix.length; ++i) {
                this.projectPrefix[i] = new File(dirs[i].trim()).getPath();
            }
        }
        if (!this.testSamplesFile().equals("")) {
            this.testSampleSet = this.gs.test;
        }
        if (Utils.interrupt) {
            return;
        }
        if (this.is("removeDuplicates")) {
            sampleSet2.removeDuplicates(this.gridsFromFile() ? null : this.gs.getDimension());
        }
        final Feature[] baseFeatures = (Feature[]) ((this.gs == null) ? null : this.gs.toFeatures());
        this.coords = this.gs.getDimension().coords;
        if (baseFeatures == null || baseFeatures.length == 0 || baseFeatures[0].n == 0) {
            this.popupError("No background points with data in all layers", null);
            return;
        }
        final boolean samplesAddedToFeatures = this.is("addSamplesToBackground") && (sampleSet2.samplesHaveData || this.gs instanceof Extractor);
        this.samplesAddedToFeatures = samplesAddedToFeatures;
        final boolean addSamplesToFeatures = samplesAddedToFeatures;
        if (addSamplesToFeatures) {
            Utils.echoln("Adding samples to background in feature space");
        }
        Feature[] features = null;
        if (!addSamplesToFeatures) {
            features = this.makeFeatures(baseFeatures);
            if (Utils.interrupt) {
                return;
            }
        }
        this.sampleSet = sampleSet2;
        this.speciesCount = new HashMap<String, Integer>();
        if (this.replicates() > 1 && !this.is("manualReplicates")) {
            if (this.cv()) {
                for (final String s : this.sampleSet.getNames()) {
                    this.speciesCount.put(s, this.sampleSet.getSamples(s).length);
                }
                this.testSampleSet = this.sampleSet.splitForCV(this.replicates());
            } else {
                this.sampleSet.replicate(this.replicates(), this.bootstrap());
            }
            final ArrayList<String> torun = new ArrayList<String>();
            for (final String s2 : this.sampleSet.getNames()) {
                if (s2.matches(".*_[0-9]+$")) {
                    torun.add(s2);
                }
            }
            this.params.species = torun.toArray(new String[0]);
        }
        if (this.testSamplesFile().equals("") && this.params.getint("randomTestPoints") != 0) {
            final SampleSet train = null;
            if (!this.is("randomseed")) {
                Utils.generator = new Random(11111L);
            }
            this.testSampleSet = this.sampleSet.randomSample(this.params.getint("randomTestPoints"));
        }
        if (Utils.interrupt) {
            return;
        }
        this.writeLog();
        if (!this.is("perSpeciesResults")) {
            try {
                this.results = new CsvWriter(new File(this.outDir(), "maxentResults.csv"), this.is("appendtoresultsfile"));
            } catch (IOException e2) {
                this.popupError("Problem opening results file", e2);
                return;
            }
        }
        for (int sample = 0; sample < this.params.species.length; ++sample) {
            this.theSpecies = this.params.species[sample];
            if (Utils.interrupt) {
                return;
            }
            if (this.is("perSpeciesResults")) {
                try {
                    this.results = new CsvWriter(new File(this.outDir(), this.theSpecies + "Results.csv"));
                } catch (IOException e3) {
                    this.popupError("Problem opening " + this.theSpecies + " results file", e3);
                    return;
                }
            }
            final String suffix = this.outputFileType();
            final File f = new File(this.outDir(), this.theSpecies + suffix);
            final File lf = new File(this.outDir(), this.theSpecies + ".lambdas");
            final String lambdafile = lf.getAbsolutePath();
            Sample[] sss = this.sampleSet.getSamples(this.theSpecies);
            if (!this.params.allowpartialdata()) {
                sss = this.withAllData(baseFeatures, sss);
            }
            final Sample[] ss = sss;
            if (ss.length == 0) {
                Utils.warn2("Skipping " + this.theSpecies + " because it has 0 training samples", "skippingBecauseNoTrainingSamples");
            } else {
                if (this.testSampleSet != null) {
                    final int len = this.testSampleSet.getSamples(this.theSpecies).length;
                    if (len == 0) {
                        Utils.warn2("Skipping " + this.theSpecies + " because it has 0 test samples", "skippingBecauseNoTestSamples");
                        continue;
                    }
                }
                Utils.reportMemory("getSamples");
                if (lf.exists()) {
                    if (this.is("skipIfExists")) {
                        if (this.is("appendtoresultsfile")) {
                            this.maybeReplicateHtml(this.theSpecies, this.getTrueBaseFeatures(baseFeatures));
                        }
                        continue;
                    } else if (this.is("askoverwrite") && Utils.topLevelFrame != null) {
                        final Object[] options = {"Skip", "Skip all", "Redo", "Redo all"};
                        final int val = JOptionPane.showOptionDialog(Utils.topLevelFrame, "Output file exists for " + this.theSpecies, "File already exists", -1, 2, null, options, options[0]);
                        switch (val) {
                            case 1: {
                                this.params.setValue("skipIfExists", true);
                            }
                            case 0: {
                                if (this.is("appendtoresultsfile")) {
                                    this.maybeReplicateHtml(this.theSpecies, this.getTrueBaseFeatures(baseFeatures));
                                }
                                continue;
                            }
                            case 3: {
                                this.params.setValue("askoverwrite", false);
                                break;
                            }
                        }
                    }
                }
                Feature[] baseFeaturesWithSamples = baseFeatures;
                if (addSamplesToFeatures) {
                    features = null;
                    baseFeaturesWithSamples = this.featuresWithSamples(baseFeatures, ss);
                    if (baseFeaturesWithSamples == null) {
                        continue;
                    }
                    features = this.makeFeatures(baseFeaturesWithSamples);
                }
                final Feature[] baseFeaturesNoBias = this.getTrueBaseFeatures(baseFeaturesWithSamples);
                if (Utils.interrupt) {
                    return;
                }
                Utils.reportDoing(this.theSpecies + ": ");
                this.contributions = null;

                final MaxentRunResults res = this.maxentRun(features, ss, (Sample[]) ((this.testSampleSet != null) ? this.testSampleSet.getSamples(this.theSpecies) : null));

                if (res == null) {
                    return;
                }
                Utils.echoln("Resulting gain: " + res.gain);
                final FeaturedSpace X = res.X;
                res.removeBiasDistribution();
                final boolean gsfromfile = this.gs instanceof GridSetFromFile;
                final boolean writeRaw2cumfile = true;
                final DoubleIterator backgroundIterator = null;
                final double auc = X.getAUC(backgroundIterator, X.testSamples);
                final double aucSD = X.aucSD;
                final double trainauc = X.getAUC(backgroundIterator, X.samples);
                this.aucmax = X.aucmax;
                if (backgroundIterator != null) {
                    X.setDensityNormalizer(backgroundIterator);
                }
                final double entropy = X.getEntropy();
                final double prevalence = X.getPrevalence(this.params);
                try {
                    X.writeFeatureWeights(lambdafile);
                } catch (IOException e4) {
                    this.popupError("Error writing feature weights file", e4);
                    return;
                }
                final double testGain = (this.testSampleSet == null) ? 0.0 : this.getTestGain(X);
                this.startHtmlPage();
                if (writeRaw2cumfile) {
                    this.raw2cumfile = raw2cumfile(lambdafile);
                    try {
                        final double[] weights = (backgroundIterator == null) ? X.getWeights() : backgroundIterator.getvals(X.densityNormalizer);
                        this.writeCumulativeIndex(weights, this.raw2cumfile, X, auc, trainauc, baseFeaturesNoBias, entropy);
                    } catch (IOException e5) {
                        this.popupError("Error writing raw-to-cumulative index file", e5);
                        return;
                    }
                }
                this.writtenGrid = null;
                if (Utils.interrupt) {
                    return;
                }
                boolean explain = true;
                for (final String s3 : res.featureTypes) {
                    if (s3.equals("product")) {
                        explain = false;
                    }
                }
                this.startedPictureHtmlSection = false;
                if (this.is("outputGrids")) {
                    final String filename = gsfromfile ? new File(this.outDir(), this.theSpecies + ".csv").getPath() : f.getPath();
                    try {
                        final Project proj = new Project(this.params);
                        proj.entropy = entropy;
                        if (gsfromfile) {
                            proj.doProject(lambdafile, (GridSetFromFile) this.gs, filename, this.cumulative(), false);
                        } else {
                            proj.doProject(lambdafile, this.environmentalLayers(), filename, this.cumulative(), false, null);
                        }
                        proj.entropy = -1.0;
                        if (Utils.interrupt) {
                            return;
                        }
                        this.writtenGrid = filename;
                        if (this.is("pictures") && !gsfromfile) {
                            this.makePicture(f.getPath(), ss, X.testSamples, null);
                        }
                        this.makeExplain(explain, f, lambdafile, this.theSpecies + "_explain.bat", new File(this.environmentalLayers()).getAbsolutePath());
                        if (this.applyThresholdValue != -1.0 && !gsfromfile) {
                            new Threshold().applyThreshold(f.getPath(), this.applyThresholdValue);
                        }
                    } catch (IOException e6) {
                        this.popupError("Error writing output file " + new File(filename).getName(), e6);
                        return;
                    }
                }
                if (Utils.interrupt) {
                    return;
                }
                this.projectedGrids = new ArrayList();
                if (this.projectPrefix != null && this.is("outputGrids")) {
                    for (int j = 0; j < this.projectPrefix.length; ++j) {
                        if (Utils.interrupt) {
                            return;
                        }
                        String prefix = new File(this.projectPrefix[j]).getName();
                        if (prefix.endsWith(".csv")) {
                            prefix = prefix.substring(0, prefix.length() - 4);
                        }
                        final boolean isFile = new File(this.projectPrefix[j]).isFile();
                        final File ff = new File(this.outDir(), this.theSpecies + "_" + prefix + (isFile ? ".csv" : suffix));
                        final File ffclamp = new File(this.outDir(), this.theSpecies + "_" + prefix + "_clamping" + (isFile ? ".csv" : suffix));
                        try {
                            final Project proj2 = new Project(this.params);
                            proj2.needLayers = this.allLayers;
                            proj2.doProject(lambdafile, this.projectPrefix[j], ff.getPath(), this.cumulative(), false, this.is("writeClampGrid") ? ffclamp.getPath() : ((String) null));
                            if (Utils.interrupt) {
                                return;
                            }
                            this.projectedGrids.add("<a href = \"" + ff.getName() + "\">The model applied to the environmental layers in " + this.projectPrefix[j] + "</a>");
                            if (this.is("pictures") && !isFile) {
                                this.makePicture(ff.getPath(), ss, X.testSamples, this.projectPrefix[j]);
                                this.makeExplain(explain, ff, lambdafile, this.theSpecies + "_" + prefix + "_explain.bat", new File(this.projectPrefix[j]).getAbsolutePath());
                                if (this.is("writeClampGrid")) {
                                    this.makePicture(ffclamp.getPath(), new Sample[0], new Sample[0], this.projectPrefix[j], true);
                                }
                                this.makeNovel(baseFeaturesNoBias, this.projectPrefix[j], new File(this.outDir(), this.theSpecies + "_" + prefix + "_novel" + suffix).getPath());
                            }
                            if (this.applyThresholdValue != -1.0 && !isFile) {
                                new Threshold().applyThreshold(ff.getPath(), this.applyThresholdValue);
                            }
                        } catch (IOException e7) {
                            this.popupError("Error projecting", e7);
                            return;
                        }
                    }
                }
                if (Utils.interrupt) {
                    return;
                }
                try {
                    this.writeSampleAverages(baseFeaturesWithSamples, ss);
                } catch (IOException e8) {
                    this.popupError("Error writing file", e8);
                    return;
                }
                if (this.is("responsecurves")) {
                    try {
                        this.createProfiles(baseFeaturesWithSamples, lambdafile, ss);
                    } catch (IOException e8) {
                        this.popupError("Error writing response curves for " + this.theSpecies, e8);
                        return;
                    }
                }
                if (Utils.interrupt) {
                    return;
                }
                double[] permcontribs = null;
                try {
                    permcontribs = new PermutationImportance().go(baseFeatures, ss, lambdafile);
                } catch (IOException e6) {
                    this.popupError("Error computing permutation importance for " + this.theSpecies, e6);
                    return;
                }
                this.writeContributions(new double[][]{this.contributions, permcontribs}, "");
                final double[][] jackknifeGain = (double[][]) ((this.is("jackknife") && baseFeaturesNoBias.length > 1) ? this.jackknifeGain(baseFeaturesWithSamples, ss, X.testSamples, res.gain, testGain, auc) : null);
                this.writeSummary(res, testGain, auc, aucSD, trainauc, this.results, baseFeaturesNoBias, jackknifeGain, entropy, prevalence, permcontribs);
                this.writeHtmlDetails(res, testGain, auc, aucSD, trainauc);
                this.htmlout.close();
                this.maybeReplicateHtml(this.theSpecies, baseFeaturesNoBias);
            }
        }
        if (this.threads() > 1) {
            this.parallelRunner.close();
        }
    }
}
