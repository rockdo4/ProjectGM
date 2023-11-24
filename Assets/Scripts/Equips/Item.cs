using System;


public class Item
{
    public enum ItemType
    {
        None = -1,

        Weapon,
        Armor,
    }

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

    public enum ArmorID
    {
        None = -1,

        test1,
        test2,
        test3,
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