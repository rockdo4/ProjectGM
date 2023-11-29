public struct Attack
{
    public int Damage { get; private set; }
    public bool IsCritical { get; private set; }
    public bool IsGroggy { get; private set; }

    public Attack(int damage, bool critical, bool groggy = false)
    {
        Damage = damage;
        IsCritical = critical;
        IsGroggy = groggy;
    }
}
