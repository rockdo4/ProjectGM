using System;

public class Item
{
    public enum ItemType
    {
        None = -1,

        Weapon,
        Armor,
    }

    public DateTime instanceID;
    public ItemType type = ItemType.None;
    public int id = -1;
    public bool isEquip = false;

    public Item(ItemType type = ItemType.None, int id = -1, bool isEquip = false)
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

public class Weapon : Item 
{
    public enum WeaponID
    {
        None = -1,

        Simple_Tonpa_Lv1 = 8100,
        Simple_Tonpa_Lv2,
        Simple_Tonpa_Lv3,
        Simple_Tonpa_Lv4,
        Simple_Tonpa_Lv5,
        Simple_Tonpa_Lv6,
        Simple_Tonpa_Lv7,
        Simple_Tonpa_Lv8,
        Simple_Tonpa_Lv9,
        Simple_Tonpa_Lv10,

        Gold_Tonpa_Lv1 = 8200,
        Gold_Tonpa_Lv2,
        Gold_Tonpa_Lv3,
        Gold_Tonpa_Lv4,
        Gold_Tonpa_Lv5,
        Gold_Tonpa_Lv6,
        Gold_Tonpa_Lv7,
        Gold_Tonpa_Lv8,
        Gold_Tonpa_Lv9,
        Gold_Tonpa_Lv10,

        Go_Work_Sword_Lv1 = 8300,
        Go_Work_Sword_Lv2,
        Go_Work_Sword_Lv3,
        Go_Work_Sword_Lv4,
        Go_Work_Sword_Lv5,
        Go_Work_Sword_Lv6,
        Go_Work_Sword_Lv7,
        Go_Work_Sword_Lv8,
        Go_Work_Sword_Lv9,
        Go_Work_Sword_Lv10,

        Vigil_Sword_Lv1 = 8400,
        Vigil_Sword_Lv2,
        Vigil_Sword_Lv3,
        Vigil_Sword_Lv4,
        Vigil_Sword_Lv5,
        Vigil_Sword_Lv6,
        Vigil_Sword_Lv7,
        Vigil_Sword_Lv8,
        Vigil_Sword_Lv9,
        Vigil_Sword_Lv10,

        Glory_Sword_Lv1 = 8500,
        Glory_Sword_Lv2,
        Glory_Sword_Lv3,
        Glory_Sword_Lv4,
        Glory_Sword_Lv5,
        Glory_Sword_Lv6,
        Glory_Sword_Lv7,
        Glory_Sword_Lv8,
        Glory_Sword_Lv9,
        Glory_Sword_Lv10,

        Darkness_Sword_Lv1 = 8600,
        Darkness_Sword_Lv2,
        Darkness_Sword_Lv3,
        Darkness_Sword_Lv4,
        Darkness_Sword_Lv5,
        Darkness_Sword_Lv6,
        Darkness_Sword_Lv7,
        Darkness_Sword_Lv8,
        Darkness_Sword_Lv9,
        Darkness_Sword_Lv10,

        Simple_Spear_Lv1 = 8700,
        Simple_Spear_Lv2,
        Simple_Spear_Lv3,
        Simple_Spear_Lv4,
        Simple_Spear_Lv5,
        Simple_Spear_Lv6,
        Simple_Spear_Lv7,
        Simple_Spear_Lv8,
        Simple_Spear_Lv9,
        Simple_Spear_Lv10,

        Gold_Spear_Lv1 = 8800,
        Gold_Spear_Lv2,
        Gold_Spear_Lv3,
        Gold_Spear_Lv4,
        Gold_Spear_Lv5,
        Gold_Spear_Lv6,
        Gold_Spear_Lv7,
        Gold_Spear_Lv8,
        Gold_Spear_Lv9,
        Gold_Spear_Lv10,
    }

    public AttackType attackType = AttackType.None;
    public WeaponType weaponType = WeaponType.None;

    public Weapon(WeaponID id, bool isEquip = false)
        : base(ItemType.Weapon, (int)id, isEquip)
    {
        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable;

        // Define AttackType
        attackType = table[id].property;
        weaponType = table[id].type;
    }
}
#endregion

#region Armor
public enum SetSkill
{
    None = -1,


}

public enum SkillID
{
    None = -1,

    a = 101,

}

public struct Skill
{
    public SkillID id;
    public int level;

    public Skill(SkillID id, int level)
    {
        this.id = id;
        this.level = level;
    }
}

public class Armor : Item
{
    public enum ArmorID
    {
        None = -1,

        HMD = 100001,

        Ballistic_Vest,

        Tactical_pants,

        Utility_belt,

        GM_Watch,
    }

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
    public SetSkill setSkill = SetSkill.None;
    public Skill[] skills = null;
    public int socket = 1;

    public Armor(ArmorID id, bool isEquip = false)
        : base(ItemType.Armor, (int)id, isEquip)
    {
        armorType = id switch
        {
            ArmorID.HMD => ArmorType.Head,

            ArmorID.Ballistic_Vest => ArmorType.Chest,

            ArmorID.Tactical_pants => ArmorType.Pants,

            ArmorID.Utility_belt => ArmorType.Belt,

            ArmorID.GM_Watch => ArmorType.Glove,

            _ => ArmorType.None,
        };


    }
}
#endregion