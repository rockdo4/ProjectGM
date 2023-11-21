public abstract class Equip
{
    public string name;
    public bool isEquip = false;
    public abstract void Link(Equip equip);
}

public class Weapon : Equip
{
    public enum WeaponID
    {
        None = -1,

        Count,
    }

    public WeaponID id;
    public int offencePower = 0;
    public Gem gem = null;

    public override void Link(Equip equip)
    {
        if (equip is Gem)
        {
            Link(equip as Gem);
        }
        return;
    }

    public void Link(Gem gem)
    {
        if (this.gem != null)
        {
            this.gem.equip = null;
        }
        this.gem = gem;
        gem.equip = this;
    }
}

public class Armor : Equip
{
    public enum ArmorID
    {
        None = -1,

        Count,
    }

    public ArmorID id;
    public int defencePower = 0;
    public Gem gem = null;

    public override void Link(Equip equip)
    {
        if (equip is Gem)
        {
            Link(equip as Gem);
        }
        return;
    }

    public void Link(Gem gem)
    {
        if (this.gem != null)
        {
            this.gem.equip = null;
        }
        this.gem = gem;
        gem.equip = this;
    }
}

public class Gem : Equip
{
    public enum GemID
    {
        None = -1,

        Count,
    }

    public GemID id;
    public Equip equip = null;

    public override void Link(Equip equip)
    {
        if (equip is Gem)
        {
            return;
        }

        equip.Link(this);
    }
}