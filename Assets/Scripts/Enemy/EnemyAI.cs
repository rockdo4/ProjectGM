using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Range")]
    [SerializeField]
    float detectRange = 10f;
    [SerializeField]
    float meleeAttackRange = 2f;

    [Header("Movement")]
    [SerializeField]
    float movementSpeed = 1f;

    [Header("Animation")]
    [SerializeField]
    private float roarDuration = 3f;
    private bool hasRoared = false;

    [SerializeField]
    float attackPower = 1f;

    [SerializeField]
    float health = 100f;

    private float mindistance = 2.5f;

    [SerializeField]
    private Animator animator;

    private bool isTwoPhase;
    float phaseTwoHealthThreshold;

    private int[] attackPattern1 = new int[] { 1, 2 }; // 곰의 A, B 패턴 공격
    private int[] attackPattern2 = new int[] { 1, 2, 3, 2, 3 };

    private int phaseOneAttackSequence = 0;
    private int phaseTwoAttackSequence = 0;

    [SerializeField]
    private LayerMask playerLayerMask;

    Vector3 originPos;

    BehaviorTreeRunner BTRunner;
    Transform detectedPlayer;

    [SerializeField]
    private EnemyType enemyType;

    private bool isAttacking = false;
    private float attackDuration = 2f;
    private float attackTimer = 0f;

    [SerializeField]
    private float meleeAttackPower = 5f;
    [SerializeField]
    private float attackPreparationTime = 3f;
    [SerializeField]
    private Material attackRangeMaterial;

    private Player player;
    private Rigidbody rigidbody;

    private bool isPreparingAttack = false;

    [Header("범위 공격의 시각화")]
    [SerializeField]

    public GameObject attackRangePrefab;
    private GameObject attackRangeInstance;

    [SerializeField]
    public bool[] attackGrid = new bool[9];
    [SerializeField]
    public List<AttackPattern> savedPatterns = new List<AttackPattern>();

    int attackIndex = -1;
  

    public enum EnemyType
    {
        Enemy1,
        Enemy2,
    }

    private void Start()
    {
        StartCoroutine(RoarInit());
    }

    IEnumerator RoarInit()
    {
        animator.SetTrigger("Roar");
        yield return new WaitForSeconds(roarDuration);
        hasRoared = true;
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
        }
        originPos = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!hasRoared) // 다시 추가
            return;

        //if (testAniPlaying)
        //    return;

        if (isAttacking)
        {
            return;
        }

        if (isPreparingAttack)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.P)) // P키를 누르면 패턴 출력
        //{
        //    PrintAttackPatterns();
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
            health -= 20;
            Debug.Log("현재 체력 : " + health);
        }

        if (health <= 0)
        {
            animator.SetTrigger("Die");
            return;
        }

        if (!isAttacking)
        {
            BTRunner.Operate();
        }

    }

    #region 곰 행동트리

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
                            new ConditionNode(IsBearPhaseOne),
                            new ActionNode(() => ExecuteAttackPattern(attackPattern1))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsBearPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(attackPattern2)),
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

                }
        ); ;
    }

    #endregion

    #region 어택 패턴 업데이트

    #endregion 공격 노드 함수

    #region

    private bool IsBearPhaseOne()
    {
        if (!isTwoPhase && health <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true;
            Debug.Log("페이즈 2로 전환");
        }
        return !isTwoPhase;
    }


    private INode.EnemyState ExecuteAttackPattern(int[] pattern)
    {
        INode.EnemyState result = INode.EnemyState.Failure;

        int attackSequence = isTwoPhase ? phaseTwoAttackSequence : phaseOneAttackSequence;

        switch (pattern[attackSequence])
        {
            case 1:
                result = DoMeleeAttack1();
                break;

            case 2:
                result = DoMeleeAttack2();
                break;

            case 3:
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

    IEnumerator PrepareMeleeAttack(string attackType)
    {
        isPreparingAttack = true;
        ShowAttackRange(true);

        yield return new WaitForSeconds(attackPreparationTime);

        ShowAttackRange(false);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = "MeleeAttack_" + attackType;
            StartCoroutine(IsAnimationRunning(animationTrigger));

            //Debug.Log(animationTrigger);

            //// 기존 방식
            // StartCoroutine(IsAnimationRunning("MeleeAttack_A"));
        }
    }

    private IEnumerator IsAnimationRunning(string stateName)
    {
        if (animator != null)
        {
            animator.SetTrigger(stateName);
            //Debug.Log("123123");

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(stateName))
            {
                //player.TakeDamage(meleeAttackPower); // 애니메이션 이벤트인 OnAttack로 변경
                //Debug.Log(stateInfo.length);
                //Debug.Log(stateInfo.IsName(stateName));
                yield return new WaitForSeconds(stateInfo.length);
            }

            isAttacking = false;
        }
    }

    private void ShowAttackRange(bool show/*, int attackIndex*/) // 인덱스 굳이?
    {
        if (show)
        {
            if (attackRangeInstance == null)
            {
                attackRangeInstance = Instantiate(attackRangePrefab, transform.position, Quaternion.identity);
            }
            // 공격 패턴에 따라 공격 범위 시각화를 조정
            AttackPattern currentPattern = savedPatterns[attackIndex];
            UpdateAttackRangeDisplay(currentPattern);

            attackRangeInstance.SetActive(true);

        }
        else
        {
            if (attackRangeInstance != null)
                attackRangeInstance.SetActive(false);
        }



        //if (show)
        //{
        //    //ClearAttackRangeInstances(); // 이전에 생성된 인스턴스를 정리

        //    for (int i = 0; i < 3; i++)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            int index = i * 3 + j;
        //            if (attackGrid[index])
        //            {
        //                Vector3 cellPosition = transform.position + transform.forward + new Vector3(j - 1 + 0.5f, 0, i - 1);
        //                GameObject cellInstance = Instantiate(attackRangePrefab, cellPosition, Quaternion.identity);

        //                // 기즈모 참고
        //                // Gizmos.DrawCube(cellPosition, new Vector3(1, 0.1f, 1));

        //                // 여기에서 cellInstance의 크기, 색상 등을 설정할 수 있습니다.
        //                // 예: cellInstance.GetComponent<Renderer>().material.color = Color.red;
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    ClearAttackRangeInstances();
        //}
    }

    private void ClearAttackRangeInstances()
    {
        // 기존에 생성된 공격 범위 인스턴스들을 제거
        foreach (var instance in GameObject.FindGameObjectsWithTag("AttackRange"))
        {
            Destroy(instance);
        }
    }


    private void UpdateAttackRangeDisplay(AttackPattern pattern)
    {
        // 패턴에 따라 공격 범위의 크기 및 위치를 조정
        for (int i = 0; i < pattern.pattern.Length; i++)
        {
            Vector3 cellPosition = CalculateCellPosition(i);
            // 해당 셀의 위치에 공격 범위 인스턴스를 배치 (예시: 셀마다 별도의 인스턴스를 생성하거나 위치를 조정)
            // ... 공격 범위 인스턴스 배치 로직 ...
        }
    }



    private Vector3 GetMonsterSize() // 임시
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Vector3 size = collider.bounds.size;
            return size;
        }
        return Vector3.one;
    }


    bool IsPlayerInCell(int index)
    {
        Vector3 cellPosition = CalculateCellPosition(index);
        return Vector3.Distance(detectedPlayer.position, cellPosition) < 1.5f; // 어느 정도 거리 내에 있는지
    }

    Vector3 CalculateCellPosition(int index)
    {
        int x = index % 3; // 가, 세
        int z = index / 3;
        return transform.position + transform.forward + new Vector3(x - 1 + 0.5f, 0, z - 1);
        // 기즈모에 표시된 범위와 일치해야하니까 수정하긴했는데
    }

    INode.EnemyState DoMeleeAttack1()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null || Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }
        if (Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log(isTwoPhase ? "페이즈2 공격A 준비" : "페이즈1 공격A 준비");

            isAttacking = true;

            attackIndex = 0; //  A패턴
            // 지금 구조는 저장된 패턴의 인덱스를 일일히 지정해주어야하는데 다른 방식도 생각해보기

            StartCoroutine(PrepareMeleeAttack("A"));
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;

    }

    INode.EnemyState DoMeleeAttack2()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null || Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }
        if (Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log(isTwoPhase ? "페이즈2 공격B 준비" : "페이즈1 공격B 준비");

            isAttacking = true;

            attackIndex = 1; //  B패턴
            StartCoroutine(PrepareMeleeAttack("B"));
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;
    }

    INode.EnemyState DoMeleeAttack3()
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null || Vector3.Distance(detectedPlayer.position, transform.position) >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }
        if (Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            Debug.Log(isTwoPhase ? "페이즈2 공격C 준비" : "출력되면 안되는 거임");

            isAttacking = true;

            attackIndex = 2; //  C패턴
            StartCoroutine(PrepareMeleeAttack("C"));
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;
    }
    #endregion

    #region
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

            Vector3 direction = (detectedPlayer.position - transform.position).normalized;
            transform.position += direction * movementSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * movementSpeed);
            return INode.EnemyState.Running;
        }
        return INode.EnemyState.Failure;
    }
    #endregion

    #region

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = i * 3 + j;
                Gizmos.color = attackGrid[index] ? Color.red : Color.green;

                // 트랜스폼 포지션 빼니까 몬스터의 현재 위치에 대한 정보가 빠져버림
                Vector3 cellPosition = transform.position + transform.forward + new Vector3(j + 0.5f, 0, i - 1);
                Gizmos.DrawCube(cellPosition, new Vector3(1, 0.1f, 1));
            }
        }
    }

    private void OnAttack() // 임시 데미지 애니메이션 이벤트
    {

        // 이 데미지를 판정해주는 테스트 메서드에서

        // 매개변수로 패턴을 넘겨봤자 의미가 있나?
        // 왜냐하면 코드에서는 실행 안함
        // 애니메이션 이벤트니까

        // 결국 작동이 됐을때 "지금 공격패턴이 어떤 패턴인지?" 를 알아야됨

        if (detectedPlayer != null)
        {
            // 그리드 형태의 공격
            for (int i = 0; i < attackGrid.Length; i++)
            {
                if (savedPatterns[attackIndex].pattern[i] && IsPlayerInCell(i))
                {
                    Debug.Log(attackIndex);

                    player.TakeDamage(meleeAttackPower);
                    break;


                    //player.TakeDamage(meleeAttackPower);
                     // 플레이어가 어느 하나의 공격 셀 내에 있으면 데미지 적용 후 반복문 종료
                    // 그런데 지금은 어느 하나라도 켜져있으면 무조건 트루 발생
                }
            }

        }
    }

    //private void PrintAttackPatterns()
    //{
    //    Debug.Log("Attack Pattern 1:");
    //    for (int i = 0; i < attackPattern1.Length; i++)
    //    {
    //        string attackType = attackPattern1[i] == 1 ? "근접 A공격" : "다른 공격 유형";
    //        Debug.Log("Index " + i + ": " + attackType);
    //    }

    //    Debug.Log("Attack Pattern 2:");
    //    for (int i = 0; i < attackPattern2.Length; i++)
    //    {
    //        string attackType = attackPattern2[i] == 1 ? "근접 B공격" : "다른 공격 유형";
    //        Debug.Log("Index " + i + ": " + attackType);
    //    }
    //}
}