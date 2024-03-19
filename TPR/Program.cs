namespace TPR;

class Program
{
    static void Main(string[] args)
    {
        var matrices = InitData();
        var size = matrices.Count;
        for (var i = 0; i < size; i++)
        {
            Console.WriteLine($"R{i + 1} -------------");
            MatrixSolver.Solve(matrices[i]);
            Console.WriteLine();
        }
        
        MatrixCombiner.UnionMatrices(matrices);
        MatrixCombiner.IntersectMatrices(matrices);
        MatrixCombiner.AdditionsAmongMatrices(matrices);
        MatrixCombiner.MatricesComposition(matrices);
        
        Console.WriteLine("R1 analysis:");
        var R1 = matrices[0];
        MatrixCombiner.BuildAlphaLevel(R1, 0.5);
        MatrixCombiner.BuildAlphaLevel(R1, 0.9);
        MatrixCombiner.BuildStrongAdvantageRelation(R1);
        MatrixCombiner.BuildIndifferenceRelation(R1);
        MatrixCombiner.BuildQuasiEquivalenceRelation(R1);
    }

    private static List<double[][]> InitData()
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
            }
        };

        return allData;
    }
}