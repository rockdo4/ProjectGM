using System.Collections;
using UnityEngine;


public class WaitNode : INode
{
    private float waitTime;
    private float startTime = -1;

    public WaitNode(float time)
    {
        this.waitTime = time;
    }

    public INode.EnemyState Evaluate()
    {
        if (startTime < 0)
            startTime = Time.time;

        if (Time.time - startTime > waitTime)
        {
            startTime = -1;
            return INode.EnemyState.Success;
        }
        else
        {
            return INode.EnemyState.Running;
        }
    }
}
