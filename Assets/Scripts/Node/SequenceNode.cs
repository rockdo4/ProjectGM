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
            return INode.EnemyState.Fail;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.EnemyState.Trace:
                    return INode.EnemyState.Trace;

                case INode.EnemyState.Attack:
                    continue;

                case INode.EnemyState.Fail:
                    return INode.EnemyState.Fail;
            }
        }

        return INode.EnemyState.Attack;
    }
}