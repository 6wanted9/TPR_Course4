namespace TPR;

public static class MatrixSolver
{
    private const string NotBinaryClass = "Вiдношення не вiдноситься до жодного бiнарного класу.";
    
    public static void Solve(double[][] matrix)
    {
        matrix.DisplayMatrix(value => $"{value:0.00}");
        var comparisionMatrix = EvaluatComparisionMatrix(matrix);
        comparisionMatrix.DisplayMatrix(value => value == ComparisionMatrixValueType.None ? "-" : value.ToString());
        GetBinaryClass(comparisionMatrix, matrix);
        // if (binaryClass == NotBinaryClass)
        // {
        //     Console.WriteLine(NotBinaryClass);
        //     return;
        // }
        //
        // Console.WriteLine($"Вiдношення вiдноситься до класу: {binaryClass}");

        // Console.WriteLine("Оптимiзацiя за домiнуванням:");
        // Console.WriteLine(GetDominationResult(comparisionMatrix, matrix));
        // Console.WriteLine("Оптимiзацiя за блокуванням:");
        // Console.WriteLine(GetBlockingResult(comparisionMatrix, matrix));
    }

    private static string GetBlockingResult(ComparisionMatrixValueType[][] comparisionMatrix, int[][] initialMatrix)
    {
        if (comparisionMatrix.All(row => row.All(v => v != ComparisionMatrixValueType.I)))
        {
            var columnsWithMaxP = GetReversedMatrix(comparisionMatrix).Select((column, columnIndex) => new
            {
                ColumnIndex = columnIndex,
                IsMaxR = column.All(v => v != ComparisionMatrixValueType.P)
            }).Where(r => r.IsMaxR).Select(r => r.ColumnIndex + 1);
            
            return $"X0p = {{{SetExtensions.ToString(columnsWithMaxP)}}}";
        }

        var colsWithMaxR = GetReversedMatrix(initialMatrix).Select((col, colIndex) => new
        {
            ColIndex = colIndex,
            IsMaxR = col.All(v => v == 1)
        }).Where(r => r.IsMaxR).Select(r => r.ColIndex);
        
        if (colsWithMaxR.Count() != 1)
        {
            return $"X0R = {{{{{SetExtensions.GetEmptyValue()}}}}}, X00R = {{{SetExtensions.GetEmptyValue()}}}";
        }

        var columnWithMaxR = colsWithMaxR.Single();
        var rowsWithMaxRStrong = initialMatrix.Select((rowValues, rowIndex) => new
        {
            RowIndex = rowIndex,
            IsMaxRStrong = rowValues.Where((value, valueIndex) => valueIndex != columnWithMaxR).All(value => value == 0)
        }).Where(r => r.IsMaxRStrong).Select(pair => pair.RowIndex + 1);
        
        return $"X0R = {{{{{columnWithMaxR + 1}}}}}, X00R = {{{SetExtensions.ToString(rowsWithMaxRStrong)}}}";
    }
    
    private static string GetDominationResult(ComparisionMatrixValueType[][] comparisionMatrix, int[][] initialMatrix)
    {
        if (comparisionMatrix.All(row => row.All(v => v != ComparisionMatrixValueType.I)))
        {
            var rowsWithMaxP = comparisionMatrix.Select((row, rowIndex) => new
            {
                RowIndex = rowIndex,
                IsMaxR = row.Where((value, valueIndex) => valueIndex != rowIndex).All(v => v == ComparisionMatrixValueType.P)
            }).Where(r => r.IsMaxR).Select(r => r.RowIndex + 1);
            
            return $"X*p = {{{SetExtensions.ToString(rowsWithMaxP)}}}";
        }

        var rowsWithMaxR = initialMatrix.Select((row, rowIndex) => new
        {
            RowIndex = rowIndex,
            IsMaxR = row.All(v => v == 1)
        }).Where(r => r.IsMaxR).Select(r => r.RowIndex);
        
        if (rowsWithMaxR.Count() != 1)
        {
            return $"X*R = {{{{{SetExtensions.GetEmptyValue()}}}}}, X**R = {{{SetExtensions.GetEmptyValue()}}}";
        }

        var rowWithMaxR = rowsWithMaxR.Single();
        var columnsWithMaxRStrong = GetReversedMatrix(initialMatrix).Select((columnValueS, columnIndex) => new
        {
            ColumnIndex = columnIndex,
            IsMaxRStrong = columnValueS.Where((value, valueIndex) => valueIndex != rowWithMaxR).All(value => value == 0)
        }).Where(r => r.IsMaxRStrong).Select(pair => pair.ColumnIndex + 1);
        
        return $"X*R = {{{{{rowWithMaxR + 1}}}}}, X**R = {{{SetExtensions.ToString(columnsWithMaxRStrong)}}}";
    }

