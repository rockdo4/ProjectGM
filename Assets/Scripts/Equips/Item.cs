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

public class Weapon : Item 
{
    public enum WeaponID
    {
        None = -1,

        Simple_Hammer = 8001,
        Gold_Hammer,

        Go_Work_Sword,
        Vigil_Sword,

        Glory_Sword,
        Darkness_Sword,

        Simple_Spear,
        Gold_Spear,
    }

    public AttackType attackType = AttackType.None;

    public Weapon(WeaponID id, bool isEquip = false)
        : base(ItemType.Weapon, (int)id, isEquip)
    {
        // Define AttackType
        attackType = id switch
        {
            // Hit
            WeaponID.Simple_Hammer => AttackType.Hit,
            WeaponID.Gold_Hammer => AttackType.Hit,

            // Slash
            WeaponID.Go_Work_Sword => AttackType.Slash,
            WeaponID.Vigil_Sword => AttackType.Slash,

            WeaponID.Glory_Sword => AttackType.Slash,
            WeaponID.Darkness_Sword => AttackType.Slash,

            // Pierce
            WeaponID.Simple_Spear => AttackType.Pierce,
            WeaponID.Gold_Spear => AttackType.Pierce,

            _ => AttackType.None,
        };
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