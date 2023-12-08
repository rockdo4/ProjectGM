using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using static EnemyAI;

[RequireComponent(typeof(Animator))]
public class EnemyAI : LivingObject
{
    private int[] bearAttackPatternPhaseOne = new int[] { 1, 2 }; // ab
    private int[] bearAttackPatternPhaseTwo = new int[] { 1, 2, 3, 2, 3 }; // abcbc

    private int[] alienAttackPatternPhaseOne = new int[] { 1, 2 }; // ab
    private int[] alienAttackPatternPhaseTwo = new int[] { 1, 2, 3 }; // abc

    private int[] spiderAttackPatternPhaseOne = new int[] { 1, 1, 2 }; // aab
    private int[] spiderAttackPatternPhaseTwo = new int[] { 1, 2, 3 }; // abc

    private int[] wolfAttackPatternPhaseOne = new int[] { 1, 1, 2 }; // aab
    private int[] wolfAttackPatternPhaseTwo = new int[] { 1, 2, 1, 2, 2 }; // ababb

    private int meleeAttackIndexOne = 0;
    private int meleeAttackIndexTwo = 1;
    private int meleeAttackIndexThree = 2;

    [Header("원 공격 타입 프리펩")]
    public GameObject attackTypeAPrefab;

    [Header("삼각형 공격 타입 프리펩")]
    public GameObject attackTypeBPrefab;

    [Header("부채꼴 공격 타입 프리펩")]
    public GameObject attackTypeCPrefab;

    [Header("와이파이 공격 타입 프리펩")]
    public GameObject attackTypeDPrefab;

    [Header("공격 대기시간")]
    [SerializeField]
    private float attackPreparationTime = 0.5f;

    [Header("몬스터의 탐지범위")]
    [SerializeField]
    private float detectRange = 10f;

    [Header("몬스터의 공격 사거리")]
    [SerializeField]
    private float meleeAttackRange = 3f;

    [Header("포효 시간 - 몬스터가 처음 등장할때만 작동")]
    [SerializeField]
    private float roarDuration = 3f;

    [Header("플레이어 할당해야 몬스터가 쫒아다님")]
    [SerializeField]
    private LayerMask playerLayerMask;

    [Header("몬스터의 타입")]
    [SerializeField]
    private EnemyType enemyType;

    [Header("공격 조건중 몬스터가 플레이어를 바라볼때의 최소각도")]
    public float minAngle = 10f;
    [Header("몬스터가 고개를 돌리는 속도")]
    public float rotationSpeed = 5f;

    BehaviorTreeRunner BTRunner;
    Transform detectedPlayer;
    private Player player;

    private Animator animator;
    private GameObject attackRangeInstance;
    private Material attackRangeMaterial;

    private int phaseOneAttackSequence = 0;
    private int phaseTwoAttackSequence = 0;

    private float phaseTwoHealthThreshold;

    private bool hasRoared;
    private bool isTwoPhase;
    private bool isAttacking = false;
    private bool isPreparingAttack = false;

    private int attackIndex = -1;

    private float grogySpeed = 0.5f;
    private float grogyTimer = 5f;

    public bool[] attackGrid = new bool[9];
    public List<AttackPattern> savedPatterns = new List<AttackPattern>();
    private List<GameObject> cellInstances = new List<GameObject>(); // 셀 인스턴스들을 저장할 리스트
    private List<GameObject> colliderObjects = new List<GameObject>();

    FanShape fanShape = null;

    public enum EnemyType
    {
        Bear,
        Alien,
        Spider,
        Wolf,
    }

