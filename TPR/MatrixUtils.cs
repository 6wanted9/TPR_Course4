namespace TPR;

public static class MatrixUtils
{
    public static void DisplayMatrix(this IEnumerable<IEnumerable<double>> matrix)
    {
        DisplayMatrix(matrix, value => $"{value:0.00}");
    }
    
    public static void DisplayMatrix<T>(this IEnumerable<IEnumerable<T>> matrix, Func<T, string>? customParser = null)
    {
        foreach (var rowArray in matrix)
        {
            Console.WriteLine(string.Join("  ", customParser == null ? rowArray.Select(v => v.ToString()) : rowArray.Select(customParser)));
        }
        Console.WriteLine();
    }
}