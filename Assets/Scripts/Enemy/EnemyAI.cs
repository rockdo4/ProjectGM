using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 몬스터 종류당 bool변수는 하나여야한다
/// 각 행동 패턴마다 공격시간이 존재한다
/// 공격시간이 끝나고 나서 다음 행동을 한다. 즉, 공격을 다시 하거나 트레이스 상태에 돌입한다
/// 몬스터의 체력이 0이하가 되면 모든 행동을 멈추고 죽어야한다.
/// </summary>

public class EnemyAI : MonoBehaviour
{
    [Header("Range")]
    [SerializeField]
    float detectRange = 10f;
    [SerializeField]
    float meleeAttackRange = 5f;

    [Header("Movement")]
    [SerializeField]
    float movementSpeed = 10f;

    [SerializeField]
    float attackPower = 1f;

    [SerializeField]
    float health = 100f;

    private bool isTwoPhase = false;
    float phaseTwoHealthThreshold;

    private int bearAttackPatternIndex = 0;
    private int[] attackPattern = new int[] { 1, 2, 3, 2, 3 };


    private int phaseOneAttackSequence = 0;
    private int phaseTwoAttackSequence = 0;

    Vector3 originPos;

    BehaviorTreeRunner BTRunner;
    Transform detectedPlayer;

    [SerializeField]
    private EnemyType enemyType;

    private bool isAttacking = false;
    private float attackDuration = 2f;
    private float attackTimer = 0f;

    public enum EnemyType
    {
        Enemy1,
        Enemy2,
    }

    private void Awake()
    {
        phaseTwoHealthThreshold = health * 0.5f;

        switch (enemyType)
        {
            case EnemyType.Enemy1:
                BTRunner = new BehaviorTreeRunner(Enemy1BT());
                break;

                //case EnemyType.Enemy2:
                //    BTRunner = new BehaviorTreeRunner(Enemy2BT());
                //    break;
                //case EnemyType.Enemy3:
                //    BTRunner = new BehaviorTreeRunner(Enemy3BT());
                //    break;
        }
        originPos = transform.position;
    }

    private void Update()
    {
        // 테스트
        if (Input.GetKeyDown(KeyCode.H))
        {
            health /= 2;
            Debug.Log("반피로 줄임, 현재 체력 : " + health);
        }

        if (health <= 0)
            return;

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttacking = false;
                attackTimer = 0f;
            }
        }
        if (!isAttacking)
        {
            // 랜덤어택 2번씩 발생하는 오류
            // 1. 오퍼레이트 호출 주기 조절
            // 2. bool변수 많이 추가 하지만?

            BTRunner.Operate();
        }

    }

    #region 몬스터 종류별 액션

    INode Enemy1BT()
    {
        return new SelectorNode
        (
        new List<INode>()
                {
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ConditionNode(IsAttackSequenceOne),
                            new ActionNode(DoMeleeAttack1), // 그냥 1공격만 함
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ConditionNode(IsAttackSequenceTwo),
                            new ActionNode(DoMeleeAttack2),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>() // 2페이즈
                        {
                            //new ConditionNode(IsBearPhaseTwo),
                            //new ActionNode(EnterBearPhaseTwo),

                            // 순서가 꼬이는 단점
                            // 스위치문 단점
                            // 

                            new ConditionNode(IsAttackSequenceOne),
                            new ActionNode(DoMeleeAttack1),
                            new ConditionNode(IsAttackSequenceTwo),
                            new ActionNode(DoMeleeAttack2),
                            new ConditionNode(IsAttackSequenceTwo),
                            new ActionNode(DoMeleeAttack3),
                             new ConditionNode(IsAttackSequenceTwo),
                            new ActionNode(DoMeleeAttack2),
                            new ConditionNode(IsAttackSequenceTwo),
                            new ActionNode(DoMeleeAttack3),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(DetectPlayer),
                            new ActionNode(TracePlayer),
                        }
                    ),
                    new ActionNode(MoveToOriginPosition)
                }
        );
    }

    #endregion

    #region 공격노드


    private bool IsAttackSequenceOne()
    {
        return phaseOneAttackSequence == 0;
    }

    private bool IsAttackSequenceTwo()
    {
        return phaseOneAttackSequence == 1;
    }

    INode.EnemyState DoMeleeAttack1()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log("근접 공격 111!");
            isAttacking = true;
            phaseOneAttackSequence = 1;
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;
    }

    INode.EnemyState DoMeleeAttack2()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer != null && Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log("근접 공격 222!");
            isAttacking = true;
            phaseOneAttackSequence = 0;
            return INode.EnemyState.Success;
        }
        return INode.EnemyState.Failure;
    }

    INode.EnemyState DoMeleeAttack3()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer != null && Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log("근접 공격 333!");
            return INode.EnemyState.Success;
        }
        return INode.EnemyState.Failure;
    }
    #endregion

    #region 감지 및 이동 노드
    INode.EnemyState DetectPlayer()
    {
        var overlapColliders = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (overlapColliders.Length > 0)
        {
            detectedPlayer = overlapColliders[0].transform;
            return INode.EnemyState.Success;
        }

        detectedPlayer = null;
        return INode.EnemyState.Failure;
    }

    INode.EnemyState TracePlayer()
    {
        if (detectedPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, detectedPlayer.position, Time.deltaTime * movementSpeed);
            return INode.EnemyState.Running;
        }

        return INode.EnemyState.Failure;
    }
    #endregion

    #region  제자리 돌아가기 나중에 다른것으로 수정
    INode.EnemyState MoveToOriginPosition()
    {
        if (Vector3.Distance(originPos, transform.position) < 0.01f)
        {
            return INode.EnemyState.Success;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, Time.deltaTime * movementSpeed);
            return INode.EnemyState.Running;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }

    INode.EnemyState EnterBearPhaseTwo()
    {
        // 2페이즈 공격 패턴에 따른 행동 실행
        // 하지만 인덱스는 별로인듯?

        INode.EnemyState result = INode.EnemyState.Failure; // 페일러 상태로 초기화
        switch (attackPattern[bearAttackPatternIndex])
        {
            case 1:
                Debug.Log("진짜 2페이즈 공격 스위치문 돌입");
                result = DoMeleeAttack1();
                break;
            case 2:
                result = DoMeleeAttack2();
                break;
            case 3:
                result = DoMeleeAttack3();
                break;
        }
        
        if (result == INode.EnemyState.Success) // 공격을 성공할때만 인덱스 업데이트
        {
            bearAttackPatternIndex = (bearAttackPatternIndex + 1) % attackPattern.Length;
        }

        return result;
    }

    private bool IsBearPhaseTwo()
    {
        // 2페이즈 진입 조건 한번만 진입하도록 수정
        if (!isTwoPhase && health <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true; // 2페이즈 상태 전환
            Debug.Log("페이즈2 진입!!!!");
            return true;
        }
        return false;
    }
}
