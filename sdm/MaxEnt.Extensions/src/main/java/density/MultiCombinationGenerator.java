/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package density;

import java.math.BigInteger;
import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author swbosch
 */
public final class MultiCombinationGenerator {

    private BigInteger numLeft;
    private BigInteger total;
    private CombinationGenerator groupsGenerator;
    private List<String[]> correlationGroups;
    private int subsetLength;
    private List<String[]> correlationGroupsUsed;
    private String[] combination;
    int currentGroup = 0;
    int[] groupIndices;

    public MultiCombinationGenerator(List<String[]> correlationGroups, int subsetLength) {
        int n = correlationGroups.size();
        int r = subsetLength;
        if (r > n) {
            throw new IllegalArgumentException();
        }
        if (n < 1) {
            throw new IllegalArgumentException();
        }

        this.subsetLength = subsetLength;
        this.correlationGroups = correlationGroups;
        reset();
    }

    public void reset() {
        this.groupsGenerator = new CombinationGenerator(correlationGroups.size(), subsetLength);
        setNextCorrelationGroupsUsed();
        this.numLeft = getTotal();
    }

    private void setNextCorrelationGroupsUsed() {
        correlationGroupsUsed = null;
        this.currentGroup = 0;
        if (this.groupsGenerator.hasMore()) {
            correlationGroupsUsed = new ArrayList<String[]>(subsetLength);
            for (int i : this.groupsGenerator.getNext()) {
                correlationGroupsUsed.add(correlationGroups.get(i));
            }
            this.combination = new String[subsetLength];
            this.groupIndices = new int[subsetLength];
            for (int i = 0; i < subsetLength; i++) {
                groupIndices[i] = 0;
                combination[i] = correlationGroupsUsed.get(i)[0];
            }
            groupIndices[0] = -1; // makes logic in next easier
        }
    }

    public BigInteger getNumLeft() {
        return this.numLeft;
    }

    public boolean hasMore() {
        return numLeft.compareTo(BigInteger.ZERO) == 1;
    }

    public BigInteger getTotal() {
        if (total == null) {
            total = BigInteger.ZERO;
            int[] groupLengths = new int[correlationGroups.size()];
            for (int i = 0; i < groupLengths.length; i++) {
                groupLengths[i] = correlationGroups.get(i).length;
            }
            // in R: sum(apply(combn(group.lengths, 8), MARGIN=2, prod))
            CombinationGenerator gen = new CombinationGenerator(groupLengths.length, subsetLength);
            while (gen.hasMore()) {
                int subTotal = 1;
                int[] ind = gen.getNext();
                for (int i : ind) {
                    subTotal *= groupLengths[i];
                }
                total = total.add(BigInteger.valueOf(subTotal));
            }
        }
        return total;
    }

    public void increaseCurrentGroup() {
        currentGroup = currentGroup + 1;
        if (this.currentGroup >= this.subsetLength) {
            // move to the next combination of correlation groups
            setNextCorrelationGroupsUsed();
        }
        // move next group index one up and reset all previous indices to zero and start again from the beginning
        groupIndices[currentGroup] = groupIndices[currentGroup] + 1;
        if (groupIndices[currentGroup] < correlationGroupsUsed.get(currentGroup).length) {
            combination[currentGroup] = correlationGroupsUsed.get(currentGroup)[groupIndices[currentGroup]];
            for (int j = 0; j < currentGroup; j++) {
                groupIndices[j] = 0;
                combination[j] = correlationGroupsUsed.get(j)[0];
            }
            currentGroup = 0;
        }
        else{
            increaseCurrentGroup();
        }
    }

    public String[] getNext() {
        /*
         High level flow:
         - generate all combinations with strings in first group and the first record in all the other groups
         - then move the second string in the second group and re-create all combinations with variables in the first group
         - then move to the thrid string in the second group  and repeat
         */

        // go to next variable within the current correlation group
        groupIndices[currentGroup] = groupIndices[currentGroup] + 1;
        if (groupIndices[currentGroup] < correlationGroupsUsed.get(currentGroup).length) {
            combination[currentGroup] = correlationGroupsUsed.get(currentGroup)[groupIndices[currentGroup]];
        } else {
            increaseCurrentGroup();
        }
        numLeft = numLeft.subtract(BigInteger.ONE);
        return combination;

    }
}
