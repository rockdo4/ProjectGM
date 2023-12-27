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
            return INode.EnemyState.Failure;

        foreach (var child in childs)
        {
            switch (child.Evaluate())
            {
                case INode.EnemyState.Running:
                    return INode.EnemyState.Running;

                case INode.EnemyState.Success:
                    return INode.EnemyState.Success;
            }
        }

        return INode.EnemyState.Failure;
    }
}