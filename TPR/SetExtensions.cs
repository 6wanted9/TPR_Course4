namespace TPR;

public static class SetExtensions
{
    private const string EmptySign = "∅";

    public static string ToString(this IEnumerable<int> set)
    {
        return !set.Any()
            ? EmptySign
            : string.Join(", ", set);
    }

    public static string GetEmptyValue() => EmptySign;
}