    public enum AttackType
    {
        A,
        B,
        C,
        D,
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
            case EnemyType.Bear:
                BTRunner = new BehaviorTreeRunner(BearBT());
                break;
            case EnemyType.Alien:
                BTRunner = new BehaviorTreeRunner(AlienBT());
                break;
            case EnemyType.Spider:
                BTRunner = new BehaviorTreeRunner(SpiderBT());
                break;
            case EnemyType.Wolf:
                BTRunner = new BehaviorTreeRunner(WolfBT());
                break;

        }
        animator = GetComponent<Animator>();
    }

    private void LookAtPlayer()
    {
        //Vector3 direction = detectedPlayer.position - transform.position;
        //direction.y = 0; // Y축 방향은 고정
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Update()
    {
        //if (detectedPlayer != null)
        //    LookAtPlayer();


        if (IsGroggy)
        {
            animator.ResetTrigger("MeleeAttack_A");
            animator.ResetTrigger("MeleeAttack_B");
            animator.ResetTrigger("MeleeAttack_C");

            animator.SetBool("Grogy", true);
            animator.speed = grogySpeed;

            grogyTimer -= Time.deltaTime;
            if (grogyTimer < 0 || !IsGroggy && animator.GetBool("Grogy"))
            {
                animator.speed = 1f;
                grogyTimer = 5f;
                IsGroggy = false;
                animator.SetBool("Grogy", false);
            }
        }
        else if (!IsGroggy) // 임시
        {
            animator.speed = 1f;
            grogyTimer = 5f;
            animator.SetBool("Grogy", false);
        }

        if (IsGroggy)
            return;

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
            HP -= 100;
            //Debug.Log("현재 체력 : " + HP);
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
                            new ConditionNode(IsPhaseOne),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Bear, bearAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Bear, bearAttackPatternPhaseTwo)),
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
        );
    }

    #endregion

    #region 호랑이 행동트리
    INode AlienBT()
    {
        return new SelectorNode
        (
        new List<INode>()
                {

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ConditionNode(IsPhaseOne),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Alien, alienAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Alien, alienAttackPatternPhaseTwo)),
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
        );
    }
    #endregion

    #region 거미 행동트리
    INode SpiderBT()
    {
        return new SelectorNode
        (
        new List<INode>()
                {

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ConditionNode(IsPhaseOne),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Spider, spiderAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Spider, spiderAttackPatternPhaseTwo)),
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
        );
    }
    #endregion

    #region 늑대 행동트리
    INode WolfBT()
    {
        return new SelectorNode
        (
        new List<INode>()
                {

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ConditionNode(IsPhaseOne),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Wolf, wolfAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Wolf, wolfAttackPatternPhaseTwo)),
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
        );
    }
    #endregion

    #region 행동트리 -> 어택 패턴 -> 애니메이션 업데이트

    private INode.EnemyState ExecuteAttackPattern(EnemyType enemytype, int[] pattern)
    {
        INode.EnemyState result = INode.EnemyState.Failure;
        int attackSequence = isTwoPhase ? phaseTwoAttackSequence : phaseOneAttackSequence;

        switch (pattern[attackSequence])
        {
            case 1:
                result = MelleAttackOne(enemytype, meleeAttackIndexOne);
                break;

            case 2:
                result = MelleAttackTwo(enemytype, meleeAttackIndexTwo);
                break;

            case 3:
                result = MelleAttackThree(enemytype, meleeAttackIndexThree);
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

    IEnumerator PrepareMeleeAttack(EnemyType enemytype, AttackType attackType)
    {
        isPreparingAttack = true;
        ShowAttackRange(true, enemytype, attackType);

        for (float speed = 0.5f; speed >= 0.0f; speed -= Time.deltaTime / attackPreparationTime)
        {
            animator.SetFloat("MoveSpeed", speed);
            yield return null;
        }

        yield return new WaitForSeconds(attackPreparationTime);

        ShowAttackRange(false, enemytype, attackType);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = $"{"Attack_"}{attackType}";
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
                //Debug.Log("공격 애니메이션 재생 시간 : " + stateInfo.length);
                yield return new WaitForSeconds(stateInfo.length);
                isAttacking = false; // 공격상태 해제를 여기서 해주지 않으면 코루틴을 하는 의미가 없어짐
                // 하지만 다음 공격버그가 생긴다면 여기일듯
            }
        }
    }

    private Vector3 GetAttackOffset(EnemyType enemyType, AttackType attackType)
    {
        if (enemyType == EnemyType.Alien)
        {
            switch (attackType)
            {
                case AttackType.B:
                    return new Vector3(0f, 0f, -2f); // 세모위치 조정 에일리언 B패턴임
                case AttackType.C:
                    return new Vector3(0f, 0f, 0f);
            }
        }

        if (enemyType == EnemyType.Bear)
        {
            switch (attackType)
            {
                case AttackType.B:
                    return new Vector3(0f, 0f, -2f); // 세모위치 조정 곰 B 패턴
                case AttackType.C:
                    return new Vector3(0f, 0f, -2f); // 곰 C 패턴
            }
        }

        return Vector3.zero;
    }

    IEnumerator ChangeAttackRangeColor(Material material, float duration)
    {
        float timer = 0;
        Color startColor = Color.yellow;
        Color midColor = new Color(1, 0.65f, 0, 1); // 주황색
        Color endColor = Color.red;

        while (timer < duration)
        {
            float halfDuration = duration / 2;
            if (timer < halfDuration) // 전반부: 노랑에서 주황으로
            {
                material.color = Color.Lerp(startColor, midColor, timer / halfDuration);
            }
            else
            {
                material.color = Color.Lerp(midColor, endColor, (timer - halfDuration) / halfDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        material.color = endColor;
    }


    #region 공격 타일 계산

    Vector3 CalculateCellPosition(int index, Vector3 offset, EnemyType enemyType, AttackType attackType) // 칼큘
    {
        int x = index % 3;
        int z = index / 3;

        Vector3 actualPosition = new Vector3((x - 1) * offset.x, 0.015f, (z - 1) * offset.z);

        if (index == 4)
        {
            actualPosition -= offset; // realOffset 추가 적용
        }

        if (enemyType == EnemyType.Alien && attackType == AttackType.C) // 임시
        {


            switch (index)
            {
                case 1: actualPosition += new Vector3(0, 0, -0.5f); break; // 뒤
                case 3: actualPosition += new Vector3(-0.5f, 0, 0); break; // 오른쪽
                case 5: actualPosition += new Vector3(0.5f, 0, 0); break; // 왼쪽
                case 7: actualPosition += new Vector3(0, 0, 0.5f); break; // 앞

                    // 여기서 딱히 로테이션은 건드리지 않았지만
                    // 여기밖에 바뀔 이유가
            }
        }



        actualPosition = transform.rotation * actualPosition; // 이게 문젠가?
        return transform.position + actualPosition;
    }

    #endregion

    private void ShowAttackRange(bool show, EnemyType enemyType, AttackType attackType) // 쇼
    {
        GameObject attackPrefab = null;
        Vector3 attackOffset = GetAttackOffset(enemyType, attackType);

        switch (attackType)
        {
            case AttackType.A:
                attackPrefab = attackTypeAPrefab;
                //Debug.Log(attackPrefab + "케이스A");
                break;

            case AttackType.B:
                attackPrefab = attackTypeBPrefab;
                //Debug.Log(attackPrefab + "케이스B");
                break;

            case AttackType.C:
                attackPrefab = attackTypeCPrefab;
                //Debug.Log(attackPrefab + "케이스C");
                break;

            case AttackType.D:
                attackPrefab = attackTypeDPrefab;
                break;
        }

        if (show)
        {
            if (attackRangeInstance != null) // 추후 수정
            {
                Destroy(attackRangeInstance);
            }

            attackRangeInstance = Instantiate(attackPrefab, transform);

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

            fanShape = attackPrefab.GetComponent<FanShape>();

            Vector3 cellSize = fanShape.Return(); // 부채꼴의 크기를 Vector3로 받음
            Vector3 offset = new Vector3(cellSize.x + 0.01f, cellSize.y + 0.01f, cellSize.z + 0.01f);

            Vector3 realOffset = attackOffset;

            Vector3 centerPointLocal = fanShape.GetCenterPoint();
            Vector3 centerPointWorld = attackRangeInstance.transform.TransformPoint(centerPointLocal);

            //Debug.Log(cellSize + " 겟 콜라이더한 셀 사이즈");

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    // 베어 어택 공격1 어택 오프셋 0,0,0 찍힘
                    //Debug.Log(attackOffset);

                    Vector3 cellPosition = CalculateCellPosition(i, realOffset, enemyType, attackType);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, transform.rotation, this.transform); // 몬스터 부모로 설정 추가
                    cell.SetActive(true);
                    cellInstances.Add(cell);


                    //if (enemyType == EnemyType.Bear && attackType == AttackType.A) // 곰 A패턴 로테이션 임시 추가
                    //{
                    //    Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                    //    Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                    //    Quaternion additionalRotation = Quaternion.Euler(0f, 330, 0f); // 120도 회전 추가

                    //    cell.transform.rotation = initialRotation * additionalRotation;

                    //}

                    if (i != 4)
                    {
                        Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                        Quaternion additionalRotation = Quaternion.Euler(0f, 120, 0f); // 120도 회전 추가

                        cell.transform.rotation = initialRotation * additionalRotation;
                    }
                    else
                    {
                        cell.transform.rotation = transform.rotation;
                    }

                    GameObject colliderObject = new GameObject("AttackCollider");
                    colliderObject.AddComponent<AttackCell>();
                    // 공격셀 프리펩을 부모로 설정
                    // 콜라이더의 로컬 위치 공격셀 중심으로잡고
                    // 콜라이더의 로컬 회전을 기본값으로

                    colliderObject.transform.SetParent(cell.transform);
                    colliderObject.transform.localRotation = Quaternion.identity;

                    BoxCollider collider = colliderObject.AddComponent<BoxCollider>();
                    collider.size = new Vector3(cellSize.x, 0.015f, cellSize.z);
                    collider.isTrigger = true;

                    Vector3 additionalOffset = Vector3.zero;

                    if (enemyType == EnemyType.Bear && attackType == AttackType.A)
                    {
                        additionalOffset = new Vector3(1.6f, 0f, 2.6f);
                    }

                    if (enemyType == EnemyType.Bear && attackType == AttackType.B || attackType == AttackType.C)
                    {
                        additionalOffset = new Vector3(0f, 0f, 2.5f);
                    }

                    if (enemyType == EnemyType.Alien && attackType == AttackType.B)
                    {
                        Vector3 currentSize = collider.size;
                        collider.size = new Vector3(currentSize.x / 1.8f, currentSize.y, currentSize.z);
                        additionalOffset = new Vector3(0f, 0f, 2.5f);
                    }

                    if (enemyType == EnemyType.Alien && attackType == AttackType.C)
                    {
                        switch (i) // 1, 3, 5, 7 인덱스
                        {
                            case 1:
                                //Debug.Log("0");
                                additionalOffset += new Vector3(0f, 0f, -2f); break;
                            case 3:
                                //Debug.Log("2");
                                additionalOffset += new Vector3(-2f, 0f, 0f); break;
                            case 5: additionalOffset += new Vector3(0f, 0f, 0f); break;
                            case 7: additionalOffset += new Vector3(0f, 0f, 0f); break;
                        }

                        Vector3 currentSize = collider.size;
                        collider.size = new Vector3(currentSize.x / 1.5f, currentSize.y, currentSize.z / 1.5f);

                    }


                    colliderObject.transform.localPosition = Vector3.zero + additionalOffset; // 위치 변경 뒤에 해야함

                    colliderObjects.Add(colliderObject);

                }
            }
            // 어택 콜라이더가 자식이 되어서 수정이 필요함 // 아니면 공격판정이 당연히 안됨 같이 꺼져서
            // 액티브 펄스를 하는 방식에서 매쉬만 끄게 하는걸로
            attackRangeInstance.SetActive(false);

            foreach (GameObject cell in cellInstances)
            {
                MeshRenderer cellMeshRenderer = cell.GetComponent<MeshRenderer>();
                if (cellMeshRenderer != null)
                {


                    //Debug.Log("색깔 바뀌냐?");

                    Material newMaterial = new Material(cellMeshRenderer.sharedMaterial);
                    newMaterial.color = Color.yellow;
                    cellMeshRenderer.material = newMaterial;

                    //Debug.Log(cellMeshRenderer.material);

                    cellMeshRenderer.enabled = true;
                    // 초기 노랑색으로 추가
                    //cellMeshRenderer.material.color = Color.yellow;
                }
            }


            //MeshRenderer meshRenderer = attackRangeInstance.GetComponent<MeshRenderer>();
            //if (meshRenderer != null)
            //{
            //    meshRenderer.enabled = false;
            //}

        }
        else
        {
            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    MeshRenderer cellMeshRenderer = cell.GetComponent<MeshRenderer>();
                    if (cellMeshRenderer != null)
                    {
                        cellMeshRenderer.enabled = false;
                    }
                }
            }
        }
    }

    #endregion 

    #region 행동트리 -> 플레이어 감지 및 추격
    INode.EnemyState DetectPlayer()
    {
        var overlapColliders = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (overlapColliders.Length > 0)
        {
            detectedPlayer = overlapColliders[0].transform;

            Vector3 direction = detectedPlayer.position - transform.position;
            direction.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // 로테이션 스피드 5f 임시

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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
            return INode.EnemyState.Running;
        }
        return INode.EnemyState.Failure;
    }
    #endregion

    #region 근접 공격타입 1, 2, 3

    INode.EnemyState MelleAttackOne(EnemyType enemyType, int attackPatternIndex)
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null)
            return INode.EnemyState.Failure;

        float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

        if (distanceToPlayer >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > minAngle)
            return INode.EnemyState.Failure;

        //Debug.Log(isTwoPhase ? "페이즈2 공격A 준비" : "페이즈1 공격A 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex; // 인덱스 바꾸고

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackType.A));
        return INode.EnemyState.Success;
    }

    INode.EnemyState MelleAttackTwo(EnemyType enemyType, int attackPatternIndex)
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null)
            return INode.EnemyState.Failure;

        float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

        if (distanceToPlayer >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > minAngle)
            return INode.EnemyState.Failure;

        // Debug.Log(isTwoPhase ? "페이즈2 공격B 준비" : "페이즈1 공격B 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackType.B));
        return INode.EnemyState.Success;
    }

    INode.EnemyState MelleAttackThree(EnemyType enemyType, int attackPatternIndex)
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null)
            return INode.EnemyState.Failure;

        float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

        if (distanceToPlayer >= meleeAttackRange)
        {
            isAttacking = false;
            return INode.EnemyState.Failure;
        }

        Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > minAngle)
            return INode.EnemyState.Failure;

        //Debug.Log(isTwoPhase ? "페이즈2 공격C 준비" : "페이즈1 공격C 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackType.C));
        return INode.EnemyState.Success;
    }

    #endregion

    #region 페이즈 체크

    private bool IsPhaseOne()
    {
        if (!isTwoPhase && HP <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true;
            //Debug.Log("페이즈 2로 전환");

            // 여기에 Magic 자식 오브젝트를 활성화하는 코드를 추가
            Transform magicObject = transform.Find("Magic");
            if (magicObject != null)
            {
                magicObject.gameObject.SetActive(true);
            }
            //else
            //{
            //    Debug.Log("Magic 오브젝트를 찾을 수 없음");
            //}
        }
        return !isTwoPhase;
    }

    #endregion



    #region 기즈모

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

            float gizmoSize = 2.1f;
            float offset = gizmoSize + 0.02f;

            for (int index = 0; index < attackGrid.Length; index++)
            {
                int x = index % 3;
                int z = index / 3;

                Vector3 actualPosition = new Vector3((x - 1) * offset, 0f, (z - 1) * offset);
                actualPosition = transform.rotation * actualPosition;
                Vector3 worldPosition = transform.position + actualPosition;

                Gizmos.color = attackGrid[index] ? Color.red : Color.green;

                // 삼각형을 그리기 위한 꼭지점 계산
                Vector3 p1 = worldPosition + new Vector3(-gizmoSize / 2, 0, -gizmoSize / 2);
                Vector3 p2 = worldPosition + new Vector3(0, 0, gizmoSize / 2);
                Vector3 p3 = worldPosition + new Vector3(gizmoSize / 2, 0, -gizmoSize / 2);

                // 삼각형 그리기
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p1);
            }

        }
