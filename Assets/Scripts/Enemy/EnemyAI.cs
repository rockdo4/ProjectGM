﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    private bool hasRoared;

    [SerializeField]
    private Animator animator;

    private bool isTwoPhase;
    float phaseTwoHealthThreshold;

    private int[] bearAttackPatternPhaseOne = new int[] { 1, 2 }; // 곰의 A, B 패턴 공격
    private int[] bearAttackPatternPhaseTwo = new int[] { 1, 2, 3, 2, 3 };

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

    private int attackIndex = -1;

    private List<GameObject> cellInstances = new List<GameObject>(); // 셀 인스턴스들을 저장할 리스트

    private List<GameObject> colliderObjects = new List<GameObject>();

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
        // LookAtPlayer();

        StartCoroutine(RoarInit());
    }

    IEnumerator RoarInit()
    {
        hasRoared = false;
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


    private void LookAtPlayer()
    {
        Vector3 direction = detectedPlayer.position - transform.position;
        direction.y = 0; // Y축 방향은 고정
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Update()
    {
        

        if (IsGroggy)
        {
            // 그로기 상태일때
            // 진짜 그로기 상태 만들어주기
        }

        if (detectedPlayer != null)
            LookAtPlayer();


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
            Debug.Log("죽음");
            animator.SetTrigger("Die");
            return;
        }

        if (!isAttacking)
        {
            // 공격중이 아닐때만 실행해버리니까 ㅏ

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
                            new ActionNode(() => ExecuteAttackPattern(bearAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsBearPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(bearAttackPatternPhaseTwo)),
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

            // 여기에 Magic 자식 오브젝트를 활성화하는 코드를 추가
            Transform magicObject = transform.Find("Magic");
            if (magicObject != null)
            {
                magicObject.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Magic 오브젝트를 찾을 수 없음");
            }
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

        for (float speed = 0.5f; speed >= 0.0f; speed -= Time.deltaTime / attackPreparationTime)
        {
            animator.SetFloat("MoveSpeed", speed);
            yield return null;
        }

        // animator.SetFloat("MoveSpeed", 0f); // 대기시간때는 Idle로

        yield return new WaitForSeconds(attackPreparationTime);

        //Debug.Log(detectedPlayer.name);

        ShowAttackRange(false);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = "MeleeAttack_" + attackType;
            StartCoroutine(IsAnimationRunning(animationTrigger));
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

                yield return new WaitForSeconds(stateInfo.length);
            }

            isAttacking = false;
        }
    }

    private void ShowAttackRange(bool show)
    {
        if (show)
        {
            if (attackRangeInstance == null)
            {
                attackRangeInstance = Instantiate(attackRangePrefab, transform);
            }

            foreach (GameObject cell in cellInstances) // 리스트의 모든 셀 인스턴스를 순회
            {
                if (cell != null)
                {
                    Destroy(cell); // 지금 이게 안좋다
                    // 파편화가 날 수 있음 2순위로 두고

                }
            }

            foreach (GameObject colliderObject in colliderObjects)
            {
                if (colliderObject != null)
                {
                    Destroy(colliderObject);
                }
            }
            colliderObjects.Clear();

            AttackPattern currentPattern = savedPatterns[attackIndex];
            cellInstances.Clear(); // 리스트 초기화

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    Vector3 cellPosition = CalculateCellPosition(i);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, transform.rotation, this.transform); // 몬스터 부모로 설정 추가
                    cell.SetActive(true);
                    cellInstances.Add(cell);

                    // 별개의 콜라이더 똑같은 위치에 하나 더 만들기
                    GameObject colliderObject = new GameObject("AttackCollider");
                    colliderObject.AddComponent<AttackCell>();
                    BoxCollider collider = colliderObject.AddComponent<BoxCollider>();


                    collider.size = new Vector3(1, 0.1f, 1);
                    collider.isTrigger = true;
                    colliderObject.transform.position = cellPosition;

                    colliderObjects.Add(colliderObject);
                }
            }

            attackRangeInstance.SetActive(false);

        }
        else
        {
            foreach (GameObject cell in cellInstances) // 리스트의 모든 셀 인스턴스를 순회
            {
                if (cell != null)
                {
                    cell.SetActive(false); // 삭제는 조금 있다
                }
            }
        }
    }

    Vector3 CalculateCellPosition(int index)
    {
        int x = index % 3;
        int z = index / 3;
        
        Vector3 actualPosition = new Vector3(x -1, 0, z + 1.4f);
        actualPosition = transform.rotation * actualPosition;
        return transform.position + actualPosition;
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
        // Vector3 distance = detectedPlayer.position - transform.position;


        var overlapColliders = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (overlapColliders.Length > 0)
        {
            detectedPlayer = overlapColliders[0].transform;

            Vector3 direction = detectedPlayer.position - transform.position;
            direction.y = 0; // Y축 방향은 고정
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 로테이션 스피드 5f 임시

            return INode.EnemyState.Success;
        }

        detectedPlayer = null;
        return INode.EnemyState.Failure;
    }

    INode.EnemyState TracePlayer()
    {
        if (detectedPlayer != null)
        {
            float actualMoveSpeed = isTwoPhase ? Stat.MoveSpeed * 1.5f : Stat.MoveSpeed;

            animator.SetFloat("MoveSpeed", 0.5f); // 속도는 바꿔주고 애니메이션은 일단 Run 유지하고

            Vector3 direction = (detectedPlayer.position - transform.position).normalized;
            transform.position += direction * actualMoveSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * actualMoveSpeed);
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

                    Vector3 cellLocalPosition = new Vector3(j - 1, 0, i + 1.4f);
                    Vector3 cellWorldPosition = transform.TransformPoint(cellLocalPosition);

                    Gizmos.DrawCube(cellWorldPosition, new Vector3(1, 0.1f, 1));

                    // 기존

                    //Vector3 cellPosition = 
                    //    transform.position + transform.forward + new Vector3(j + 1.4f, 0, i - 1);
                    //Gizmos.DrawCube(cellPosition, new Vector3(1, 0.1f, 1));
                }
            }
        }
    }

    private void Attack()
    {
        float actualMoveSpeed = isTwoPhase ? Stat.MoveSpeed * 2 : Stat.MoveSpeed;


        foreach (GameObject cell in colliderObjects)
        {
            if (cell != null)
            {
                AttackCell attackCell = cell.GetComponent<AttackCell>();

                // Debug.Log("AttackCell: " + (attackCell != null) + ", PlayerInside: " + (attackCell != null && attackCell.playerInside));

                if (attackCell != null && attackCell.playerInside)
                {
                    ExecuteAttack(gameObject.GetComponent<EnemyAI>(), player);
                    break;
                }
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