using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int[] attackPattern1 = new int[] { 1, 2 };
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
    private float attackPreparationTime = 2f;
    [SerializeField]
    private Material attackRangeMaterial;

    private bool isPreparingAttack = false;

    [Header("범위 공격의 시각화")]
    [SerializeField]
    public GameObject attackRangePrefab;
    private GameObject attackRangeInstance;

    private Player player;

    private Rigidbody rigidbody;


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
        // rigidbody = GetComponent<Rigidbody>();
        GetMonsterSize();
    }

    private void Update()
    {
        if (!hasRoared) // 다시 추가
            return;

        if (isAttacking) // 밀리어택에서 별도로 관리하기로 수정
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttacking = false;
                attackTimer = 0f;
            }
        }

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

    private void FixedUpdate()
    {
    }

    #region

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

    IEnumerator PrepareMeleeAttack()
    {
        isPreparingAttack = true;
        ShowAttackRange(true);

        yield return new WaitForSeconds(attackPreparationTime);

        ShowAttackRange(false);
        isPreparingAttack = false;
    }

    private void ShowAttackRange(bool show)
    {
        if (show)
        {
            if (attackRangeInstance == null)
            {
                attackRangeInstance = Instantiate(attackRangePrefab, transform.position, Quaternion.identity);
                Vector3 monsterSize = GetMonsterSize();

                float rangeSize = Mathf.Max(monsterSize.x, monsterSize.z); // 가로와 세로 중 더 큰 값을 사용
                attackRangeInstance.transform.localScale = new Vector3((rangeSize * 0.1f), 0.1f, (rangeSize * 0.1f));
            }
            attackRangeInstance.SetActive(true);
        }
        else
        {
            if (attackRangeInstance != null)
                attackRangeInstance.SetActive(false);
        }

    }

    private Vector3 GetMonsterSize()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Vector3 size = collider.bounds.size;
            Debug.Log("Monster Size: " + size);
            return size;
        }
        return Vector3.one; // 기본 크기 반환
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

        if (detectedPlayer != null && // 이 if
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange && !isAttacking)
        {
            Debug.Log(isTwoPhase ? "페이즈2 공격A" : "페이즈1 공격A");

            isAttacking = true;

            StartCoroutine(PrepareMeleeAttack());

            player = detectedPlayer.GetComponent<Player>();
            if (player != null)
            {
                // 애니메이션 출력 시간동안은 멈추다가, 그 후에 isAttacking를 false변환
                animator.SetTrigger("MeleeAttack_A");
                isAttacking = false;
                player.TakeDamage(meleeAttackPower);
            }

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

        Debug.Log(isTwoPhase ? "페이즈2 공격B" : "페이즈1 공격B");


        if (detectedPlayer != null && // 이 if
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange && !isAttacking)
        {
            isAttacking = true;

            StartCoroutine(PrepareMeleeAttack());

            animator.SetTrigger("MeleeAttack_B");

            player = detectedPlayer.GetComponent<Player>();
            if (player != null)
            {
                isAttacking = false;
                player.TakeDamage(meleeAttackPower);
            }

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

        Debug.Log(isTwoPhase ? "페이즈2 공격C" : "이상함");


        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;

            StartCoroutine(PrepareMeleeAttack());

            animator.SetTrigger("MeleeAttack_C");

            player = detectedPlayer.GetComponent<Player>();
            if (player != null)
            {
                isAttacking = false;
                player.TakeDamage(meleeAttackPower);
            }

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

        if (detectedPlayer != null)
        {
            animator.SetFloat("MoveSpeed", 0.5f);

            Vector3 direction = (detectedPlayer.position - transform.position).normalized;
            rigidbody.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation(direction);
            rigidbody.MoveRotation(rotation);

            return INode.EnemyState.Running;
        }

        return INode.EnemyState.Failure;
    }
    #endregion

    #region


    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color =  Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color =  Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }

}
