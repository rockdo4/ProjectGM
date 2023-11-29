using System.Collections;
using UnityEngine;

public class TempEnemy : LivingObject
{
    public EnemyStat Stat
    {
        get
        {
            return stat as EnemyStat;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
