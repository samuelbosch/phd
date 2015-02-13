package density;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Locale;

public class RunnerExtended {

    GridSet gs;
    SampleSet sampleSet;
    SampleSet testSampleSet = null;
    String[] projectPrefix = null;
    Params params;
    CsvWriter results;
    GUI gui = null;
    String theSpecies;
    NumberFormat nf = NumberFormat.getNumberInstance(Locale.US);
    ArrayList projectedGrids;
    String writtenGrid;
    double aucmax;
    double applyThresholdValue;
    boolean samplesAddedToFeatures;
    String raw2cumfile = null;
    ParallelRun parallelRunner;
    double[][] coords;
    HashMap<String, Integer> speciesCount;

// <editor-fold defaultstate="collapsed" desc="utility functions">
    boolean is(String s) {
        return this.params.getboolean(s);
    }

    boolean logistic() {
        return this.params.logistic();
    }

    boolean cumulative() {
        return this.params.cumulative();
    }

    int replicates() {
        return this.params.getint("replicates");
    }

    int replicates(String species) {
        if ((this.speciesCount == null) || (!cv()) || (this.speciesCount.get(species) == null) || (((Integer) this.speciesCount.get(species)).intValue() > replicates())) {
            return replicates();
        }
        return ((Integer) this.speciesCount.get(species)).intValue();
    }

    int threads() {
        return this.params.getint("threads");
    }

    String outDir() {
        return this.params.getString("outputdirectory");
    }

    String biasFile() {
        return this.params.getString("biasFile");
    }

    String testSamplesFile() {
        return this.params.getString("testSamplesFile");
    }

    String environmentalLayers() {
        return this.params.getString("environmentalLayers");
    }

    String projectionLayers() {
        return this.params.getString("projectionLayers");
    }

    double betaMultiplier() {
        return this.params.getdouble("betaMultiplier");
    }

    String outputFileType() {
        return "." + this.params.getString("outputFileType");
    }

    boolean cv() {
        return this.params.getString("replicatetype").equals("crossvalidate");
    }

    boolean bootstrap() {
        return this.params.getString("replicatetype").equals("bootstrap");
    }

    boolean subsample() {
        return this.params.getString("replicatetype").equals("subsample");
    }

    static String raw2cumfile(String lambdafile) {
        return lambdafile.replaceAll(".lambdas$", "_omission.csv");
    }
// </editor-fold>

    public static class MaxentRunResults {

        double gain;
        double time;
        int iterations;
        FeaturedSpace X;
        String[] featureTypes;

        void removeBiasDistribution() {
            if (this.X.biasDist != null) {
                this.X.setBiasDist(null);
            }
        }

        MaxentRunResults(double gain, int iter, FeaturedSpace X, double time, String[] types) {
            this.gain = gain;
            this.iterations = iter;
            this.X = X;
            this.time = time;
            this.featureTypes = types;
        }
    }

