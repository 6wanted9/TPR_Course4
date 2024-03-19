namespace TPR;

public class MatrixPropertyExistenceWithReason
{
    public MatrixPropertyExistenceWithReason()
    {
        
    }
    
    public MatrixPropertyExistenceWithReason(bool fulfilled, string? reason = null)
    {
        Fulfilled = fulfilled;
        Reason = reason;
    }
    public bool Fulfilled { get; set; }
    public string? Reason { get; set; }
}