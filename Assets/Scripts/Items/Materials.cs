public class Materials
{
    public enum MatType
    {
        None = -1,

        Weapon_Module = 71,
        Armor_Module,
        Upgrade_Module,
        Core,
    }

    public MatType matType = MatType.None;
    public int count = 0;
    public int Capacity { get; } = 99;
    public int id = -1;

    public Materials(int id, int count = 0)
    {
        this.id = id;

        matType = (MatType)(id / 1000); // 데이터 테이블 연동 형식으로 변경 필요
        this.count = count;
    }

    public bool IncreaseCount(int value)
    {
        if (count + value > Capacity)
        {
            return false;
        }

        count += value;
        return true;
    }

    public bool DecreaseCount(int value)
    {
        if (count - value <= 0)
        {
            return false;
        }

        count -= value;
        return true;
    }
}