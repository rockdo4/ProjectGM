using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
public class EnemyAI : LivingObject
{

    [Header("FanShape 프리펩 유형")]
    public GameObject[] fanShapePrefabs;

    private ObjectPool<FanShape>[] fanShapePools;
    private List<FanShape> activeFanShapes = new List<FanShape>();


    [Header("곰 페이즈1 공격 패턴")]
    public int[] bearAttackPatternPhaseOne = new int[] { 1, 2 }; // ab

    [Header("곰 페이즈2 공격 패턴")]
    public int[] bearAttackPatternPhaseTwo = new int[] { 1, 2, 3, 2, 3 };

    [Header("에일리언 페이즈1 공격 패턴")]
    public int[] alienAttackPatternPhaseOne = new int[] { 1, 2 };

    [Header("에일리언 페이즈2 공격 패턴")]
    public int[] alienAttackPatternPhaseTwo = new int[] { 1, 2, 3 };

    [Header("멧돼지 페이즈1 공격 패턴")]
    public int[] boarAttackPatternPhaseOne = new int[] { 8 };

    [Header("멧돼지 페이즈2 공격 패턴")]
    public int[] boarAttackPatternPhaseTwo = new int[] { 1, 2, 3};

    [Header("멧돼지 원거리 공격 발사 간격")]
    public float IntervalTime = 0.15f;
    
    [Header("늑대 페이즈1 공격 패턴")]
    public int[] wolfAttackPatternPhaseOne = new int[] { 1, 2, 2, 1, 1, 2}; // 테스트로 레인지A 인덱스 넣기

    [Header("늑대 페이즈2 공격 패턴")]
    public int[] wolfAttackPatternPhaseTwo = new int[] { 1, 2 };

    [Header("거미 페이즈1 공격 패턴")]
    public int[] spiderAttackPatternPhaseOne = new int[] { 1, 2, 3 };

    [Header("거미 페이즈2 공격 패턴")]
    public int[] spiderAttackPatternPhaseTwo = new int[] { 1, 2, 3 };

    [Header("드래곤 페이즈1 공격 패턴")]
    public int[] dragonAttackPatternPhaseOne = new int[] { 1, 2, 3 };

    [Header("드래곤 페이즈2 공격 패턴")]
    public int[] dragonAttackPatternPhaseTwo = new int[] { 1, 2, 3 };

    private int EnemyMeleeAttackIndexOne = 0;
    private int EnemyMeleeAttackIndexTwo = 1;
    private int EnemyMeleeAttackIndexThree = 2;
    private int EnemyMeleeAttackIndexFour = 3;
    private int EnemyMeleeAttackIndexFive = 4;
    private int EnemyMeleeAttackIndexSix = 5;

    private int EnemyRangedAttackIndexOne = 6;
    private int EnemyRangedAttackIndexTwo = 7;
    private int EnemyRangedAttackIndexThree = 8;
    private int EnemyRangedAttackIndexFour = 9;

    private FanShape fanshape;

    [Header("공격 준비 시간 - 초기값(패턴에 따로 시간 지정해주지 않았을때만 적용)")]
    [SerializeField]
    public List<AttackPreparationTime> attackPreparationTimes;

    [Header("A공격 타입 프리펩")]
    public GameObject AttackPatternTypeAPrefab;

    [Header("B공격 타입 프리펩")]
    public GameObject AttackPatternTypeBPrefab;

    [Header("C공격 타입 프리펩")]
    public GameObject AttackPatternTypeCPrefab;

    [Header("D공격 타입 프리펩")]
    public GameObject AttackPatternTypeDPrefab;

    [Header("원거리 A공격 타입 프리펩")]
    public GameObject RangeAttackPatternTypeAPrefab;

    [Header("원거리 B공격 타입 프리펩")]
    public GameObject RangeAttackPatternTypeBPrefab;

    [Header("원거리 C공격 타입 프리펩")]
    public GameObject RangeAttackPatternTypeCPrefab;

