public class TempEnemy : LivingObject
{
    public bool isGroggy = false;
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
