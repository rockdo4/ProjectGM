public interface INode
{
    public enum EnemyState
    {
        Success,
        Running,
        Failure,
    }
    public EnemyState Evaluate();
    
}
