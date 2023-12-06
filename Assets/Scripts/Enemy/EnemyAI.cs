using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAI : LivingObject
{
    private int[] bearAttackPatternPhaseOne = new int[] { 1, 2 }; // ab
    private int[] bearAttackPatternPhaseTwo = new int[] { 1, 2, 3, 2, 3 }; // abcbc

    private int[] tigerAttackPatternPhaseOne = new int[] { 3, 3, 1 }; // cca
    private int[] tigerAttackPatternPhaseTwo = new int[] { 3, 3, 1, 2, 1 }; // ccaba 공격순서

    private int[] spiderAttackPatternPhaseOne = new int[] { 1, 1, 2 }; // aab
    private int[] spiderAttackPatternPhaseTwo = new int[] { 1, 2, 3}; // abc

    private int[] wolfAttackPatternPhaseOne = new int[] { 1, 1, 2 }; // aab
    private int[] wolfAttackPatternPhaseTwo = new int[] { 1, 2, 1, 2, 2 }; // ababb

    private int meleeAttackIndexOne = 0;
    private int meleeAttackIndexTwo = 1;
    private int meleeAttackIndexThree = 2;

    [Header("범위 공격의 시각화 프리펩")]
    public GameObject attackRangePrefab;

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

    public enum EnemyType
    {
        Bear,
        Tiger,
        Spider,
        Wolf,

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
            case EnemyType.Tiger:
                BTRunner = new BehaviorTreeRunner(TigerBT());
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
    INode TigerBT()
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
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Tiger, tigerAttackPatternPhaseOne))
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(EnemyType.Tiger, tigerAttackPatternPhaseTwo)),
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

    IEnumerator PrepareMeleeAttack(EnemyType enemytype, string attackType)
    {
        isPreparingAttack = true;
        ShowAttackRange(true);

        for (float speed = 0.5f; speed >= 0.0f; speed -= Time.deltaTime / attackPreparationTime)
        {
            animator.SetFloat("MoveSpeed", speed);
            yield return null;
        }

        yield return new WaitForSeconds(attackPreparationTime);

        ShowAttackRange(false);
        isPreparingAttack = false;

        player = detectedPlayer.GetComponent<Player>();

        if (player != null)
        {
            string animationTrigger = $"{enemytype}{attackType}";
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
                Debug.Log("공격 애니메이션 재생 시간 : " + stateInfo.length);
                yield return new WaitForSeconds(stateInfo.length);
                isAttacking = false; // 공격상태 해제를 여기서 해주지 않으면 코루틴을 하는 의미가 없어짐
                // 하지만 다음 공격버그가 생긴다면 여기일듯
            }
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

                    // 별개의 투명 콜라이더 공격범위의 시각화 Cell과 일치하게
                    GameObject colliderObject = new GameObject("AttackCollider");
                    colliderObject.AddComponent<AttackCell>();
                    BoxCollider collider = colliderObject.AddComponent<BoxCollider>();

                    collider.size = new Vector3(2.1f, 0.1f, 2.1f);
                    collider.isTrigger = true;
                    colliderObject.transform.position = cellPosition;

                    colliderObjects.Add(colliderObject);
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
                    cell.SetActive(false); // 삭제는 조금 있다
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

        Debug.Log(isTwoPhase ? "페이즈2 공격A 준비" : "페이즈1 공격A 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, "A"));
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

        Debug.Log(isTwoPhase ? "페이즈2 공격B 준비" : "페이즈1 공격B 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, "B"));
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

        Debug.Log(isTwoPhase ? "페이즈2 공격C 준비" : "페이즈1 공격C 준비");
        isAttacking = true;

        attackIndex = attackPatternIndex;

        StartCoroutine(PrepareMeleeAttack(enemyType, "C"));
        return INode.EnemyState.Success;
    }

    #endregion

    #region 페이즈 체크

    private bool IsPhaseOne()
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

    #endregion

    #region 공격 타일 계산

    Vector3 CalculateCellPosition(int index) // 칼큘
    {
        Renderer renderer = attackRangePrefab.GetComponent<Renderer>();// 어택 레인지 프리펩
        if (renderer == null)
        {
            Debug.LogError("attackRangePrefab에 Renderer 컴포넌트가 없습니다.");
            return Vector3.zero;
        }

        Vector3 cellSize = renderer.bounds.size; // 프리펩의 로컬 사이즈 받아오기
        float offset = cellSize.x + 0.01f;

        int x = index % 3;
        int z = index / 3;

        Vector3 actualPosition = new Vector3(x * offset - (offset * 1), 0f, z * offset + meleeAttackRange); // 지금은 임시로 y축 위치를 0f로 함
        // 왜냐하면 테스트씬의 그라운드의 y포지션이 -0.01f임
        // 하드코딩 1 오프셋 관련 - (offset * 4) 나중에 수식 활용
        // int x = index % 10; 이것도 마찬가지
        // 뒤에 나눠주는 값 셀이 100개라면 나누는 수는 10이어야하니까 루트 씌워야됨
        actualPosition = transform.rotation * actualPosition;
        return transform.position + actualPosition;
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

                Vector3 actualPosition = new Vector3((x - 1) * offset, 0f, (z + 1) * offset);
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
                // Debug.Log("AttackCell: " + (attackCell != null) + ", PlayerInside: " + (attackCell != null && attackCell.playerInside));

                if (attackCell != null && attackCell.playerInside)
                {
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