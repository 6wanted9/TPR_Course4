namespace TPR;

public static class MatrixCombiner
{
    public static void UnionMatrices(List<double[][]> matrices)
    {
        Console.WriteLine("Union of the matrices:");
        ProcessMatrices(matrices, Math.Max).DisplayMatrix();
    }
    
    public static double[][] IntersectMatrices(List<double[][]> matrices)
    {
        Console.WriteLine("Intersect of the matrices:");
        var intersect = ProcessMatrices(matrices, Math.Min);
        intersect.DisplayMatrix();

        return intersect;
    }
    
    public static void AdditionsAmongMatrices(List<double[][]> matrices)
    {
        Console.WriteLine("Additions among matrices:");
        var intersect = ProcessMatrices(matrices, Math.Min);
        var matricesAmount = matrices.Count;
        for (var i = 0; i < matricesAmount; i++)
        {
            if (matrices[i].SequenceEqual(intersect))
            {
                Console.WriteLine($"Matrix {i + 1} is addition of matrix {matricesAmount - i}");
                return;
            }
        }
        
        Console.WriteLine("There's no additions.");
    }
    
    public static void MatricesComposition(List<double[][]> matrices)
    {
        Console.WriteLine("Matrices composition:");
        var size = matrices[0].Length;
        var result = new double[size][];
        for (var x = 0; x < size; x++)
        {
            result[x] = new double[size];
            for (var z = 0; z < size; z++)
            {
                var minimums = new List<double>();
                for (var y = 0; y < size; y++)
                {
                    minimums.Add(Math.Min(matrices.First()[x][y], matrices.Last()[y][z]));
                }
                
                result[x][z] = minimums.Max();
            }
        }
     
        result.DisplayMatrix();
    }
    
    public static void BuildAlphaLevel(double[][] matrix, double alpha)
    {
        Console.WriteLine($"Alpha-level with alpha = {alpha:0.00}");
        var alphaLevel = matrix.Select(row => row.Select(value => value >= alpha ? 1 : 0).ToArray()).ToArray();
        alphaLevel.DisplayMatrix();
    }
    
    public static double[][] BuildStrongAdvantageRelation(double[][] matrix)
    {
        Console.WriteLine($"Strong advantage relation");
        var relation = matrix.Select((row, x) => row.Select((value, y) =>
        {
            var diffWithMirrorValue = value - matrix[y][x];
            return diffWithMirrorValue > 0 ? diffWithMirrorValue : 0;
        }).ToArray()).ToArray();
        
        relation.DisplayMatrix();

        return relation;
    }
    
    public static void BuildIndifferenceRelation(double[][] matrix)
    {
        Console.WriteLine($"Indifference relation");
        var relation = matrix.Select((row, x) => row.Select((value, y) =>
        {
            var mirrorValue = matrix[y][x];
            return Math.Max(Math.Min(1 - value, 1 - mirrorValue), Math.Min(value, mirrorValue));
        }).ToArray()).ToArray();
        
        relation.DisplayMatrix();
    }
    
    public static void BuildQuasiEquivalenceRelation(double[][] matrix)
    {
        Console.WriteLine($"Quasi equivalence relation");
        var relation = matrix.Select((row, x) => row.Select((value, y) => Math.Min(value, matrix[y][x])).ToArray()).ToArray();
        
        relation.DisplayMatrix();
    }
    
    public static List<double> GetMaxValuesInEachColumn(double[][] matrix)
    {
        var maxColumnValues = new List<double>();
        var size = matrix.Length;
        for (var y = 0; y < size; y++)
        {
            double max = 0;
            for (var x = 0; x < size; x++)
            {
                var currentValueInColumn = matrix[x][y];
                if (currentValueInColumn > max)
                {
                    max = currentValueInColumn;
                }
            }

            maxColumnValues.Add(max);
        }


        return maxColumnValues;
    }

    public static IEnumerable<double> BuildFuzzySubset(List<double> relationMaxColumnsValues)
    {
        return relationMaxColumnsValues.Select(value => 1 - value);
    }
    
    private static double[][] ProcessMatrices(List<double[][]> matrices, Func<double, double, double> comparator)
    {
        var size = matrices[0].Length;
        var result = new double[size][];
        for (var i = 0; i < size; i++)
        {
            result[i] = new double[size];
            for (var j = 0; j < size; j++)
            {
                result[i][j] = CompareAllMatrices(matrices, comparator, i, j);
            }
        }
        
        return result;
    }

    private static double CompareAllMatrices(List<double[][]> matrices, Func<double, double, double> comparator, int x, int y)
    {
        double? value = null;
        foreach (var matrix in matrices)
        {
            value = value.HasValue ? comparator(value.Value, matrix[x][y]) : matrix[x][y];
        }

        return value.Value;
    }
}