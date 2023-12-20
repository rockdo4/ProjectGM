using UnityEngine;


public abstract class Effect : MonoBehaviour
{
    [Header("이펙트 유지 시간")]
    [Tooltip("파티클 시스템은 무시")]
    [SerializeField]
    private float duration;
    [Header("이펙트 오브젝트")]
    public GameObject prefab;
    //[Header("이펙트 강제 중지 여부")]
    //[Tooltip("유지 시간과 무관하게 행동이 끝나면 이펙트도 사라짐")]
    //public bool useForceStop = false; // 생각안남...

    private float timer = 0f;
    private bool isPlay = false;

    private void Awake()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (!isPlay)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > duration)
        {
            PlayEnd();
        }
    }

    public virtual void PlayEnd()
    {
        gameObject.SetActive(isPlay = false);
    }

    public virtual void PlayStart(Vector3 direction = default)
    {
        timer = 0f;
        gameObject.SetActive(isPlay = true);
    }

    public abstract void Init(Transform targetTransform = null);
}
