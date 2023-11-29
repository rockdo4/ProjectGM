using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Animator))]
public class EnemyAI : LivingObject
{
    [Header("Range")]
    [SerializeField]
    float detectRange = 10f;
    [SerializeField]
    float meleeAttackRange = 2f;

    [Header("Animation")]
    [SerializeField]
    private float roarDuration = 3f;
    private bool hasRoared = false;

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

    [SerializeField]
    private float attackPreparationTime = 3f;
    [SerializeField]
    private Material attackRangeMaterial;

    private Player player;

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

    GameObject cellInstance;
    private GameObject cellParent;
    private List<GameObject> cellInstances = new List<GameObject>(); // 셀 인스턴스들을 저장할 리스트

    public enum EnemyType
    {
        Enemy1,
        Enemy2,
    }

    #region Interaction Player And Enemy
    public EnemyStat Stat
    {
        get
        {
            return stat as EnemyStat;
        }
    }
    
    #endregion


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

    protected override void Awake()
    {
        base.Awake();
        phaseTwoHealthThreshold = HP * 0.5f;
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
        if (!hasRoared)
            return;

        if (isAttacking)
        {
            return;
        }

        if (isPreparingAttack)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            HP -= 20;
            Debug.Log("현재 체력 : " + HP);
        }

        if (HP <= 0)
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
        if (!isTwoPhase && HP <= phaseTwoHealthThreshold)
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
        Debug.Log(detectedPlayer.name);

        ShowAttackRange(false);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = "MeleeAttack_" + attackType;
            StartCoroutine(IsAnimationRunning(animationTrigger));
            //// 기존 방식
            // StartCoroutine(IsAnimationRunning("MeleeAttack_A"));
        }
    }

    private IEnumerator IsAnimationRunning(string stateName)
    {
        if (animator != null)
        {
            animator.SetTrigger(stateName);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(stateName))
            {
                //player.TakeDamage(Stat.AttackDamage); // 애니메이션 이벤트인 OnAttack로 변경
                //Debug.Log(stateInfo.length);
                //Debug.Log(stateInfo.IsName(stateName));
                yield return new WaitForSeconds(stateInfo.length);
            }

            isAttacking = false;
        }
    }

    private void ShowAttackRange(bool show /*, int attackIndex*/) // 인덱스 굳이? 공격패턴에서 지정해주기
    {

        if (show)
        {
            if (attackRangeInstance == null)
            {
                attackRangeInstance = Instantiate(attackRangePrefab, transform.position, Quaternion.identity);
            }

            AttackPattern currentPattern = savedPatterns[attackIndex];
            cellInstances.Clear(); // 리스트 초기화

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    Vector3 cellPosition = CalculateCellPosition(i);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, Quaternion.identity);
                    cell.SetActive(true);
                    cellInstances.Add(cell); // 생성된 셀 인스턴스를 리스트에 추가
                }
            }

            attackRangeInstance.SetActive(false);
        }
        else
        {
            foreach (GameObject cell in cellInstances) // 리스트의 모든 셀 인스턴스를 순회
            {
                if (cell != null)
                    cell.SetActive(false);
            }


        }


        //// 부모 오브젝트 끄는방법
        //if (show)
        //{
        //    if (attackRangeInstance == null)
        //    {
        //        attackRangeInstance = Instantiate(attackRangePrefab, transform.position, Quaternion.identity);
        //    }

        //    AttackPattern currentPattern = savedPatterns[attackIndex];

        //    for (int i = 0; i < currentPattern.pattern.Length; i++)
        //    {
        //        if (currentPattern.pattern[i]) // 9개의 타일 순회하면서 트루 타일일때
        //        {
        //            Vector3 cellPosition = CalculateCellPosition(i);
        //            cellInstance = Instantiate(attackRangeInstance, cellPosition, Quaternion.identity);
        //            cellInstance.SetActive(true);
        //        }
        //    }

        //    // Debug.Log($"! INDEX : {attackIndex} / COUNT : {i} \n True/false : {savedPatterns[attackIndex].pattern[i]}");

        //    attackRangeInstance.SetActive(false); // 기존꺼는 끄고
        //}
        //else
        //{
        //    foreach (var cell in cellInstance)
        //    {
        //        cell.SetActive(false);
        //    }

        //    // cellInstance라는 변수는 하나하나 일일히 추적하지 않아서 마지막껏만 꺼짐

        //    // 태그방식은 씬을 다 순회해서 별로임

        //    // 부모를 액티브 펄스 해주는게 더 좋다
        //    for (int i = 0; i < attackGrid.Length; i++)
        //    {
        //        if (cellInstance != null)
        //            cellInstance.SetActive(false);
        //    }
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

    bool IsPlayerInCell(int index)
    {
        Vector3 cellPosition = CalculateCellPosition(index);
        return Vector3.Distance(detectedPlayer.position, cellPosition) < 1.5f; // 어느 정도 거리 내에 있는지
    }

    Vector3 CalculateCellPosition(int index)
    {
        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        int index = i * 3 + j;
        //        Gizmos.color = attackGrid[index] ? Color.red : Color.green;

        //        Vector3 cellPosition = transform.position + transform.forward + new Vector3(j + 0.5f, 0, i - 1);
        //        Gizmos.DrawCube(cellPosition, new Vector3(1, 0.1f, 1));
        //    }
        //}



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
            transform.position += direction * Stat.MoveSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Stat.MoveSpeed);
            return INode.EnemyState.Running;
        }
        return INode.EnemyState.Failure;
    }
    #endregion

    #region

    #endregion

    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
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

                    Vector3 cellPosition = transform.position + transform.forward + new Vector3(j + 0.5f, 0, i - 1);
                    Gizmos.DrawCube(cellPosition, new Vector3(1, 0.1f, 1));
                }
            }
        }
    }

    private void Attack()
    {
        if (detectedPlayer == null)
        {
            return;
        }
        // 그리드 형태의 공격
        for (int i = 0; i < savedPatterns[attackIndex].pattern.Length; i++)
        {
            if (savedPatterns[attackIndex].pattern[i] && IsPlayerInCell(i))
            {
                ExecuteAttack(gameObject.GetComponent<EnemyAI>(), player);
                break;
            }
        }
    }

    private void ExecuteAttack(LivingObject attacker, LivingObject defender)
    {
        Attack attack = Stat.CreateAttack(attacker, defender, true);

        var attackables = defender.GetComponents<IAttackable>();
        foreach (var attackable in attackables)
        {
            attackable.OnAttack(gameObject, attack);
        }
    }
}