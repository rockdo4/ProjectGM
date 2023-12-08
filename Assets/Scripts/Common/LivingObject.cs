using UnityEngine;
using UnityEngine.Events;

public abstract class LivingObject : MonoBehaviour
{
    [Header("Stat 연결")]
    public Stat stat;
    public int HP { get; set; }
    public bool IsGroggy { get; set; }
    [Header("사망 시 이벤트")]
    public UnityEvent OnDeathEvent;

    protected virtual void Awake()
    {
        HP = stat.HP;
        IsGroggy = false;
    }

}