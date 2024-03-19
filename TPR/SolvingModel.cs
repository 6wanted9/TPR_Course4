namespace TPR;

public class SolvingModel
{
    private readonly Dictionary<MatrixProperty, MatrixPropertyExistenceWithReason> _properties = Enum
        .GetValues<MatrixProperty>()
        .Select(property => new KeyValuePair<MatrixProperty, MatrixPropertyExistenceWithReason>(property, new MatrixPropertyExistenceWithReason()))
        .ToDictionary();
    
    public MatrixPropertyExistenceWithReason this[MatrixProperty propertyType]
    {
        get => GetPropertyByType(propertyType);
        init
        {
            var matrixPropertyExistenceWithReason = GetPropertyByType(propertyType);
            matrixPropertyExistenceWithReason.Fulfilled = value.Fulfilled;
            matrixPropertyExistenceWithReason.Reason = value.Reason;
        }
    }

    private MatrixPropertyExistenceWithReason GetPropertyByType(MatrixProperty type)
    {
        return _properties.TryGetValue(type, out var property)
            ? property
            : throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }
}