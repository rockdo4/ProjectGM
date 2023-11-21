public interface INode
{
    public enum EnemyState
    {
        Idle,
        Attack,
        Trace,
        Die,
        Fail,
        Groggy,
    }

    public EnemyState Evaluate();
    
}
