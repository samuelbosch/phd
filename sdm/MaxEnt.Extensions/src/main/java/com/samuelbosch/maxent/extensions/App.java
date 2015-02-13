package com.samuelbosch.maxent.extensions;

import density.MultiCombinationGenerator;
import density.MultivariableRunner;
import java.util.ArrayList;

public class App {

    public static void main(String[] args) {
        runMaxentMultiVar(args);
        //testMaxentAtuleMate();
        //testMultiCombinationGenerator();
        //testMaxent();
        //System.out.println( "Hello World!" );
    }

    public static void runMaxentMultiVar(String[] args) {
        if (args.length != 4) {
            System.out.println("USAGE");
            System.out.println("java -jar Maxent.Extensions.jar <subsetSize> <samplesfile> <backgroundfile> <outputDir>");
        } else {
            int subsetSize = Integer.parseInt(args[0]);
            String samplesFile = args[1];
            String backgroundFile = args[2];
            String outputDir = args[3];
            MultivariableRunner r = new MultivariableRunner(samplesFile, backgroundFile, outputDir);
            // 95% correlation groups
            ArrayList<String[]> correlationGroups = new ArrayList<String[]>();
            correlationGroups.add(new String[]{"alk_mean", "biogeo08_sss_mean_5m", "biogeo09_sss_min_5m", "biogeo11_sss_range_5m", "biogeo12_sss_variance_5m", "sal_mean_an"});
            correlationGroups.add(new String[]{"calcite_mean"});
            correlationGroups.add(new String[]{"chlo_max", "chlo_mean", "chlo_min", "chlo_range", "da_mean", "da_min", "eudepth_mean"});
            correlationGroups.add(new String[]{"CloudFr_max", "CloudFr_mean", "CloudFr_min", "SolIns_max", "SolIns_mean", "SolIns_min", "nitrate_mean", "par_max", "par_mean", "par_min", "phos_mean", "silica_mean", "sst_mean"});
            correlationGroups.add(new String[]{"ph_mean"});
            correlationGroups.add(new String[]{"biogeo16_sst_range_5m", "biogeo17_sst_variance_5m", "sst_range"});
            correlationGroups.add(new String[]{"waveheight"});
            correlationGroups.add(new String[]{"bathy_5m"});
            correlationGroups.add(new String[]{"biogeo01_aspect_EW_5m"});
            correlationGroups.add(new String[]{"biogeo02_aspect_NS_5m"});
            correlationGroups.add(new String[]{"biogeo03_plan_curvature_5m"});
            correlationGroups.add(new String[]{"biogeo04_profile_curvature_5m", "biogeo07_concavity_5m"});
            correlationGroups.add(new String[]{"biogeo05_dist_shore_5m"});
            correlationGroups.add(new String[]{"biogeo06_bathy_slope_5m"});
            r.run(correlationGroups, subsetSize);
        }
    }

    public static void testMaxentAtuleMate() {
        String samplesFile = "D:\\temp\\hpc\\obis\\species\\Atule_mate.csv";
        String backgroundFile = "D:\\temp\\hpc\\obis\\background_biooracle_marspec_values.csv";
        String outputDir = "D:\\temp\\hpc\\obis\\output";
        MultivariableRunner r = new MultivariableRunner(samplesFile, backgroundFile, outputDir);
        // 95% correlation groups
        ArrayList<String[]> correlationGroups = new ArrayList<String[]>();
        //correlationGroups.add(new String[]{"alk_mean", "biogeo08_sss_mean_5m", "biogeo09_sss_min_5m", "biogeo11_sss_range_5m", "biogeo12_sss_variance_5m", "sal_mean_an"});
        correlationGroups.add(new String[]{"calcite_mean"});
        //correlationGroups.add(new String[]{"chlo_max", "chlo_mean", "chlo_min", "chlo_range", "da_mean", "da_min", "eudepth_mean"});
        //correlationGroups.add(new String[]{"CloudFr_max", "CloudFr_mean", "CloudFr_min", "SolIns_max", "SolIns_mean", "SolIns_min", "nitrate_mean", "par_max", "par_mean", "par_min", "phos_mean", "silica_mean", "sst_mean"});
        correlationGroups.add(new String[]{"ph_mean"});
        correlationGroups.add(new String[]{"biogeo16_sst_range_5m", "biogeo17_sst_variance_5m", "sst_range"});
        correlationGroups.add(new String[]{"waveheight"});
        correlationGroups.add(new String[]{"bathy_5m"});
        correlationGroups.add(new String[]{"biogeo01_aspect_EW_5m"});
        correlationGroups.add(new String[]{"biogeo02_aspect_NS_5m"});
        correlationGroups.add(new String[]{"biogeo03_plan_curvature_5m"});
        correlationGroups.add(new String[]{"biogeo04_profile_curvature_5m", "biogeo07_concavity_5m"});
        correlationGroups.add(new String[]{"biogeo05_dist_shore_5m"});
        correlationGroups.add(new String[]{"biogeo06_bathy_slope_5m"});
        r.run(correlationGroups, 2);
    }

    public static void testMaxent() {
        String samplesFile = "D:\\temp\\maxentplus\\Acantholumpenus_mackayi.csv";
        String backgroundFile = "D:\\temp\\maxentplus\\background.csv";
        String outputDir = "D:\\temp\\maxentplus\\output";
        MultivariableRunner r = new MultivariableRunner(samplesFile, backgroundFile, outputDir);
        // TODO: FILL in correlation groups
        ArrayList<String[]> correlationGroups = new ArrayList<String[]>();
        correlationGroups.add(new String[]{"chlo_mean", "da_mean"});
        correlationGroups.add(new String[]{"silica_mean", "phos_mean"});
        correlationGroups.add(new String[]{"dissox_mean"});
//        this.correlationGroups = new ArrayList<String[]>();
//        this.correlationGroups.add(new String[]{"chlo_mean", "da_mean"});
//        this.correlationGroups.add(new String[]{"silica_mean", "phos_mean", "dissox_mean"});
        r.run(correlationGroups, correlationGroups.size());
    }

    public static void testMultiCombinationGenerator() {
        ArrayList<String[]> groups = new ArrayList<String[]>();
        groups.add(new String[]{"a", "b"});
        groups.add(new String[]{"c"});
        groups.add(new String[]{"e", "f", "g"});
        MultiCombinationGenerator m = new MultiCombinationGenerator(groups, 2);
        System.out.println(m.getTotal());
        while (m.hasMore()) {
            for (String s : m.getNext()) {
                System.out.print(s);
                /* should print */
                //ac bc ae be af bf ag bg ce cf cg
            }
            System.out.print(" ");
        }
        System.out.println();
    }
}
