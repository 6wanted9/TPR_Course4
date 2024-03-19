namespace TPR;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=================== Task 1 ===================");
        var relations = InitRelations();
        var firstTwoRelations = relations.Take(2).ToList();
        var size = firstTwoRelations.Count;
        for (var i = 0; i < size; i++)
        {
            Console.WriteLine($"R{i + 1} -------------");
            MatrixSolver.Solve(firstTwoRelations[i]);
            Console.WriteLine();
        }
        
        MatrixCombiner.UnionMatrices(firstTwoRelations);
        MatrixCombiner.IntersectMatrices(firstTwoRelations);
        MatrixCombiner.AdditionsAmongMatrices(firstTwoRelations);
        MatrixCombiner.MatricesComposition(firstTwoRelations);
        
        Console.WriteLine("R1 analysis:");
        var R1 = firstTwoRelations[0];
        MatrixCombiner.BuildAlphaLevel(R1, 0.5);
        MatrixCombiner.BuildAlphaLevel(R1, 0.9);
        var strongAdvantageRelation = MatrixCombiner.BuildStrongAdvantageRelation(R1);
        MatrixCombiner.BuildIndifferenceRelation(R1);
        MatrixCombiner.BuildQuasiEquivalenceRelation(R1);
        
        Console.WriteLine("=================== Task 2 ===================");
        Console.WriteLine($"R:");
        R1.DisplayMatrix();
        
        Console.WriteLine($"R^S:");
        strongAdvantageRelation.DisplayMatrix();
        
        Console.WriteLine("max{UR^S(uj, ui)}:");
        var relationMaxColumnsValues = MatrixCombiner.GetMaxValuesInEachColumn(strongAdvantageRelation);
        MatrixUtils.DisplayMatrix([relationMaxColumnsValues]);
        
        Console.WriteLine("UR^nd(ui):");
        var fuzzySubset = MatrixCombiner.BuildFuzzySubset(relationMaxColumnsValues);
        MatrixUtils.DisplayMatrix([fuzzySubset]);

        var maxFuzzySubsetValue = fuzzySubset.Max();
        Console.WriteLine($"max(UR^nd(ui)) = {maxFuzzySubsetValue:0.00}");

        var solution = MatrixCombiner.GetSolution(fuzzySubset, maxFuzzySubsetValue);
        Console.WriteLine($"u* = {solution}");
        
        Console.WriteLine("=================== Task 3 ===================");
        Console.WriteLine("P:");
        var P = MatrixCombiner.IntersectMatrices(relations, true);
        P.DisplayMatrix();
        
        Console.WriteLine("Q:");
        var weights = InitWeights();
        var Q = MatrixCombiner.BuildConvexConvolutionOfRelations(relations, weights);
        Q.DisplayMatrix();
        
        Console.WriteLine("PS:");
        var PS = MatrixCombiner.BuildStrongAdvantageRelation(P, true);
        PS.DisplayMatrix();
        
        Console.WriteLine("QS:");
        var QS = MatrixCombiner.BuildStrongAdvantageRelation(Q, true);
        QS.DisplayMatrix();
        
        Console.WriteLine("UP^nd(ui):");
        var psMaxColumnsValues = MatrixCombiner.GetMaxValuesInEachColumn(PS);
        var UPnd = MatrixCombiner.BuildFuzzySubset(psMaxColumnsValues).ToArray();
        MatrixUtils.DisplayMatrix([UPnd]);
        
        Console.WriteLine("UQ^nd(ui):");
        var qsMaxColumnsValues = MatrixCombiner.GetMaxValuesInEachColumn(QS);
        var UQnd = MatrixCombiner.BuildFuzzySubset(qsMaxColumnsValues).ToArray();
        MatrixUtils.DisplayMatrix([UQnd]);
        
        Console.WriteLine("U^nd(ui):");
        var Und = MatrixCombiner.IntersectVectors([UPnd, UQnd]);
        MatrixUtils.DisplayMatrix([Und]);
        
        var u = MatrixCombiner.GetSolution(Und, Und.Max());
        Console.WriteLine($"u* = {u}");

    }

    private static List<double[][]> InitRelations()
    {
        var allData = new List<double[][]>
        {
            new []
            {
                new []{ 0.4, 1,	0.9, 0.6, 1, 0.9 },
                new []{ 0.2, 0.7, 0.5, 0.1, 0.3, 0.8 },
                new []{ 0.3, 0.3, 0.2, 0.6, 0.9, 0.6 },
                new []{ 1, 1, 1, 0.3, 1, 0.8 },
                new []{ 0.1, 0, 1, 0.3, 0.8, 0.2 },
                new []{ 0.9, 0, 0.3, 0.8, 0.8, 0.1 },
            },
            new []
            {
                new []{ 0.7, 0.5, 0.7, 0.5, 0.2, 0.5 },
                new []{ 1, 1, 0.1, 0.6, 0.3, 0.7 },
                new []{ 0.4, 1, 1, 0.7, 0, 0.3 },
                new []{ 0.7, 0.9, 0.7, 0.9, 0.1, 0.6 },
                new []{ 0.1, 0.3, 0.7, 0.4, 0.7, 0.2 },
                new []{ 0.3, 0.3, 0.4, 0.4, 0.2, 0.4 },
            },
            new []
            {
                new []{ 0.6, 0.6, 0.5, 0.5, 0.5, 0.3 },
                new []{ 0.3, 0.4, 0.1, 0, 0.2, 0.2 },
                new []{ 0.5, 0, 0.1, 0.3, 0.9, 0.9 },
                new []{ 0.9, 0.5, 0.7, 0.6, 0.5, 0.5 },
                new []{ 0.6, 0.4, 0.8, 0.8, 0.8, 0.4 },
                new []{ 0.6, 0.3, 0.5, 0.8, 0.6, 0.8 },
            },
            new []
            {
                new []{ 0.9, 0.1, 0.2, 0.9, 0.2, 0.8 },
                new []{ 0.7, 0.6, 0.3, 1, 0.1, 0.7 },
                new []{ 0.6, 0.3, 1, 0, 0.1, 0.2 },
                new []{ 0.7, 0.7, 0.7, 0.4, 0.7, 0.3 },
                new []{ 0.6, 0.2, 0.6, 0.5, 0.4, 0.4 },
                new []{ 0.9, 0.8, 0.2, 0.1, 0.5, 0.8 }
            },
            new []
            {
                new []{ 0.8, 0.3, 0.3, 0.4, 0.5, 0.1 },
                new []{ 0.1, 0.1, 0.1, 0.7, 0.7, 0.8 },
                new []{ 0.5, 0.7, 0.7, 0, 0.7, 0.4 },
                new []{ 0.5, 0.3, 0.9, 0.1, 1, 0 },
                new []{ 0.8, 0.7, 0.4, 0.8, 0.4, 0.2 },
                new []{ 0.4, 0.9, 0.2, 0.6, 0.3, 0.1 }
            }
        };

        return allData;
    }
    
    private static List<double> InitWeights()
    {
        var allData = new List<double> { 0.2, 0.27, 0.21, 0.18, 0.14 };

        return allData;
    }
    
    private static List<double[][]> InitTestRelations()
    {
        var allData = new List<double[][]>
        {
            new []
            {
                new []{ 1, 1, 0.2, 0.4 },
                new []{ 0, 1, 0.8, 0.6 },
                new []{ 0.5, 0.5, 1, 0 },
                new []{ 0.5, 0.5, 1, 1 },
            },
            new []
            {
                new []{ 1, 0, 0.2, 0.9 },
                new []{ 1, 1, 0.9, 0.5 },
                new []{ 0.5, 0.5, 1, 1 },
                new []{ 0.5, 0.5, 0, 1 },
            },
            new []
            {
                new []{ 1, 0.3, 0.7, 1 },
                new []{ 0.5, 1, 1, 0.9 },
                new []{ 0.5, 0, 1, 0.1 },
                new []{ 0.5, 0.5, 0.5, 1 },
            },
            new []
            {
                new []{ 1, 0.2, 0.5, 0 },
                new []{ 0.5, 1, 0, 0.6 },
                new []{ 0.5, 1, 1, 0.8 },
                new []{ 1, 0.5, 0.5, 1 },
            },
            new []
            {
                new []{ 1, 0.1, 1, 0.6 },
                new []{ 0.5, 1, 0.3, 1 },
                new []{ 0, 0.5, 1, 0 },
                new []{ 0.5, 0, 0.5, 1 },
            },
        };

        return allData;
    }
    
    private static List<double> InitTestWeights()
    {
        var allData = new List<double> { 0.2, 0.2, 0.3, 0.2, 0.1 };

        return allData;
    }
}