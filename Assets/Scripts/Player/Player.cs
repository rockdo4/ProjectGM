using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("PlayerStat 연결")]
    public PlayerStat stat;

    private GameObject enemy;
    private Rigidbody rigid;

    private float evadeTimer = 0f;
    private int evadePoint;
    private Coroutine coEvade;

    #region TestData
    public Slider slider;
    private float attackRange = 10f;
    private Color evadeColor = Color.white;
    private Color evadeSuccessColor = Color.yellow;
    private Color justEvadeSuccessColor = Color.green;
    private Color hitColor = Color.red;
    private Color originalColor;
    private MeshRenderer ren;
    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        ren = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += Attack;
        TouchManager.Instance.SwipeListeners += Evade;
        TouchManager.Instance.HoldListeners += AutoAttack;

        originalColor = ren.material.color;
        enemy = GameObject.FindGameObjectWithTag(Tags.enemy);

        //Look At Enemy
        CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.LookAt = enemy.transform;
    }

    private void Update()
    {
        if (enemy == null)
        {
            return;
        }

        //Attack
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (evadeTimer < stat.justEvadeTime)
            {
                ren.material.color = justEvadeSuccessColor;
                evadePoint += stat.justEvadePoint;
            }
            else if (evadeTimer >= stat.justEvadeTime && evadeTimer < stat.evadeTime)
            {
                ren.material.color = evadeSuccessColor;
                evadePoint += stat.evadePoint;
            }
            else
            {
                ren.material.color = hitColor;
                evadePoint += stat.hitEvadePoint;
            }
            slider.value = evadePoint;
        }

        transform.LookAt(enemy.transform.position);
    }

    private void Evade()
    {
        var direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Up => Vector3.forward,
            _ => Vector3.zero
        };

        var distance = Vector3.Distance(transform.position, enemy.transform.position);
        //if (direction == Vector3.forward && distance > attackRange)
        //{
        //    //Move
        //}
        //else
        //{
        //    //Evade
        //}

        if (coEvade != null)
        {
            return;
        }
        coEvade = StartCoroutine(CoEvade(direction));
    }

    private IEnumerator CoMove()
    {
        yield return null;
    }

    private IEnumerator CoEvade(Vector3 direction)
    {
        ren.material.color = evadeColor;

        evadeTimer = 0f;
        while (evadeTimer < stat.evadeTime)
        {
            evadeTimer += Time.deltaTime;

            // if 최대거리인가? 움직이지 않고 움직이고

            var position = rigid.position;
            position += rigid.rotation * direction * stat.MoveSpeed * Time.deltaTime;
            rigid.MovePosition(position);

            yield return null;
        }

        coEvade = null;
        ren.material.color = originalColor;
    }

    private void Attack()
    {
        Debug.Log("Tap");
    }

    private void AutoAttack()
    {
        Debug.Log("Hold");
    }
}