using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ bool������ �ϳ������Ѵ�
/// �� �ൿ ���ϸ��� ���ݽð��� �����Ѵ�
/// ���ݽð��� ������ ���� ���� �ൿ�� �Ѵ�. ��, ������ �ٽ� �ϰų� Ʈ���̽� ���¿� �����Ѵ�
/// ������ ü���� 0���ϰ� �Ǹ� ��� �ൿ�� ���߰� �׾���Ѵ�.
/// </summary>

public class EnemyAI : MonoBehaviour
{
    [Header("Range")]
    [SerializeField]
    float detectRange = 10f;
    [SerializeField]
    float meleeAttackRange = 2f;

    [Header("Movement")]
    [SerializeField]
    float movementSpeed = 10f;

    [Header("Animation")]
    [SerializeField]
    private float roarDuration = 3f;
    private bool hasRoared = false;

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
    private Material attackRangeMaterial; // ���� ������ ǥ���� ����

    private bool isPreparingAttack = false;
    private GameObject attackRangeIndicator;

    private Player player;

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
            if (attackRangeIndicator == null)
            {
                attackRangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                Destroy(attackRangeIndicator.GetComponent<Collider>()); // �浹ü ����

                attackRangeIndicator.transform.localScale = new Vector3(meleeAttackRange * 2, 0.1f, meleeAttackRange * 2);
                attackRangeIndicator.GetComponent<Renderer>().material = attackRangeMaterial;
            }
            attackRangeIndicator.transform.position = transform.position;
            attackRangeIndicator.SetActive(true);
        }
        else
        {
            if (attackRangeIndicator != null)
                attackRangeIndicator.SetActive(false);
        }
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
        if (!hasRoared)
            return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            health -= 20;
            Debug.Log("���Ƿ� ����, ���� ü�� : " + health);
        }

        if (health <= 0)
        {
            animator.SetTrigger("Die");
            return;
        }

        if (isAttacking) // �������϶� ���߸��
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
            // �������� 2���� �߻��ϴ� ����
            // 1. ���۷���Ʈ ȣ�� �ֱ� ����
            // 2. bool���� ���� �߰� ������?
            // 3. 

            BTRunner.Operate();
        }

    }

    #region ���� ������ �׼�

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
                            // �����߿� �÷��̾ ��Ÿ����� ����� �����Ӵ����� �����ϴ� ������ �߻�

                            new ConditionNode(IsBearPhaseOne), // ������ 1 üũ
                            new ActionNode(() => ExecuteAttackPattern(attackPattern1)) // ������ 1 ���� ����
                        }
                    ),

                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new InverterNode(new ConditionNode(IsBearPhaseOne)),
                            new ActionNode(() => ExecuteAttackPattern(attackPattern2)) // ������ 2 ���� ����
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

    #region ���ݳ��

    private bool IsBearPhaseOne()
    {
        if (!isTwoPhase && health <= phaseTwoHealthThreshold)
        {
            isTwoPhase = true;
            Debug.Log("������ 2�� ��ȯ");
            // phaseTwoAttackSequence = 0; // ������ 2�� ���� ������ �ʱ�ȭ
        }
        return !isTwoPhase; // ������ 1�� �ٽ� ��ȯ
    }

    private INode.EnemyState ExecuteAttackPattern(int[] pattern)
    {
        INode.EnemyState result = INode.EnemyState.Failure;

        // ����� ���� ����� ������ �ε��� ����
        int attackSequence = isTwoPhase ? phaseTwoAttackSequence : phaseOneAttackSequence;

        switch (pattern[attackSequence])
        {
            case 1:
                Debug.Log(isTwoPhase ? "������2 ����A" : "������1 ����A");
                result = DoMeleeAttack1();
                break;
            case 2:
                Debug.Log(isTwoPhase ? "������2 ����B" : "������1 ����B");
                result = DoMeleeAttack2();
                break;
            case 3:
                Debug.Log("������2 ����C");
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
        if (isPreparingAttack || isAttacking)
            return INode.EnemyState.Failure;

        StartCoroutine(PrepareMeleeAttack());

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
            return INode.EnemyState.Failure;
        }

        if (detectedPlayer != null && // �� if
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("MeleeAttack_A");

            player = detectedPlayer.GetComponent<Player>();
            if (player != null)
            {
                //player.TakeDamage(meleeAttackPower);
            }

            // ���� ���� ���� �÷��̾� Ž��
            //Vector3 attackCenter = transform.position + transform.forward * (meleeAttackRange / 2);
            //Collider[] hitEnemies = Physics.OverlapBox(attackCenter, new Vector3(1, 1, meleeAttackRange / 2), transform.rotation, playerLayerMask);
            //foreach (var enemyCollider in hitEnemies)
            //{
            //    Player player = enemyCollider.GetComponent<Player>();
            //    if (player != null)
            //    {
            //        player.TakeDamage(meleeAttackPower);
            //    }
            //}
            return INode.EnemyState.Success;
        }

        return INode.EnemyState.Failure;


    }

    INode.EnemyState DoMeleeAttack2()
    {
        if (isPreparingAttack || isAttacking)
            return INode.EnemyState.Failure;

        StartCoroutine(PrepareMeleeAttack());

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
            return INode.EnemyState.Failure;
        }

        if (detectedPlayer != null &&
            Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("MeleeAttack_A");

            player = detectedPlayer.GetComponent<Player>();
            if (player != null)
            {
                //player.TakeDamage(meleeAttackPower);
            }

            //isAttacking = true;
            //movementSpeed = 0;
            //animator.SetTrigger("MeleeAttack_B");

            //Collider[] hitEnemies = Physics.OverlapSphere(transform.position, meleeAttackRange, playerLayerMask);
            //foreach (var enemyCollider in hitEnemies)
            //{
            //    Vector3 directionToEnemy = (enemyCollider.transform.position - transform.position).normalized;
            //    if (Vector3.Angle(transform.forward, directionToEnemy) < 90)
            //    {
            //        Player player = enemyCollider.GetComponent<Player>();
            //        if (player != null)
            //        {
            //            player.TakeDamage(meleeAttackPower);
            //        }
            //    }
            //}
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

        if (isPreparingAttack || isAttacking)
            return INode.EnemyState.Failure;

        StartCoroutine(PrepareMeleeAttack());

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
            return INode.EnemyState.Failure; // ��Ÿ� �� �߰ݻ��� üũ
        }


        //Vector3 attackCenter = transform.position + transform.forward * (meleeAttackRange / 2);
        //Vector3 attackSize = new Vector3(3, 1, 1); // ���� 3ĭ

        //Collider[] hitTargets = Physics.OverlapBox(attackCenter, attackSize / 2, transform.rotation, playerLayerMask);

        //if (hitTargets.Length > 0 && detectedPlayer != null &&
        //    Vector3.Distance(detectedPlayer.position, transform.position) < meleeAttackRange)
        //{

        //    isAttacking = true;
        //    animator.SetTrigger("MeleeAttack_C");

        //    foreach (var target in hitTargets)
        //    {
        //        Player player = target.GetComponent<Player>();
        //        if (player != null)
        //        {
        //            player.TakeDamage(meleeAttackPower);
        //        }
        //    }

        //    return INode.EnemyState.Success;

        //}

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

    #region ���� �� �̵� ���
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
        {// ���߿� ��ߵ� �ӽ���
            // ����?

            animator.SetFloat("MoveSpeed", 0.5f); // 

            transform.position = Vector3.MoveTowards(transform.position, detectedPlayer.position, Time.deltaTime * movementSpeed);
            transform.LookAt(detectedPlayer); // Look At Player code
            return INode.EnemyState.Running;
        }

        return INode.EnemyState.Failure;
    }
    #endregion

    #region  ���ڸ� ���ư��� ���߿� �ٸ������� ����
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
