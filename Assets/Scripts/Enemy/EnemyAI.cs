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

    [SerializeField]
    private Animator animator;

    private bool isTwoPhase;
    float phaseTwoHealthThreshold;

    private int[] attackPattern1 = new int[] { 1, 2 };
    private int bearAttackPatternIndex = 0;

    private int[] attackPattern2 = new int[] { 1, 2, 3, 2, 3 };
    private int bearAttackPatternIndex2 = 0;

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
        isTwoPhase = false;

        switch (enemyType)
        {
            case EnemyType.Enemy1:
                BTRunner = new BehaviorTreeRunner(BearBT());
                break;

                //case EnemyType.Enemy2:
                //    BTRunner = new BehaviorTreeRunner(Enemy2BT());
                //    break;
                //case EnemyType.Enemy3:
                //    BTRunner = new BehaviorTreeRunner(Enemy3BT());
                //    break;
        }
        originPos = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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
            // 3. 

            BTRunner.Operate();
        }

    }

    #region 몬스터 종류별 액션

    INode BearBT()
    {
        return new SelectorNode
        (
        new List<INode>()
                {
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            // 공격중에 플레이어가 사거리에서 벗어나면 프레임단위로 공격하는 문제가 발생

                            new ConditionNode(IsBearPhaseOne), // 페이즈 1 체크
                            new ActionNode(() => ExecuteAttackPattern(attackPattern1)) // 페이즈 1 공격 패턴
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsBearPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(attackPattern2)) // 페이즈 2 공격 패턴
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
        ); ; ;
    }

    #endregion

    #region 공격노드

    private bool IsBearPhaseOne()
    {
        if (!isTwoPhase && health <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true;
            Debug.Log("페이즈 2로 전환");
            // phaseTwoAttackSequence = 0; // 페이즈 2의 공격 시퀀스 초기화
        }
        return !isTwoPhase; // 페이즈 1로 다시 반환
    }

    private INode.EnemyState ExecuteAttackPattern(int[] pattern)
    {
        INode.EnemyState result = INode.EnemyState.Failure;

        // 페이즈에 따라 사용할 시퀀스 인덱스 결정
        int attackSequence = isTwoPhase ? phaseTwoAttackSequence : phaseOneAttackSequence;

        switch (pattern[attackSequence])
        {
            case 1:
                Debug.Log(isTwoPhase ? "페이즈2 공격A" : "페이즈1 공격A");
                result = DoMeleeAttack1();
                break;
            case 2:
                Debug.Log(isTwoPhase ? "페이즈2 공격B" : "페이즈1 공격B");
                result = DoMeleeAttack2();
                break;
            case 3:
                Debug.Log("페이즈2 공격C");
                result = DoMeleeAttack3();
                break;
        }

        if (result == INode.EnemyState.Success)
        {
            if (isTwoPhase)
            {
                phaseTwoAttackSequence = (phaseTwoAttackSequence + 1) % pattern.Length;
            }
            else
            {
                phaseOneAttackSequence = (phaseOneAttackSequence + 1) % pattern.Length;
            }
        }
        return result;
    }

    INode.EnemyState DoMeleeAttack1()
    {
        //if (isAttacking)
        //{
        //    // 공격 중일 때도 플레이어와의 거리를 체크해서
        //    // 플레이어가 위치를 벗어나면 다시 추격할 수 있게
        //    // 하지만 이걸 쓰면 난이도가 올라가서 안쓸듯
        //    if (detectedPlayer == null || Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        //    {
        //        isAttacking = false;
        //        return INode.EnemyState.Failure;
        //    }
        //    return INode.EnemyState.Success;
        //}

        if (detectedPlayer == null)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        if (isAttacking)
            return INode.EnemyState.Failure;

        if (Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure; // 플레이어가 사거리 밖이면 추격 상태로 전환
        }

        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("MeleeAttack_A");
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;











        //if (isAttacking)
        //    return INode.EnemyState.Failure;

        //if (detectedPlayer != null &&
        //    Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        //{
        //    isAttacking = true;
        //    return INode.EnemyState.Success;
        //}
        //return INode.EnemyState.Failure;
    }

    INode.EnemyState DoMeleeAttack2()
    {
        if (detectedPlayer == null)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        if (isAttacking)
            return INode.EnemyState.Failure;

        if (Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure; // 플레이어가 사거리 밖이면 추격 상태로 전환
        }

        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("MeleeAttack_B");
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;


        //if (isAttacking)
        //    return INode.EnemyState.Failure;

        //if (detectedPlayer != null && Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        //{
        //    isAttacking = true;
        //    return INode.EnemyState.Success;
        //}

        //return INode.EnemyState.Failure;
    }

    INode.EnemyState DoMeleeAttack3()
    {

        if (detectedPlayer == null)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        if (isAttacking)
            return INode.EnemyState.Failure;

        if (Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure; // 플레이어가 사거리 밖이면 추격 상태로 전환
        }

        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("MeleeAttack_C");
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;


        //if (isAttacking)
        //    return INode.EnemyState.Failure;

        //if (detectedPlayer != null && Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        //{
        //    isAttacking = true;
        //    return INode.EnemyState.Success;
        //}
        //return INode.EnemyState.Failure;
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
            animator.SetFloat("MoveSpeed", 0.5f);
            transform.position = Vector3.MoveTowards(transform.position, detectedPlayer.position, Time.deltaTime * movementSpeed);
            transform.LookAt(detectedPlayer); // Look At Player code
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
            animator.SetTrigger("BearWalk");
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
}
