using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("PlayerStat ¿¬°á")]
    public PlayerStat stat;

    public float evadeTimer = 0f;
    public float evadePoint { get; set; } = 0;

    public bool isGroggyAttack { get; set; }

    public bool canCombo { get; set; }
    public bool isAttack { get; set; }

    public GameObject enemy { get; private set; }
    public Rigidbody rigid { get; private set; }
    public BoxCollider colldier { get; private set; }
    public CinemachineVirtualCamera virtualCamera;
    public Animator animator { get; private set; }// animator test code

    public float MoveDistance
    {
        get
        {
            return colldier.bounds.size.y * 2;
        }
    }
    public float DistanceToEnemy
    {
        get
        {
            if (enemy == null)
            {
                return 0f;
            }
            return Vector3.Distance(transform.position, enemy.transform.position);
        }
    }

    #region TestData
    public Slider slider;
    public float attackRange = 2f;
    public int maxComboCount = 4;
    public int comboCount = 0;
    public bool comboSuccess = false;
    public float comboSuccessRate = 0.8f;
    public enum playerAttackState
    {
        before, 
    }
    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        colldier = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>(); // animator test code
    }

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag(Tags.enemy);
    }
}