    MaxentRunResults maxentRun(Feature[] features, Sample[] ss, Sample[] testss) {
        autoSetBeta(features, ss.length);
        for (int j = 0; j < features.length; j++) {
            features[j].setLambda(0.0D);
        }
        for (int j = 0; j < features.length; j++) {
            features[j].setActive(true);
        }
        if (is("autofeature")) {
            autoSetActive(features, ss.length);
        }
        HashSet<String> types = new HashSet();
        for (int j = 0; j < features.length; j++) {
            int type = features[j].type();
            if ((type == 1) && (!is("linear"))) {
                features[j].setActive(false);
            }
            if ((features[j].isActive()) && (recordTypeName(type) != null)) {
                types.add(recordTypeName(type));
            }
        }
        int cnt = 0;
        for (int i = 0; i < features.length; i++) {
            if (features[i].isActive()) {
                cnt++;
            }
        }
        if ((cnt == 0) || ((cnt == 1) && (!biasFile().equals("")))) {
            popupError("No features available: select more feature types or deselect auto features", null);
            Utils.interrupt = true;
            return null;
        }
        FeaturedSpace X = new FeaturedSpace(ss, features);
        X.setXY(this.coords);
        if (is("biasIsBayesianPrior")) {
            X.biasIsBayesianPrior = true;
        }
        if (testss != null) {
            X.recordTestSamples(testss);
        }
        for (int j = 0; j < features.length; j++) {
            if ((features[j].isBinary()) && (features[j].sampleExpectation == 0.0D)) {
                Utils.echoln("Deactivating " + features[j].name);
                features[j].setActive(false);
            }
        }
        Utils.reportMemory("FeaturedSpace");

        Sequential alg = new Sequential(X, this.params);
        alg.setParallelUpdateFrequency(this.params.getint("parallelUpdateFrequency"));
        if (Utils.interrupt) {
            return null;
        }
        double gain = Math.log(X.bNumPoints()) - alg.run();
        determineContributions(X);
        if (Utils.interrupt) {
            return null;
        }
        return new MaxentRunResults(gain, alg.iteration, X, alg.getTime(), (String[]) types.toArray(new String[0]));
    }
    int TAREA = 0;
    int TTRAINO = 1;
    int TTESTO = 2;
    int TCUM = 3;
    int TLOGISTIC = 4;
    int TTHRESH = 5;
    double beta_lqp;
    double beta_thr;
    double beta_hge;
    double beta_cat;

    static double interpolate(int[] x, double[] y, int xx) {
        int i;
        for (i = 0; i < x.length; i++) {
            if (xx <= x[i]) {
                break;
            }
        }
        if (i == 0) {
            return y[0];
        }
        if (i == x.length) {
            return y[(x.length - 1)];
        }
        return y[(i - 1)] + (y[i] - y[(i - 1)]) * (xx - x[(i - 1)]) / (x[i] - x[(i - 1)]);
    }
    String regularizationConstants()
   {
     return "linear/quadratic/product: " + this.nf.format(this.beta_lqp) + ", categorical: " + this.nf.format(this.beta_cat) + ", threshold: " + this.nf.format(this.beta_thr) + ", hinge: " + this.nf.format(this.beta_hge);
   }
    void autoSetBeta(Feature[] features, int numSamples) {
        int[] thresholds = null;
        double[] betas = null;
        if ((is("product")) && ((!is("autofeature")) || (numSamples >= this.params.getint("lq2lqptThreshold")))) {
            thresholds = new int[]{0, 10, 17, 30, 100};
            betas = new double[]{2.6D, 1.6D, 0.9D, 0.55D, 0.05D};
        } else if ((is("quadratic")) && ((!is("autofeature")) || (numSamples >= this.params.getint("l2lqThreshold")))) {
            thresholds = new int[]{0, 10, 17, 30, 100};
            betas = new double[]{1.3D, 0.8D, 0.5D, 0.25D, 0.05D};
        } else {
            thresholds = new int[]{10, 30, 100};
            betas = new double[]{1.0D, 0.2D, 0.05D};
        }
        this.beta_lqp = interpolate(thresholds, betas, numSamples);
        this.beta_thr = interpolate(new int[]{0, 100}, new double[]{2.0D, 1.0D}, numSamples);

        this.beta_hge = 0.5D;
        if (is("doSqrtCat")) {
            this.beta_cat = interpolate(new int[]{10, 17, 30}, new double[]{0.2D, 0.1D, 0.05D}, numSamples);

            this.beta_cat = Math.sqrt(this.beta_lqp * this.beta_cat);
        } else {
            this.beta_cat = interpolate(new int[]{0, 10, 17}, new double[]{0.65D, 0.5D, 0.25D}, numSamples);
        }
        if (this.params.getdouble("beta_categorical") >= 0.0D) {
            this.beta_cat = this.params.getdouble("beta_categorical");
        }
        if (this.params.getdouble("beta_threshold") >= 0.0D) {
            this.beta_thr = this.params.getdouble("beta_threshold");
        }
        if (this.params.getdouble("beta_hinge") >= 0.0D) {
            this.beta_hge = this.params.getdouble("beta_hinge");
        }
        if (this.params.getdouble("beta_lqp") >= 0.0D) {
            this.beta_lqp = this.params.getdouble("beta_lqp");
        }
        for (int i = 0; i < features.length; i++) {
            if ((features[i] instanceof BinaryFeature)) {
                features[i].setBeta(this.beta_cat * betaMultiplier());
            } else if ((features[i] instanceof ThrGeneratorFeature)) {
                features[i].setBeta(this.beta_thr * betaMultiplier());
            } else if ((features[i] instanceof HingeGeneratorFeature)) {
                features[i].setBeta(this.beta_hge * betaMultiplier());
            } else {
                features[i].setBeta(this.beta_lqp * betaMultiplier());
                if (this.params.betaMap != null) {
                    String sval = (String) this.params.betaMap.get(features[i].name);
                    if (sval != null) {
                        features[i].setBeta(Double.parseDouble(sval));
                        Utils.echoln("Setting beta for " + features[i].name + " to " + sval);
                    }
                }
            }
        }
        Utils.echoln("Regularization values: " + regularizationConstants());
    }

