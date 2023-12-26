public class InverterNode : INode
{
    private INode child;

    public InverterNode(INode child)
    {
        this.child = child;
    }

    public INode.EnemyState Evaluate()
    {
        INode.EnemyState result = child.Evaluate();

        switch (result)
        {
            case INode.EnemyState.Success:
                return INode.EnemyState.Failure;

            case INode.EnemyState.Failure:
                return INode.EnemyState.Success;

            default:
                return result;
        }
    }
}
