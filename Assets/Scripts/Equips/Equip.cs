using System;

public class Equip
{
    public enum EquipType
    {
        None = -1,

        Weapon = 1,
        Armor,
    }

    public DateTime instanceID;
    public EquipType type = EquipType.None;
    public int id = -1;
    public bool isEquip = false;

    public Equip(EquipType type = EquipType.None, int id = -1, bool isEquip = false)
    {
        instanceID = DateTime.Now;

        this.type = type;
        this.id = id;
        this.isEquip = isEquip;
    }
}

#region Weapon
public enum AttackType
{
    None = -1,

    Hit = 1, // 타격형
    Slash, // 참격형
    Pierce, // 관통형

}
public enum WeaponType
{
    None = -1,
    Tonpa = 1,
    Two_Hand_Sword,
    One_Hand_Sword,
    Spear
}

public class Weapon : Equip 
{
    public AttackType attackType = AttackType.None;
    public WeaponType weaponType = WeaponType.None;

    public Weapon(int id, bool isEquip = false)
        : base(EquipType.Weapon, id, isEquip)
    {
        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable;

        // Define AttackType
        attackType = table[id].property;
        weaponType = table[id].type;
    }
}
#endregion

#region Armor
public struct Skill
{
    public int id;
    public int level;

    public Skill(int id, int level)
    {
        this.id = id;
        this.level = level;
    }
}

public class Armor : Equip
{
    public enum ArmorType
    {
        None = -1,

        Head = 1,
        Chest,
        Pants,
        Belt,
        Glove,
    }

    public ArmorType armorType = ArmorType.None;
    public int setSkill = -1;
    public Skill[] skills = null;
    public int socket = 1;

    public Armor(int id, bool isEquip = false)
        : base(EquipType.Armor, id, isEquip)
    {
        var table = CsvTableMgr.GetTable<ArmorTable>().dataTable;

        armorType = table[id].Armor_type;
        setSkill = table[id].set_skill;
        socket = table[id].socket;

    }
}
#endregion