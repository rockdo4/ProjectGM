using System;

public class ConditionNode : INode
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public INode.EnemyState Evaluate()
    {
        return condition() ? INode.EnemyState.Success : INode.EnemyState.Failure;
    }
}
