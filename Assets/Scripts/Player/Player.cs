using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("PlayerStat ¿¬°á")]
    public PlayerStat stat;

    public float evadeTimer = 0f;
    public int evadePoint = 0;
    public bool canEvade = false;
    public bool canCombo = false;
    public bool isAttack = false;

    public GameObject enemy { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Collider colldier { get; private set; }
    public Animator anim; // animator test code

    public CinemachineVirtualCamera virtualCamera;



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
    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        colldier = GetComponent<Collider>();
        anim = GetComponent<Animator>(); // animator test code
    }

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag(Tags.enemy);
    }
}