    void autoSetActive(Feature[] features, int numSamples) {
        for (int i = 0; i < features.length; i++) {
            switch (features[i].type()) {
                case 4:
                    if (numSamples < this.params.getint("lq2lqptThreshold")) {
                        features[i].setActive(false);
                    }
                    break;
                case 11:
                    if (numSamples < this.params.getint("hingeThreshold")) {
                        features[i].setActive(false);
                    }
                    break;
                case 3:
                    if (numSamples < this.params.getint("lq2lqptThreshold")) {
                        features[i].setActive(false);
                    }
                    break;
                case 2:
                    if (numSamples < this.params.getint("l2lqThreshold")) {
                        features[i].setActive(false);
                    }
                    break;
            }
        }
    }

    String recordTypeName(int t) {
        String[] typenames = {"linear", "quadratic", "product", "threshold", "hinge"};
        int[] types = {1, 2, 3, 4, 11};
        for (int i = 0; i < types.length; i++) {
            if (types[i] == t) {
                return typenames[i];
            }
        }
        return null;
    }

    void popupError(String s, Throwable e) {
        Utils.popupError(s, e);
    }
    double[] contributions;

    void determineContributions(FeaturedSpace X) {
        if (this.contributions != null) {
            return;
        }
        String[] names = this.params.layers;
        this.contributions = new double[names.length];
        for (int i = 0; i < X.features.length; i++) {
            double contrib = X.features[i].contribution;
            if (contrib > 0.0D) {
                String featureName = X.features[i].name.replaceFirst("\\(", "").replaceFirst("=.*\\)", "").replaceFirst("\\^2", "").replaceFirst("'", "").replaceFirst("`", "").replaceFirst(".*<", "").replaceFirst("\\)", "").replaceFirst("__rev", "");
                String[] ff = featureName.split("\\*");
                for (int k = 0; k < ff.length; k++) {
                    int j;
                    for (j = 0; j < names.length; j++) {
                        if (names[j].equals(ff[k])) {
                            this.contributions[j] += contrib / ff.length;
                            break;
                        }
                    }
                    if (j == names.length) {
                        Utils.echoln("Contribution not found: " + featureName);
                    }
                }
            }
        }
        double sum = 0.0D;
        for (int j = 0; j < names.length; j++) {
            sum += this.contributions[j];
        }
        for (int j = 0; j < names.length; j++) {
            this.contributions[j] = (sum == 0.0D ? 0.0D : this.contributions[j] * 100.0D / sum);
        }
    }

}