    private static string GetBinaryClass(ComparisionMatrixValueType[][] comparisionMatrix, double[][] initialMatrix)
    {
        var reflexityCheck = CheckMatrixValues(initialMatrix, true, false, false, false, 1);
        var antireflexityCheck = CheckMatrixValues(comparisionMatrix, true, false, false, false, ComparisionMatrixValueType.N);
        var connectivityCheck = CheckMatrixValues(comparisionMatrix, false, true, true, false, ComparisionMatrixValueType.N);
        var solutionModel = new SolvingModel
        {
            [MatrixProperty.Reflexity] = reflexityCheck,
            [MatrixProperty.StrongReflexity] = reflexityCheck.Fulfilled
                ? CheckMatrixValues(initialMatrix, false, true, true, false, 1)
                : new MatrixPropertyExistenceWithReason(false, "Reflexity is not fulfilled"),
            [MatrixProperty.Antireflexity] = antireflexityCheck,
            [MatrixProperty.StrongAntireflexity] = antireflexityCheck.Fulfilled
                ? CheckMatrixValues(initialMatrix, false, true, true, false, 0)
                : new MatrixPropertyExistenceWithReason(false, "Antireflexity is not fulfilled"),
            [MatrixProperty.Symmetry] = CheckMatrixValues(comparisionMatrix, true, false, false, false, ComparisionMatrixValueType.I, ComparisionMatrixValueType.N),
            [MatrixProperty.Asymmetry] = CheckMatrixValues(comparisionMatrix, true, true, false, false, ComparisionMatrixValueType.N, ComparisionMatrixValueType.P),
            [MatrixProperty.Antisymmetry] = CheckMatrixValues(comparisionMatrix, false, true, false, false, ComparisionMatrixValueType.N, ComparisionMatrixValueType.P),
            [MatrixProperty.Transitivity] = CheckTransitivnist(initialMatrix),
            [MatrixProperty.Connectivity] = connectivityCheck,
            [MatrixProperty.StrongConnectivity] = connectivityCheck.Fulfilled
                ? CheckMatrixValues(initialMatrix, true, true, false, true, 1)
                : new MatrixPropertyExistenceWithReason(false, "Connectivity is not fulfilled")
        };

        foreach (var propertyType in Enum.GetValues<MatrixProperty>())
        {
            var property = solutionModel[propertyType];
            var fulfilled = property.Fulfilled;
            Console.WriteLine($"{propertyType} IS{(fulfilled ? "" : " NOT")} fulfilled{(fulfilled ? "" : $", because {property.Reason}")}");
        }

        return solutionModel switch
        {
            not null when solutionModel.CheckMatrixPropertiesExistence(MatrixProperty.Reflexity, MatrixProperty.Symmetry, MatrixProperty.Transitivity) => "Еквiвалентнiсть",
            not null when solutionModel.CheckMatrixPropertiesExistence(MatrixProperty.Antireflexity, MatrixProperty.Transitivity) => "Строгого порядку",
            not null when solutionModel.CheckMatrixPropertiesExistence(MatrixProperty.Reflexity, MatrixProperty.Antisymmetry, MatrixProperty.Transitivity) => "Нестрогого порядку",
            not null when solutionModel.CheckMatrixPropertiesExistence(MatrixProperty.Reflexity, MatrixProperty.Transitivity) => "Квазiпорядок",
            _ => NotBinaryClass
        };
    }

