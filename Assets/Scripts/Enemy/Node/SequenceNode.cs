using System.Collections.Generic;

public sealed class SequenceNode : INode
{
    List<INode> _childs;

    public SequenceNode(List<INode> childs)
    {
        _childs = childs;
    }

    public INode.EnemyState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return INode.EnemyState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.EnemyState.Running:
                    return INode.EnemyState.Running;

                case INode.EnemyState.Success:
                    continue;

                case INode.EnemyState.Failure:
                    return INode.EnemyState.Failure;
            }
        }

        return INode.EnemyState.Success;
    }
}