#endif
    }

    #endregion

    #region 공격 판정 애니메이션 이벤트

    private void Attack()
    {
        float actualAttackDamage = isTwoPhase ? Stat.AttackDamage * 2 : Stat.AttackDamage;

        foreach (GameObject cell in colliderObjects)
        {
            if (cell != null)
            {

                AttackCell attackCell = cell.GetComponent<AttackCell>();

                //Debug.Log("AttackCell: " + (attackCell != null) + ", PlayerInside: " + (attackCell != null && attackCell.playerInside));

                if (attackCell != null && attackCell.playerInside)
                {
                    //Debug.Log("온어택 호출");
                    ExecuteAttack(gameObject.GetComponent<EnemyAI>(), player, actualAttackDamage);
                    break;
                }
            }
        }
    }

    #endregion

    #region 애니메이션 이벤트후에 실제 데미지 주는 부분 - 영재가 추가
    private void ExecuteAttack(LivingObject attacker, LivingObject defender, float actualAttackDamage)
    {
        Attack attack = Stat.CreateAttack(attacker, defender, true);
        attack.CheatAttack((int)actualAttackDamage);

        var attackables = defender.GetComponents<IAttackable>();
        foreach (var attackable in attackables)
        {
            attackable.OnAttack(gameObject, attack);
        }
    }

    #endregion
}