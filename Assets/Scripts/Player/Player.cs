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
    private Collider colldier;
    
    private float evadeTimer = 0f;
    private int evadePoint;
    private float EvadeDistance
    {
        get
        {
            return colldier.bounds.size.y * 2;
        }
    }
    private Coroutine coEvade;

    #region TestData
    public Slider slider;
    private Color evadeColor = Color.white;
    private Color evadeSuccessColor = Color.yellow;
    private Color justEvadeSuccessColor = Color.green;
    private Color hitColor = Color.red;
    private Color originalColor;
    private MeshRenderer ren;
    #endregion

    private void Awake()
    {
        ren = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        colldier = GetComponent<Collider>();
    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += Attack;
        TouchManager.Instance.SwipeListeners += Evade;
        TouchManager.Instance.HoldListeners += AutoAttack;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            // Collider의 크기 얻기
            Vector3 size = collider.bounds.size;
            Debug.Log("Rigidbody의 Collider 크기: " + size);
        }

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

        //var distance = Vector3.Distance(transform.position, enemy.transform.position);
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

        var position = rigid.position;
        evadeTimer = 0f;
        while (evadeTimer < stat.evadeTime)
        {
            evadeTimer += Time.fixedDeltaTime;

            Debug.Log(Vector3.Distance(rigid.position, position));
            if (Vector3.Distance(rigid.position, position) <= EvadeDistance)
            {
                rigid.MovePosition(rigid.position + rigid.rotation * direction * stat.MoveSpeed * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();
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