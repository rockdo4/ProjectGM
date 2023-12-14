using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAI : LivingObject
{
    private int[] bearAttackPatternPhaseOne = new int[] { 1, 2 }; // ab
    private int[] bearAttackPatternPhaseTwo = new int[] { 1, 2, 3, 2, 3 }; // abcbc

    private int[] alienAttackPatternPhaseOne = new int[] { 1, 2 }; // ab
    private int[] alienAttackPatternPhaseTwo = new int[] { 1, 2, 3 }; // abc

    // 공격 속도는 D<C<B<A 순서로 A가 제일 빠르다
    //private int[] wildBoarAttackPatternPhaseOne = new int[] { 5 }; 

    // 테스트
    private int[] wildBoarAttackPatternPhaseOne = new int[] { 1, 2, 3 }; // abd
    private int[] wildBoarAttackPatternPhaseTwo = new int[] { 1, 2, 3 }; // abdc

    //private int[] wildBoarAttackPatternPhaseOne = new int[] { 1, 2, 5 }; // abd
    //private int[] wildBoarAttackPatternPhaseTwo = new int[] { 1, 2, 5, 3 }; // abdc

    private int[] wolfAttackPatternPhaseOne = new int[] { 1, 1, 2 }; // aab
    private int[] wolfAttackPatternPhaseTwo = new int[] { 1, 2, 1, 2, 2 }; // ababb

    private int EnemyMeleeAttackIndexOne = 0;
    private int EnemyMeleeAttackIndexTwo = 1;
    private int EnemyMeleeAttackIndexThree = 2;
    private int EnemyMeleeAttackIndexFour = 3;

    private int EnemyRangedAttackIndexOne = 0;
    private int EnemyRangedAttackIndexTwo = 1;
    private int EnemyRangedAttackIndexThree = 2;
    private int EnemyRangedAttackIndexFour = 3;

    [Header("슬래시 원거리 공격 타입 프리펩")]
    public GameObject rangeIndicator;

    [Header("A공격 타입 프리펩")]
    public GameObject AttackPatternTypeAPrefab;

    [Header("B공격 타입 프리펩")]
    public GameObject AttackPatternTypeBPrefab;

    [Header("C공격 타입 프리펩")]
    public GameObject AttackPatternTypeCPrefab;

    [Header("D공격 타입 프리펩")]
    public GameObject AttackPatternTypeDPrefab;

    [Header("공격 대기시간")]
    [SerializeField]
    public float attackPreparationTime = 0.5f;

    [Header("몬스터의 탐지범위")]
    [SerializeField]
    private float detectRange = 100f;

    [Header("몬스터의 근접 공격 사거리")]
    [SerializeField]
    private float meleeAttackRange = 3f;

    [Header("몬스터의 근접 공격 사거리")]
    [SerializeField]
    private float rangeAttackRange = 6f;

    [Header("포효 시간 - 몬스터가 처음 등장할때만 작동")]
    [SerializeField]
    private float roarDuration = 3f;

    [Header("플레이어 할당해야 몬스터가 쫒아다님")]
    [SerializeField]
    private LayerMask playerLayerMask;

    [Header("몬스터의 타입")]
    [SerializeField]
    public EnemyType enemyType;

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

    private bool isDie;

    

    public enum EnemyType
    {
        Bear,
        Alien,
        WildBoar,
        Wolf,
    }

    public enum AttackPatternType
    {
        A,
        B,
        C,
        D,
        RangeA,
        RangeB,
        RangeC,
        RangeD,
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
            case EnemyType.WildBoar:
                BTRunner = new BehaviorTreeRunner(WildBoarBT());
                break;
            case EnemyType.Wolf:
                BTRunner = new BehaviorTreeRunner(WolfBT());
                break;

        }
        animator = GetComponent<Animator>();
        animator.speed = 1f; // 기본 속도로 복귀
        isDie = false;

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    rangeIndicator.SetActive(!rangeIndicator.activeSelf);
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
            HP -= 100;
            Debug.Log("현재 체력 : " + HP);
        }

        if (!hasRoared)
            return;
        
        if (HP <= 0 && !isDie)
        {
            animator.SetTrigger("Die");
            isDie = true;
            return;
        }

        if (isPreparingAttack)
        {
            return;
        }

        if (isAttacking)
        {
            return;
        }

        BTRunner.Operate(); // 위치변경

        
    }

    #region 그로기



    INode.EnemyState GroggyTrueState()
    {
        if (IsGroggy)
        {
            //Debug.Log("그로기 상태입니다.");
            grogyTimer -= Time.deltaTime;

            IsGroggy = true;

            animator.ResetTrigger("Attack_A");
            animator.ResetTrigger("Attack_B");
            animator.ResetTrigger("Attack_C");

            //animator.speed = grogySpeed;

            animator.SetBool("Grogy", true);

            //Debug.Log(grogyTimer);

            if (grogyTimer <= 0)
            {
                grogyTimer = 5f; // 그로기 상태 지속 시간 초기화
                IsGroggy = false;
                //GroggyFalseState();
            }

            return INode.EnemyState.Success; // 계속 그로기 상태 유지 // 러닝 or 석세스
        }

        return INode.EnemyState.Failure;
    }

    INode.EnemyState GroggyFalseState()
    {
        //Debug.Log("그로기 상태가 해제되었습니다.");

        grogyTimer = 5f;
        //animator.speed = 1f; // 기본 속도로 복귀
        //Debug.Log("셋불 그로기 펄스 호출 - Set그로기 스테이트");
        animator.SetBool("Grogy", false);

        IsGroggy = false;
        isAttacking = false;
        isPreparingAttack = false;

        // SetGroggyState(false); // 안전을 위해 상태 해제를 한번 더 호출
        return INode.EnemyState.Success;
    }

    #endregion


    #region 행동트리 -> 페이즈 전환 체크 및 처리


    #endregion


    #region 곰 행동트리

    INode BearBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new DecoratorNode
                (
                    () => IsGroggy,
                    new ActionNode(GroggyTrueState)
                ),

                new DecoratorNode
                (
                    () => !IsGroggy,

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(GroggyFalseState),

                            new SelectorNode
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
                            )
                        }
                    )
                )
            }
        );
    }



    #endregion

    #region 에일리언 행동트리
    INode AlienBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new DecoratorNode
                (
                    () => IsGroggy,
                    new ActionNode(GroggyTrueState)
                ),

                new DecoratorNode
                (
                    () => !IsGroggy,

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(GroggyFalseState),

                            new SelectorNode
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
                            )
                        }
                    )
                )
            }
        );
    }
    #endregion

    #region 멧돼지 행동트리
    INode WildBoarBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new DecoratorNode
                (
                    () => IsGroggy,
                    new ActionNode(GroggyTrueState)
                ),

                new DecoratorNode
                (
                    () => !IsGroggy,

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(GroggyFalseState),

                            new SelectorNode
                            (
                                new List<INode>()
                                {
                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new ConditionNode(IsPhaseOne),
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.WildBoar, wildBoarAttackPatternPhaseOne))
                                        }
                                    ),

                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new InverterNode(new ConditionNode(IsPhaseOne)),
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.WildBoar, wildBoarAttackPatternPhaseTwo)),
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
                            )
                        }
                    )
                )
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
                new DecoratorNode
                (
                    () => IsGroggy,
                    new ActionNode(GroggyTrueState)
                ),

                new DecoratorNode
                (
                    () => !IsGroggy,

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(GroggyFalseState),

                            new SelectorNode
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
                            )
                        }
                    )
                )
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
                result = MelleAttackOne(enemytype, EnemyMeleeAttackIndexOne);
                break;

            case 2:
                result = MelleAttackTwo(enemytype, EnemyMeleeAttackIndexTwo);
                break;

            case 3:
                result = MelleAttackThree(enemytype, EnemyMeleeAttackIndexThree);
                break;

            case 4:
                result = MelleAttackFour(enemytype, EnemyMeleeAttackIndexFour);
                break;

            //case 5:
            //    result = RangeAttackOne(enemytype, EnemyRangedAttackIndexOne);
            //    break;

            case 6:
                result = RangeAttackTwo(enemytype, EnemyRangedAttackIndexTwo);
                break;

            case 7:
                result = RangeAttackThree(enemytype, EnemyRangedAttackIndexThree);
                break;

            case 8:
                result = RangeAttackFour(enemytype, EnemyRangedAttackIndexFour);
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

    IEnumerator PrepareMeleeAttack(EnemyType enemytype, AttackPatternType AttackPatternType) // 근접
    {
        isPreparingAttack = true;
        ShowMeleeAttackRange(true, enemytype, AttackPatternType);

        for (float speed = 0.5f; speed >= 0.0f; speed -= Time.deltaTime / attackPreparationTime)
        {
            animator.SetFloat("MoveSpeed", speed);
            yield return null;
        }

        yield return new WaitForSeconds(attackPreparationTime);

        ShowMeleeAttackRange(false, enemytype, AttackPatternType);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = $"{"Attack_"}{AttackPatternType}";
            StartCoroutine(IsAnimationRunning(animationTrigger));
        }
    }

    IEnumerator PrepareRangedAttack(EnemyType enemytype, AttackPatternType attackPatternType) // 원거리
    {
        isPreparingAttack = true;
        ShowRangeAttackRange(true, enemytype, attackPatternType);

        for (float speed = 0.5f; speed >= 0.0f; speed -= Time.deltaTime / attackPreparationTime)
        {
            animator.SetFloat("MoveSpeed", speed);
            yield return null;
        }

        yield return new WaitForSeconds(attackPreparationTime);

        ShowRangeAttackRange(false, enemytype, attackPatternType);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            // 원거리 공격 시각화가 끝난 뒤 공격 애니메이션과 같이 실행
            //MonsterRangedAttack(enemytype, AttackPatternType);

            //string animationTrigger = $"{"Attack_"}{AttackPatternType.A}"; // 이거 임시임
            //string animationTrigger = $"{"Attack_"}{AttackPatternType.A}";
            //StartCoroutine(IsAnimationRunning(animationTrigger));
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
                Debug.Log(isAttacking);
                Debug.Log("공격 애니메이션 재생 시간 : " + stateInfo.length);
                yield return new WaitForSeconds(stateInfo.length);
                isAttacking = false; // 공격상태 해제를 여기서 해주지 않으면 코루틴을 하는 의미가 없어짐
                // 하지만 다음 공격버그가 생긴다면 여기일듯
            }
        }
    }

    private Vector3 GetAttackOffset(EnemyType enemyType, AttackPatternType AttackPatternType)
    {
        

        if (enemyType == EnemyType.Bear)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.A:
                    return new Vector3(0f, 0f, -2f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f); // 세모위치 조정 곰 B 패턴
                case AttackPatternType.C:
                    return new Vector3(4f, 0f, -2f); // 곰 C 패턴
                default: return Vector3.zero;
            }
        }

        if (enemyType == EnemyType.Alien)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f); // 세모위치 조정 에일리언 B패턴임
                case AttackPatternType.C:
                    return Vector3.zero;
                default: return Vector3.zero;
            }
        }

        if (enemyType == EnemyType.WildBoar)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.A:
                    return new Vector3(0f, 0f, -2f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f);
                case AttackPatternType.C:
                    return new Vector3(0f, 0f, -7f);
                default: return Vector3.zero;
            }
        }

        return Vector3.zero;
    }

    #region 공격 타일 계산

    Vector3 CalculateCellPosition(int index, Vector3 offset, Vector3 realoffset, EnemyType enemyType, AttackPatternType AttackPatternType) // 칼큘
    {
        int x = index % 3;
        int z = index / 3;

        Vector3 actualPosition = new Vector3((x - 1) * offset.x, 0.015f, (z - 1) * offset.z);

        if (index == 4) // 중간 위치의 인덱스
        {
            actualPosition -= realoffset; // realOffset 추가 적용
        }

        //if (enemyType == EnemyType.Alien && AttackPatternType == AttackPatternType.C) // 임시
        //{
        //    actualPosition += new Vector3(0, 0, 4f);
        //}

        if (enemyType == EnemyType.Alien && AttackPatternType == AttackPatternType.C) // 임시
        {
            switch (index)
            {
                case 1: actualPosition += new Vector3(0, 0, 4f); break; // 뒤
                case 3: actualPosition += new Vector3(4f, 0, 0); break; // 오른쪽
                case 5: actualPosition += new Vector3(-4f, 0, 0); break; // 왼쪽
                case 7: actualPosition += new Vector3(0, 0, -4f); break; // 앞
            }
        }
        actualPosition = transform.rotation * actualPosition; // 이게 문젠가?
        // 이거를 스위치문 위에 올리면 위치는 이상하게 찍혀도 해결되긴함
        // 근데 위치가 가까워지면 여전히 로테이션이 붕뜨게된다

        return transform.position + actualPosition;
    }

    #endregion

    private void ShowMeleeAttackRange(bool show, EnemyType enemyType, AttackPatternType AttackPatternType) // 쇼
    {
        GameObject attackPrefab = null;
        Vector3 attackOffset = GetAttackOffset(enemyType, AttackPatternType);

        switch (AttackPatternType)
        {
            case AttackPatternType.A:
                attackPrefab = AttackPatternTypeAPrefab;
                break;

            case AttackPatternType.B:
                attackPrefab = AttackPatternTypeBPrefab;
                break;

            case AttackPatternType.C:
                attackPrefab = AttackPatternTypeCPrefab;
                break;

            case AttackPatternType.D:
                attackPrefab = AttackPatternTypeDPrefab;
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

            fanShape = attackRangeInstance.GetComponent<FanShape>();
            fanShape.enemyAi = this; // 대기시간 넣어야 돼서 추가

            Vector3 cellSize = fanShape.Return(); // 부채꼴의 크기를 Vector3로 받음
            Vector3 offset = new Vector3(cellSize.x + 0.01f, cellSize.y + 0.015f, cellSize.z + 0.01f);
            //Vector3 centerPointLocal = fanShape.GetCenterPoint();
            //Vector3 centerPointWorld = attackRangeInstance.transform.TransformPoint(centerPointLocal);

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    Vector3 cellPosition = CalculateCellPosition(i, offset, attackOffset, enemyType, AttackPatternType);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, transform.rotation, this.transform); // 몬스터 부모로 설정 추가
                    cell.SetActive(true);
                    cellInstances.Add(cell);

                    if (i != 4)
                    {
                        Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                        Quaternion additionalRotation = Quaternion.Euler(0f, 120f, 0f);

                        cell.transform.rotation = initialRotation * additionalRotation;
                    }
                    else if (enemyType == EnemyType.Bear && AttackPatternType == AttackPatternType.A)
                    {
                        Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                        Quaternion additionalRotation = Quaternion.Euler(0f, 150f, 0f);

                        cell.transform.rotation = initialRotation * additionalRotation;
                    }

                    // 와일드 보어

                    else if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.A)
                    {
                        Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                        Quaternion additionalRotation = Quaternion.Euler(0f, 135f, 0f);

                        cell.transform.rotation = initialRotation * additionalRotation;
                    }
                    else if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.B)
                    {
                        Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                        Quaternion additionalRotation = Quaternion.Euler(0f, 105f, 0f);

                        cell.transform.rotation = initialRotation * additionalRotation;
                    }
                    //else if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.C)
                    //{
                    //    Vector3 directionToMonster = (transform.position - cellPosition).normalized;
                    //    Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);

                    //    Quaternion additionalRotation = Quaternion.Euler(0f, 70f, 0f);

                    //    cell.transform.rotation = initialRotation * additionalRotation;
                    //}
                    else
                    {
                        cell.transform.rotation = transform.rotation;
                    }

                    GameObject colliderObject = new GameObject("AttackCollider");
                    colliderObject.AddComponent<AttackCell>();

                    colliderObject.transform.SetParent(cell.transform);
                    colliderObject.transform.localRotation = Quaternion.identity;

                    BoxCollider collider = colliderObject.AddComponent<BoxCollider>();
                    collider.size = new Vector3(cellSize.x, 0.015f, cellSize.z);
                    collider.isTrigger = true;

                    Vector3 additionalOffset = Vector3.zero;

                    if (enemyType == EnemyType.Bear && AttackPatternType == AttackPatternType.A)
                    {
                        additionalOffset = new Vector3(1.6f, 0f, 2.6f);
                    }

                    if (enemyType == EnemyType.Bear && AttackPatternType == AttackPatternType.B || AttackPatternType == AttackPatternType.C)
                    {
                        additionalOffset = new Vector3(0f, 0f, 2.5f);
                    }

                    if (enemyType == EnemyType.Alien && AttackPatternType == AttackPatternType.B)
                    {
                        Vector3 currentSize = collider.size;
                        collider.size = new Vector3(currentSize.x / 1.8f, currentSize.y, currentSize.z);
                        additionalOffset = new Vector3(0f, 0f, 2.5f);
                    }

                    if (enemyType == EnemyType.Alien && AttackPatternType == AttackPatternType.C)
                    {
                        switch (i) // 1, 3, 5, 7 인덱스
                        {
                            case 1:
                                additionalOffset += new Vector3(0f, 0f, -2f); break;
                            case 3:
                                additionalOffset += new Vector3(-2f, 0f, 0f); break;
                            case 5: additionalOffset += new Vector3(0f, 0f, 0f); break;
                            case 7: additionalOffset += new Vector3(0f, 0f, 0f); break;
                        }

                        Vector3 currentSize = collider.size;
                        collider.size = new Vector3(currentSize.x / 1.5f, currentSize.y, currentSize.z / 1.5f);

                    }

                    if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.A)
                    {
                        additionalOffset = new Vector3(2f, 0f, 2f);
                    }

                    if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.B)
                    {
                        Vector3 currentSize = collider.size;
                        collider.size = new Vector3(currentSize.x * 0.3f, currentSize.y, currentSize.z * 0.08f);
                        additionalOffset = new Vector3(7f, 0, 0.7f);
                    }

                    if (enemyType == EnemyType.WildBoar && AttackPatternType == AttackPatternType.C)
                    {
                        additionalOffset = Vector3.zero;
                    }


                    colliderObject.transform.localPosition = Vector3.zero + additionalOffset; // 위치 변경 뒤에 해야함
                    colliderObjects.Add(colliderObject);
                }
            }
            // 어택 콜라이더가 자식이 되어서 수정이 필요함 // 아니면 공격판정이 당연히 안됨 같이 꺼져서
            // 액티브 펄스를 하는 방식에서 매쉬만 끄게 하는걸로
            attackRangeInstance.SetActive(false);

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

    private void ShowRangeAttackRange(bool show, EnemyType enemyType, AttackPatternType AttackPatternType) // 쇼
    {
        Debug.Log("레인지 어택 호출 하는지?");

        GameObject attackPrefab = null;
        Vector3 attackOffset = GetAttackOffset(enemyType, AttackPatternType);

        switch (AttackPatternType)
        {
            case AttackPatternType.A:
                Debug.Log("원거리 프리펩 A 할당 되었나");
                attackPrefab = AttackPatternTypeAPrefab;
                break;

            case AttackPatternType.RangeB:
                attackPrefab = AttackPatternTypeBPrefab;
                break;

            case AttackPatternType.RangeC:
                attackPrefab = AttackPatternTypeCPrefab;
                break;

            case AttackPatternType.RangeD:
                attackPrefab = AttackPatternTypeDPrefab;
                break;
        }

        if (show)
        {
            if (attackRangeInstance != null) // 추후 수정
            {
                Destroy(attackRangeInstance);
            }

            attackRangeInstance = Instantiate(attackPrefab, transform);

            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    Destroy(cell);
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
            cellInstances.Clear();

            AttackPattern currentPattern = savedPatterns[attackIndex];

            fanShape = attackRangeInstance.GetComponent<FanShape>();
            fanShape.enemyAi = this;

            Vector3 cellSize = fanShape.Return();
            Vector3 offset = new Vector3(cellSize.x + 0.01f, cellSize.y + 0.015f, cellSize.z + 0.01f);


            //if (rangeIndicator != null)
            //{
            //    // 원거리 공격 범위 설정
            //    rangeIndicator.transform.position = transform.position; // 적 위치로 설정
            //    rangeIndicator.transform.localScale = new Vector3(1.5f, 2f, 1.5f); // 적당한 크기로 조정

            //    MeshRenderer meshRenderer = rangeIndicator.GetComponent<MeshRenderer>();
            //    if (meshRenderer != null)
            //    {
            //        meshRenderer.sharedMaterial.color = new Color(1f, 0f, 0f, 0.5f);// 반투명
            //    }
            //    else
            //    {
            //        Debug.LogError("meshRenderer에 재질이 할당되지 않았습니다.");
            //    }

            //    rangeIndicator.SetActive(true); // 범위 표시 활성화
            //}
            //else
            //{
            //    Debug.LogError("meshRenderer가 널임.");
            //}



            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    Vector3 cellPosition = CalculateCellPosition(i, offset, attackOffset, enemyType, AttackPatternType);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, transform.rotation, this.transform); // 몬스터 부모로 설정 추가
                    cell.SetActive(true);
                    cellInstances.Add(cell);

                    cell.transform.rotation = transform.rotation;
                }
            }

            attackRangeInstance.SetActive(false);
        }
        else
        {
            //if (rangeIndicator != null)
            //{
            //    rangeIndicator.SetActive(false); // 범위 표시 비활성화
            //}

            // 범위 표시 비활성화 직후 원거리 공격 날리기 구현
            //detectedPlayer

            player = detectedPlayer.GetComponent<Player>();

            if (player != null)
            {
                Vector3 targetPosition = player.transform.position;

                foreach (GameObject cell in cellInstances)
                {
                    if (cell != null)
                    {
                        cell.transform.position = transform.position; // 시작 위치 설정
                        cell.SetActive(true); // 활성화


                        Rigidbody rb = cell.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            // 임펄스 계산
                            Vector3 direction = (targetPosition - transform.position).normalized;
                            float forceMagnitude = 10f; // 필요한 임펄스 힘 계산
                            rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                        }
                    }
                }
            }

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

        isAttacking = true;

        attackIndex = attackPatternIndex; // 인덱스 바꾸고

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackPatternType.A));
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

        isAttacking = true;
        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackPatternType.B));
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

        isAttacking = true;
        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackPatternType.C));
        return INode.EnemyState.Success;
    }

    INode.EnemyState MelleAttackFour(EnemyType enemyType, int attackPatternIndex)
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

        StartCoroutine(PrepareMeleeAttack(enemyType, AttackPatternType.D));
        return INode.EnemyState.Success;
    }

    //INode.EnemyState RangeAttackOne(EnemyType enemyType, int attackPatternIndex)
    //{
    //    if (isAttacking)
    //        return INode.EnemyState.Failure;

    //    if (detectedPlayer == null)
    //        return INode.EnemyState.Failure;

    //    float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

    //    if (distanceToPlayer >= rangeAttackRange)
    //    {
    //        isAttacking = false;
    //        return INode.EnemyState.Failure;
    //    }

    //    Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
    //    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

    //    if (angleToPlayer > minAngle)
    //        return INode.EnemyState.Failure;

    //    isAttacking = true;
    //    attackIndex = attackPatternIndex;


    //    //attackIndex = 0; // 임시로 0
    //    // 근데 레인지어택인덱스도 지금 0으로 설정 되어있긴함
    //    //attackIndex = attackPatternIndex;

    //    //StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.A)); // 이거 임시임
    //    StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.RangeA));
    //    return INode.EnemyState.Success;
    //}

    INode.EnemyState RangeAttackTwo(EnemyType enemyType, int attackPatternIndex)
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

        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.RangeB));
        return INode.EnemyState.Success;
    }

    INode.EnemyState RangeAttackThree(EnemyType enemyType, int attackPatternIndex)
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

        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.RangeC));
        return INode.EnemyState.Success;
    }

    INode.EnemyState RangeAttackFour(EnemyType enemyType, int attackPatternIndex)
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

        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.RangeD));
        return INode.EnemyState.Success;
    }


    #endregion

    #region 페이즈 체크

    private bool IsPhaseOne()
    {
        if (!isTwoPhase && HP <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true;

            Transform magicObject = transform.Find("Magic"); // 일단 곰만
            if (magicObject != null)
            {
                magicObject.gameObject.SetActive(true);
            }
          
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

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeAttackRange);

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
                    //Debug.Log("온어택 호출이 안됐으면 너가 그 몬스터의 애니메이션 이벤트를 안넣었겠지?");
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