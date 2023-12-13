using System;

public class DecoratorNode : INode
{
    private Func<bool> condition;
    private INode child;

    public DecoratorNode(Func<bool> condition, INode child)
    {
        this.condition = condition;
        this.child = child;
    }

    public INode.EnemyState Evaluate()
    {
        if (condition())
        {
            return child.Evaluate();
        }
        return INode.EnemyState.Failure;
    }
}