    private static bool CheckMatrixPropertiesExistence(this SolvingModel solvingModel, params MatrixProperty[] properties)
    {
        return properties.All(property => solvingModel[property].Fulfilled);
    }
    
    private static MatrixPropertyExistenceWithReason CheckMatrixValues<TValue>(
        TValue[][] matrix,
        bool includeDiagonal = true,
        bool includeNonDiagonal = true,
        bool excludeTargetValues = false,
        bool includeMirrorValue = false,
        params TValue[] targetValues)
        where TValue: struct
    {
        var size = matrix.Length;
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (!(includeDiagonal || i != j) && (includeNonDiagonal || i == j))
                {
                    continue;
                }

                var targetValueBelongsToMatrix = targetValues.Contains(matrix[i][j]);
                if (!targetValueBelongsToMatrix && includeMirrorValue)
                {
                    targetValueBelongsToMatrix = targetValues.Contains(matrix[j][i]); 
                }
                
                var validBelongingCondition = excludeTargetValues
                    ? !targetValueBelongsToMatrix
                    : targetValueBelongsToMatrix;
                
                if (!validBelongingCondition)
                {
                    return new MatrixPropertyExistenceWithReason(false, $"Matrix[{i + 1}][{j + 1}] is {(excludeTargetValues ? "" : "not ")}one of target values:  {string.Join(',', targetValues.Select(t => t.ToString()))}");
                }
            }
        }
        
        return new MatrixPropertyExistenceWithReason(true);
    }

    private static MatrixPropertyExistenceWithReason CheckTransitivnist(double[][] matrix)
    {
        var size = matrix.Length;
        for (var x = 0; x < size; x++)
        {
            for (var z = 0; z < size; z++)
            {
                var matrixXZ = matrix[x][z];
                for (var y = 0; y < size; y++)
                {
                    if (matrixXZ < Math.Min(matrix[x][y], matrix[y][z]))
                    {
                        return new MatrixPropertyExistenceWithReason(false, $"Matrix[{x + 1}][{z + 1}] < Matrix[{x + 1}][{y + 1}] ^ Matrix[{y + 1}][{z + 1}]");
                    }
                }
            }
        }

        return new MatrixPropertyExistenceWithReason(true);
    }
    
    private static ComparisionMatrixValueType[][] EvaluatComparisionMatrix(double[][] matrix)
    {
        var size = matrix.Length;
        var solution = new ComparisionMatrixValueType[size][];
        for (var i = 0; i < size; i++)
        {
            solution[i] = new ComparisionMatrixValueType[size];
            for (var j = 0; j < size; j++)
            {
                solution[i][j] = EvaluateComparisionFieldValue(matrix, i, j);
            }
        }
        return solution;
    }

    private static ComparisionMatrixValueType EvaluateComparisionFieldValue(double[][] matrix, int x, int y)
    {
        return matrix[x][y] switch
        {
            var value and > 0 when matrix[y][x] == value => ComparisionMatrixValueType.I,
            0 when matrix[y][x] == 0 => ComparisionMatrixValueType.N,
            > 0 when matrix[y][x] == 0 => ComparisionMatrixValueType.P,
            0 when matrix[y][x] > 0 => ComparisionMatrixValueType.P,
            _ => ComparisionMatrixValueType.None
        };
    }

    private static T[][] GetReversedMatrix<T>(T[][] matrix)
    {
        var size = matrix.Length;
        var reversedMatrix = new T[size][];
        for (var i = 0; i < size; i++)
        {
            reversedMatrix[i] = new T[size];
            for (var j = 0; j < size; j++)
            {
                reversedMatrix[i][j] = matrix[j][i];
            }
        }

        return reversedMatrix;
    }
}