    [Header("원거리 D공격 타입 프리펩")]
    public GameObject RangeAttackPatternTypeDPrefab;

    [Header("공격 대기시간 기본값")]
    [SerializeField]
    public float attackPreparationTime = 1f;

    [Header("몬스터의 탐지범위")]
    [SerializeField]
    private float detectRange = 100f;

    [Header("몬스터의 근접 공격 사거리")]
    [SerializeField]
    private float meleeAttackRange = 3f;

    [Header("몬스터의 원거리 공격 사거리")]
    [SerializeField]
    private float rangeAttackRange = 6f;

    [Header("포효 시간 - 몬스터가 처음 등장할때만 작동")]
    [SerializeField]
    private float roarDuration = 3f;

    [Header("플레이어 할당해야 몬스터가 쫒아다님")]
    [SerializeField]
    private LayerMask playerLayerMask;

    [Header("몬스터의 타입")]
    public EnemyType enemyType;

    [Header("공격 조건중 몬스터가 플레이어를 바라볼때의 최소각도")]
    public float minAngle = 10f;

    [Header("몬스터가 고개를 돌리는 속도")]
    public float rotationSpeed = 8f;

    BehaviorTreeRunner BTRunner;
    public Transform detectedPlayer;
    private Player player;

    private Animator animator;
    private GameObject attackRangeInstance;

    private int phaseOneAttackSequence = 0;
    private int phaseTwoAttackSequence = 0;

    private float phaseTwoHealthThreshold;

    private bool hasRoared;
    private bool isTwoPhase;
    private bool isAttacking = false;
    private bool isPreparingAttack = false;

    private int attackIndex = -1;
    private float grogyTimer = 5f;

    public bool[] attackGrid = new bool[9];
    public List<AttackPattern> savedPatterns = new List<AttackPattern>();
    private List<GameObject> cellInstances = new List<GameObject>(); // 셀 인스턴스들을 저장할 리스트
    private List<GameObject> colliderObjects = new List<GameObject>();

    FanShape fanShape = null;
    private bool isDie;
    private int poolIndex;

    [Serializable]
    public struct AttackPreparationTime
    {
        public EnemyType enemyType;
        public AttackPatternType attackPatternType;
        public float preparationTime;
    }

    public float CurrentPreparationTime { get; private set; }

    public enum EnemyType
    {
        Bear,
        Alien,
        Boar,
        Wolf,
        Spider,
        Dragon
    }

    public enum AttackPatternType
    {
        A,
        B,
        C,
        D,
        E,
        F,
        RangeA,
        RangeB,
        RangeC,
        RangeD,
        RangeE,
        RangeF,
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

    public FanShape GetFanShape(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= fanShapePools.Length)
        {
            Debug.LogError("잘못된 FanShape 유형 인덱스: " + typeIndex);
            return null;
        }
        return fanShapePools[typeIndex].Get();
    }

    public void ReleaseFanShape(int typeIndex, FanShape fanShape)
    {
        if (typeIndex < 0 || typeIndex >= fanShapePools.Length)
        {
            Debug.LogError("잘못된 FanShape 유형 인덱스: " + typeIndex);
            return;
        }
        fanShapePools[typeIndex].Release(fanShape);
    }

    private void Start()
    {
        StartCoroutine(RoarInit());
    }

    IEnumerator RoarInit()
    {
        hasRoared = false;
        CameraManager.Instance.SetCameraWithTag(Tags.enemy);
        animator.SetTrigger("Roar");
        yield return new WaitForSeconds(roarDuration);
        CameraManager.Instance.SetCameraWithTag(Tags.player);
        hasRoared = true;
    }

    private ObjectPool<FanShape> InitializeFanShapePool(GameObject prefab)
    {
        return new ObjectPool<FanShape>(
       createFunc: () =>
       {
           var obj = Instantiate(prefab);
           return obj.GetComponent<FanShape>();
       },
       actionOnGet: (obj) => { obj.gameObject.SetActive(true); },
       actionOnRelease: (obj) => { obj.gameObject.SetActive(false); },
       null,
       defaultCapacity: 10, // 초기 용량
       maxSize: 20       // 최대 용량
   );
    }

