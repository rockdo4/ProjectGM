// 플레이어를 바라보도록 로테이션을 조정하는 노드
using UnityEngine;

public class RotateTowardsPlayerNode : INode
{
    private Transform monsterTransform;
    private Transform playerTransform;

    public RotateTowardsPlayerNode(Transform monster, Transform player)
    {
        this.monsterTransform = monster;
        this.playerTransform = player;
    }

    public INode.EnemyState Evaluate()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - monsterTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, lookRotation, Time.deltaTime * 5f);
            return INode.EnemyState.Success;
            return INode.EnemyState.Running; // 러닝으로 수정 할 수도
        }
        return INode.EnemyState.Failure;
    }
}
