using System.Collections.Generic;

public sealed class SelectorNode : INode
{
    List<INode> childs;

    public SelectorNode(List<INode> childs)
    {
        this.childs = childs;
    }
    public INode.EnemyState Evaluate()
    {
        if (childs == null)
            return INode.EnemyState.Fail;

        foreach (var child in childs)
        {
            switch (child.Evaluate())
            {
                case INode.EnemyState.Trace:
                    return INode.EnemyState.Trace;

                case INode.EnemyState.Attack:
                    return INode.EnemyState.Attack;
            }
        }

        return INode.EnemyState.Fail;
    }
}