    protected override void Awake()
    {
        fanShapePools = new ObjectPool<FanShape>[fanShapePrefabs.Length];
        for (int i = 0; i < fanShapePrefabs.Length; i++)
        {
            fanShapePools[i] = InitializeFanShapePool(fanShapePrefabs[i]);
        }

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
            case EnemyType.Boar:
                BTRunner = new BehaviorTreeRunner(BoarBT());
                break;
            case EnemyType.Wolf:
                BTRunner = new BehaviorTreeRunner(WolfBT());
                break;
            case EnemyType.Spider:
                BTRunner = new BehaviorTreeRunner(SpiderBT());
                break;
            case EnemyType.Dragon:
                BTRunner = new BehaviorTreeRunner(DragonBT());
                break;

        }
        animator = GetComponent<Animator>();
        isDie = false;
    }

    private void Update()
    {
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

    INode BoarBT()
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
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Boar, boarAttackPatternPhaseOne))
                                        }
                                    ),

                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new InverterNode(new ConditionNode(IsPhaseOne)),
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Boar, boarAttackPatternPhaseTwo)),
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
                            )
                        }
                    )
                )
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
                            )
                        }
                    )
                )
            }
        );
    }
    #endregion

    #region 드래곤 행동트리
    INode DragonBT()
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
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Dragon, dragonAttackPatternPhaseOne))
                                        }
                                    ),

                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new InverterNode(new ConditionNode(IsPhaseOne)),
                                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Dragon, dragonAttackPatternPhaseTwo)),
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
        // 공격 준비부터 달리는 모션 나오는거 제거
        animator.SetFloat("MoveSpeed", 0f);

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

            case 5:
                result = RangeAttackOne(enemytype, EnemyRangedAttackIndexOne);
                break;

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

    IEnumerator PrepareMeleeAttack(EnemyType enemytype, AttackPatternType attackPatternType) // 프리페어
    {
        if (activeFanShapes != null)
        {
            foreach (var fanShape in activeFanShapes)
            {
                MeshRenderer meshRenderer = fanShape.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                }

                fanShapePools[poolIndex].Release(fanShape);
            }
        }

        isPreparingAttack = true;
        ShowMeleeAttackRange(true, enemytype, attackPatternType);

        float specificPreparationTime = attackPreparationTime; // 기본값으로 초기화

        foreach (var preparationTime in attackPreparationTimes)
        {
            if (preparationTime.enemyType == enemytype && preparationTime.attackPatternType == attackPatternType)
            {
                specificPreparationTime = preparationTime.preparationTime;
                CurrentPreparationTime = specificPreparationTime;
                break;
            }
        }

        //Debug.Log("이번 공격 대기시간 : "  + specificPreparationTime);
        yield return new WaitForSeconds(specificPreparationTime);

        ShowMeleeAttackRange(false, enemytype, attackPatternType);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            //Debug.Log(attackIndex);
            //Debug.Log(attackPatternType);

            string animationTrigger = $"{"Attack_"}{attackPatternType}"; //attackPatternType 이게 코드없이 되냐?
            IsAnimationRunning(animationTrigger);
            //StartCoroutine(IsAnimationRunning(animationTrigger));
        }
    }

    IEnumerator PrepareRangedAttack(EnemyType enemytype, AttackPatternType attackPatternType) // 원거리
    {
        isPreparingAttack = true;
        ShowProjectileAttackRange(true, enemytype, attackPatternType);

        float specificPreparationTime = attackPreparationTime;

        foreach (var preparationTime in attackPreparationTimes)
        {
            if (preparationTime.enemyType == enemytype && preparationTime.attackPatternType == attackPatternType)
            {
                specificPreparationTime = preparationTime.preparationTime;
                CurrentPreparationTime = specificPreparationTime;
                break;
            }
        }

        yield return new WaitForSeconds(specificPreparationTime);

        switch (attackPatternType)
        {
            case AttackPatternType.RangeA:
                ShowProjectileAttackRange(false, enemytype, attackPatternType); // 기존 공격 패턴 A
                break;
            case AttackPatternType.RangeB:
                yield return StartCoroutine(RangeAttackPatternB());
                break;
            case AttackPatternType.RangeE:
                yield return StartCoroutine(RangeAttackPatternE());
                break;
            case AttackPatternType.RangeF:
                yield return StartCoroutine(RangeAttackPatternF());
                break;
        }

        isPreparingAttack = false;
        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            // 임시 애니메이션 임시 원거리 공격 애니메이션 임시임시
            //string animationTrigger = $"{"Attack_"}{"A"}";
            string animationTrigger = $"{"Attack_"}{attackPatternType}";
            IsAnimationRunning(animationTrigger);

        }
    }

    private void IsAnimationRunning(string stateName)
    {
        if (animator != null)
        {
            animator.SetTrigger(stateName);
        }
    }

    private void AnimationIsOver() // 애니메이션 이벤트 공격 애니메이션 끝나고 나서.
    {
        isAttacking = false; // 
    }

    private Vector3 GetAttackOffset(EnemyType enemyType, AttackPatternType AttackPatternType)
    {
        if (enemyType == EnemyType.Bear)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.A:
                    return new Vector3(0f, 0f, -4f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -4f);
                case AttackPatternType.C:
                    return new Vector3(0f, 0f, -4f);
                case AttackPatternType.D:
                    return new Vector3(0f, 0f, -4f);
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
                case AttackPatternType.D:
                    return new Vector3(0f, 0f, -4f);
                default: return Vector3.zero;
            }
        }

        if (enemyType == EnemyType.Boar)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.A:
                    return new Vector3(0f, 0f, -2f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f);
                case AttackPatternType.C:
                    return new Vector3(0f, 0f, -5f);
                //case AttackPatternType.RangeA:
                //    return new Vector3(0f, -4f, -4f);
                default: return Vector3.zero;
            }
        }

        if (enemyType == EnemyType.Wolf)
        {
            switch (AttackPatternType)
            {
                case AttackPatternType.A:
                    return new Vector3(0f, 0f, -5f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f);
                default: return Vector3.zero;
            }
        }

        if (enemyType == EnemyType.Spider)
        {
            switch (AttackPatternType)
            {
                //case AttackPatternType.A:
                //    return new Vector3(0f, 0f, -7f);
                case AttackPatternType.B:
                    return new Vector3(0f, 0f, -2f);
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

        if (index == 4)
        {
            actualPosition -= realoffset;
        }

        if (enemyType == EnemyType.Alien && AttackPatternType == AttackPatternType.C)
        {
            switch (index)
            {
                case 1: actualPosition += new Vector3(0, 0, 4f); break; // 뒤
                case 3: actualPosition += new Vector3(4f, 0, 0); break; // 오른쪽
                case 5: actualPosition += new Vector3(-4f, 0, 0); break; // 왼쪽
                case 7: actualPosition += new Vector3(0, 0, -4f); break; // 앞
            }
        }
        if (enemyType == EnemyType.Spider && AttackPatternType == AttackPatternType.C)
        {
            switch (index)
            {
                case 1: actualPosition += new Vector3(0, 0, 4f); break; // 뒤
                case 3: actualPosition += new Vector3(4f, 0, 0); break; // 오른쪽
                case 5: actualPosition += new Vector3(-4f, 0, 0); break; // 왼쪽
                case 7: actualPosition += new Vector3(0, 0, -4f); break; // 앞
            }
        }

        if (enemyType == EnemyType.Spider && AttackPatternType == AttackPatternType.A)
        {
            switch (index)
            {
                case 7: actualPosition += new Vector3(0f, 0, -4f); break; 
                case 8: actualPosition += new Vector3(0, 0, -3f); break;
                case 9: actualPosition += new Vector3(-4f, 0, 0f); break;
            }
        }

        actualPosition = transform.rotation * actualPosition;

        return transform.position + actualPosition;
    }

    #endregion

    private int GetPoolIndexForAttackPatternType(AttackPatternType attackPatternType)
    {
        switch (attackPatternType)
        {
            case AttackPatternType.A:
                return 0;

            case AttackPatternType.B:
                return 1;

            case AttackPatternType.C:
                return 2;

            case AttackPatternType.D:
                return 3;

            case AttackPatternType.E:
                return 4;

            case AttackPatternType.F:
                return 5;

            case AttackPatternType.RangeA:
                return 6;

            case AttackPatternType.RangeB:
                return 7;

            case AttackPatternType.RangeC:
                return 8;

            case AttackPatternType.RangeD:
                return 9;
        }

        return 0;
    }

    Quaternion CalculateRotation(EnemyType enemyType, AttackPatternType attackPatternType, Vector3 monsterPosition, Vector3 fanShapePosition)
    {
        Vector3 directionToMonster = (monsterPosition - fanShapePosition).normalized;
        Quaternion initialRotation = Quaternion.LookRotation(directionToMonster);
        Quaternion additionalRotation = Quaternion.identity;

        // 식 변경으로 기본값은 180f

        switch (enemyType)
        {
            case EnemyType.Bear:
                if (attackPatternType == AttackPatternType.A)
                    additionalRotation = Quaternion.Euler(0f, 150f, 0f);
                else if (attackPatternType == AttackPatternType.B)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);
                else if (attackPatternType == AttackPatternType.C)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);
                else if (attackPatternType == AttackPatternType.D)
                    additionalRotation = Quaternion.Euler(0f, 125f, 0f);
                break;

            case EnemyType.Boar:
                if (attackPatternType == AttackPatternType.A)
                    additionalRotation = Quaternion.Euler(0f, 135f, 0f);

                else if (attackPatternType == AttackPatternType.B)
                    additionalRotation = Quaternion.Euler(0f, 105f, 0f);

                else if (attackPatternType == AttackPatternType.C)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);

                break;

            case EnemyType.Alien:
                if (attackPatternType == AttackPatternType.A)
                    additionalRotation = Quaternion.Euler(-90f, 0f, 0f);
                else if (attackPatternType == AttackPatternType.B)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);
                else if (attackPatternType == AttackPatternType.C)
                    additionalRotation = Quaternion.Euler(0f, 140f, 0f);
                else if (attackPatternType == AttackPatternType.D)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);
                break;

            case EnemyType.Spider:
                if (attackPatternType == AttackPatternType.A)
                    additionalRotation = Quaternion.Euler(0f, 180f, 0f);
                else if (attackPatternType == AttackPatternType.B)
                    additionalRotation = Quaternion.Euler(0f, 120f, 0f);
                else if (attackPatternType == AttackPatternType.C)
                    additionalRotation = Quaternion.Euler(0f, 140f, 0f);
                break;

            case EnemyType.Wolf:
                if (attackPatternType == AttackPatternType.A)
                    additionalRotation = Quaternion.Euler(0.172f, 180f, 0f);

                else if (attackPatternType == AttackPatternType.B)
                    additionalRotation = Quaternion.Euler(0f, 120f, 0f);
                break;

            default:
                break;
        }

        return initialRotation * additionalRotation;
    }


    private void ShowMeleeAttackRange(bool show, EnemyType enemyType, AttackPatternType AttackPatternType) // 쇼
    {
        Vector3 attackOffset = GetAttackOffset(enemyType, AttackPatternType);
        poolIndex = GetPoolIndexForAttackPatternType(AttackPatternType);

        if (show)
        {
            //if (activeFanShapes != null)
            //{
            //    foreach (var fanShape in activeFanShapes)
            //    {
            //        MeshRenderer meshRenderer = fanShape.GetComponent<MeshRenderer>();
            //        if (meshRenderer != null)
            //        {
            //            meshRenderer.enabled = true;
            //        }

            //        fanShape.gameObject.SetActive(false);
            //        fanShapePools[GetPoolIndexForAttackPatternType(AttackPatternType)].Release(fanShape);
            //    }
            //}

            AttackPattern currentPattern = null;

            if (attackIndex >= 0 && attackIndex < savedPatterns.Count)
            {
                currentPattern = savedPatterns[attackIndex]; // 유효성 검사 후 값 할당
            }
            else
            {
                Debug.LogError("attackIndex가 범위를 벗어났습니다: " + attackIndex);
                return;
            }

            if(activeFanShapes != null)
            {
                activeFanShapes.Clear(); // 리스트 초기화
            }

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    FanShape fanShapeInstance = fanShapePools[poolIndex].Get();
                    fanShapeInstance.gameObject.SetActive(true);

                    fanShape = fanShapeInstance.GetComponent<FanShape>();
                    fanShape.enemyAi = this;

                    Vector3 cellSize = fanShape.Return();
                    Vector3 offset = new Vector3(cellSize.x + 0.01f, cellSize.y + 0.015f, cellSize.z + 0.01f);

                    fanShapeInstance.transform.SetParent(transform, false);
                    fanShapeInstance.transform.position = CalculateCellPosition(i, offset, attackOffset, enemyType, AttackPatternType);

                    fanShapeInstance.transform.rotation = CalculateRotation(enemyType, AttackPatternType, transform.position, fanShapeInstance.transform.position);


                    activeFanShapes.Add(fanShapeInstance);
                }
            }

        }
        else
        {
            if (activeFanShapes != null)
            {
                foreach (var fanShape in activeFanShapes)
                {
                    MeshRenderer meshRenderer = fanShape.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = false;
                    }

                    //fanShapePools[poolIndex].Release(fanShape);
                }
            }
        }
    }

    private void ShowProjectileAttackRange(bool show, EnemyType enemyType, AttackPatternType AttackPatternType) // 쇼
    {
        //Debug.Log("레인지 어택 호출 하는지?");
        GameObject attackPrefab = null;
        Vector3 attackOffset = GetAttackOffset(enemyType, AttackPatternType);

        switch (AttackPatternType)
        {
            case AttackPatternType.RangeA:
                attackPrefab = RangeAttackPatternTypeAPrefab;
                break;

            case AttackPatternType.RangeB:
                attackPrefab = RangeAttackPatternTypeBPrefab;
                break;

            case AttackPatternType.RangeC:
                attackPrefab = RangeAttackPatternTypeCPrefab;
                break;

            case AttackPatternType.RangeD:
                attackPrefab = RangeAttackPatternTypeDPrefab;
                break;
        }

        if (show)
        {
            if (attackRangeInstance != null)
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
            cellInstances.Clear();


            AttackPattern currentPattern = savedPatterns[attackIndex];
            Renderer renderer = attackRangeInstance.GetComponent<Renderer>();

            Vector3 cellSize = Vector3.zero;

            switch (AttackPatternType)
            {
                case AttackPatternType.RangeA:
                    cellSize = renderer.bounds.size;
                    break;

                case AttackPatternType.RangeB:
                    cellSize = renderer.bounds.size;
                    break;

                case AttackPatternType.RangeC:
                    if (show)
                    {
                        fanShape = attackPrefab.GetComponent<FanShape>();
                        fanShape.enemyAi = this;
                        CreatePrefabAtPlayer(attackPrefab); // C패턴 하나생성

                        attackRangeInstance.SetActive(false);

                        return;
                    }
                    break;

                case AttackPatternType.RangeD:
                    fanShape = attackPrefab.GetComponent<FanShape>();
                    fanShape.enemyAi = this;
                    cellSize = fanShape.Return();

                    attackRangeInstance.SetActive(false);

                    if (show)
                    {
                        int numberOfPrefabs = 7;
                        float radius = 4.5f;
                        CreatePrefabsAroundPlayer(attackPrefab, numberOfPrefabs, radius);
                        return;
                    }
                    break;
            }

            Vector3 offset = new Vector3(cellSize.x + 0.01f, cellSize.y + 0.015f, cellSize.z + 0.01f);

            for (int i = 0; i < currentPattern.pattern.Length; i++)
            {
                if (currentPattern.pattern[i])
                {
                    Vector3 cellPosition = CalculateCellPosition(i, offset, attackOffset, enemyType, AttackPatternType);
                    GameObject cell = Instantiate(attackRangeInstance, cellPosition, transform.rotation, this.transform); // 몬스터 부모로 설정 추가
                    //cell.AddComponent<EnemyProjectile>();
                    cell.SetActive(true);
                    cellInstances.Add(cell);

                    cell.transform.rotation = transform.rotation;
                }
            }

            attackRangeInstance.SetActive(false);
        }
        else
        {
            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    cell.SetActive(false);
                }
            }

            RangeAttackPatternA();
        }
    }

    private void RangeAttackPatternA()
    {
        if (detectedPlayer != null)
        {
            Vector3 targetPosition = detectedPlayer.transform.position;

            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    cell.transform.position = transform.position;
                    cell.SetActive(true);

                    Rigidbody rb = cell.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 direction = (targetPosition - transform.position).normalized;
                        float forceMagnitude = 10f;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    private IEnumerator RangeAttackPatternB()
    {
        if (detectedPlayer != null)
        {
            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    cell.transform.position = transform.position; // 시작 위치 설정
                    cell.SetActive(true); // 활성화

                    Rigidbody rb = cell.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 targetPosition = detectedPlayer.transform.position;

                        Vector3 direction = (targetPosition - transform.position).normalized;
                        float forceMagnitude = 10f;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                    }

                    yield return new WaitForSeconds(IntervalTime);
                }
            }
        }
    }

    private IEnumerator RangeAttackPatternE()
    {
        if (detectedPlayer != null)
        {
            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    cell.transform.position = transform.position; // 시작 위치 설정
                    cell.SetActive(true); // 활성화

                    Rigidbody rb = cell.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 targetPosition = detectedPlayer.transform.position;

                        Vector3 direction = (targetPosition - transform.position).normalized;
                        float forceMagnitude = 10f;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                    }

                    yield return new WaitForSeconds(IntervalTime);
                }
            }
        }
    }

    private IEnumerator RangeAttackPatternF()
    {
        if (detectedPlayer != null)
        {
            foreach (GameObject cell in cellInstances)
            {
                if (cell != null)
                {
                    cell.transform.position = transform.position; // 시작 위치 설정
                    cell.SetActive(true); // 활성화

                    Rigidbody rb = cell.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 targetPosition = detectedPlayer.transform.position;

                        Vector3 direction = (targetPosition - transform.position).normalized;
                        float forceMagnitude = 10f;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                    }

                    yield return new WaitForSeconds(IntervalTime);
                }
            }
        }
    }

    private void CreatePrefabAtPlayer(GameObject prefab)
    {
        GameObject createdObject = Instantiate(prefab, detectedPlayer.transform.position + new Vector3(0, 0.015f, 0), Quaternion.identity);
        cellInstances.Add(createdObject);
    }

    // 원거리 
    private void CreatePrefabsAroundPlayer(GameObject prefab, int count, float radius)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject createdPrefab;
            Vector3 position;

            if (i == 0)
            {
                position = detectedPlayer.transform.position;
                createdPrefab = Instantiate(prefab, position, Quaternion.identity);
            }
            else
            {
                position = RandomCircle(detectedPlayer.transform.position, radius);
                createdPrefab = Instantiate(prefab, position, Quaternion.identity);
            }

            Vector3 updatedPosition = createdPrefab.transform.position;
            updatedPosition.y += 0.015f;
            createdPrefab.transform.position = updatedPosition;

            cellInstances.Add(createdPrefab); // 리스트에 Add 추가 이거 중요함

            //if (i == 0)
            //{
            //    Instantiate(prefab, detectedPlayer.transform.position, Quaternion.identity);
            //}
            //else
            //{
            //    Vector3 randomPosition = RandomCircle(detectedPlayer.transform.position, radius);
            //    Instantiate(prefab, randomPosition, Quaternion.identity);
            //}
        }
    }

    private Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
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
            // direction.Normalize();
            direction.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // 로테이션 스피드 5f 임시

            return INode.EnemyState.Success;
        }
        detectedPlayer = null;
        return INode.EnemyState.Failure;
    }

    INode.EnemyState TracePlayer() // 트레이스
    {
        if (detectedPlayer != null && !isAttacking && !isPreparingAttack)
        {
            float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);
            //Vector3 direction = (detectedPlayer.position - transform.position).normalized;

            if (distanceToPlayer <= meleeAttackRange)
            {
                //Quaternion rotation = Quaternion.LookRotation(direction);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

                animator.SetFloat("MoveSpeed", 0f);
                return INode.EnemyState.Success;
            }
            else
            {
                Vector3 direction = (detectedPlayer.position - transform.position).normalized;
                animator.SetFloat("MoveSpeed", 0.5f); // 애니메이션 속도 조정
                transform.position += direction * Stat.MoveSpeed * Time.deltaTime;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);



                //animator.SetFloat("MoveSpeed", 0.5f); // 애니메이션 속도 조정
                //transform.position += direction * Stat.MoveSpeed * Time.deltaTime;

                //Quaternion rotation = Quaternion.LookRotation(direction);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }

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

        //Debug.Log(isAttacking);

        //if (enemyType == EnemyType.Boar && 0 == attackPatternIndex)
        //{
        //    Debug.Log("밀리어택 원 멧돼지");

        //}

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

    INode.EnemyState RangeAttackOne(EnemyType enemyType, int attackPatternIndex)
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null)
            return INode.EnemyState.Failure;

        float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

        if (distanceToPlayer >= rangeAttackRange)
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

        //Debug.Log(attackPatternIndex);
        StartCoroutine(PrepareRangedAttack(enemyType, AttackPatternType.RangeA));
        return INode.EnemyState.Success;
    }

    INode.EnemyState RangeAttackTwo(EnemyType enemyType, int attackPatternIndex)
    {
        if (isAttacking)
            return INode.EnemyState.Failure;

        if (detectedPlayer == null)
            return INode.EnemyState.Failure;

        float distanceToPlayer = Vector3.Distance(detectedPlayer.position, transform.position);

        if (distanceToPlayer >= rangeAttackRange)
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

        //Debug.Log(attackPatternIndex);
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

        if (distanceToPlayer >= rangeAttackRange)
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

        if (distanceToPlayer >= rangeAttackRange)
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
        //if (activeFanShapes == null) return;

        //Gizmos.color = Color.red;
        //float raycastDistance = rangeAttackRange;

        //foreach (var fanShapes in activeFanShapes)
        //{
        //    if (fanShapes != null)
        //    {
        //        //Bounds bounds = cell.GetComponent<Renderer>().bounds;
        //        //Gizmos.DrawWireCube(bounds.center, bounds.size);

        //        Vector3 shapesPosition = fanShapes.transform.position;
        //        Vector3 direction = transform.forward * raycastDistance;
        //        Gizmos.DrawRay(shapesPosition, direction);
        //    }
        //}

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

        foreach (var fanShapeInstance in activeFanShapes)
        {
            if (fanShapeInstance != null && fanShapeInstance.isplayerInside)
            {

                // 공격이 무조건 맞아

                // 이상적인거는
                // 공격 맞기 직전까지 판단을해야되잖아
                // 안에있냐? 밖에있냐?


                //Debug.Log("플레이어가 공격 범위 안에 있습니다.");
                //Debug.Log(actualAttackDamage);
                ExecuteAttack(gameObject.GetComponent<EnemyAI>(), player, actualAttackDamage);
                break;
            }
        }
    }

    #endregion

    #region 애니메이션 이벤트후에 실제 데미지 주는 부분 - 영재가 추가
    public void ExecuteAttack(LivingObject attacker, LivingObject defender, float actualAttackDamage)
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