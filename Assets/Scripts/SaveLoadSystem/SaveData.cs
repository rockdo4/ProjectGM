using System;
using System.Collections.Generic;
using System.Data;

public abstract class SaveData
{
    public int Version { get; set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public SaveDataV1() 
    {
        Version = 1;
    }

    public int Gold { get; set; } = 0;

    public override SaveData VersionUp()
    {
        var data = new SaveDataV2();
        data.Gold = Gold;
        
        return data;
    }
}

public class SaveDataV2 : SaveData 
{
    public SaveDataV2()
    {
        Version = 2;
    }

    public int Gold { get; set; } = 0;

    public int Quest { get; set; } = 1;

    public override SaveData VersionUp()
    {
        var data = new SaveDataV3();
        data.Gold = Gold;
        data.Quest = Quest;

        return data;
    }
}

public class SaveDataV3 : SaveData
{
    public SaveDataV3()
    {
        Version = 3;

        Inventory.Add(Item.ItemType.Weapon, new List<Item>());
        Inventory.Add(Item.ItemType.Armor, new List<Item>());

        Equipment.Add(Item.ItemType.Weapon, new DateTime());
        Equipment.Add(Item.ItemType.Armor, new DateTime());
    }

    public int Gold { get; set; } = 0;

    public int Quest { get; set; } = 1;

    public readonly Dictionary<Item.ItemType, List<Item>> Inventory
        = new Dictionary<Item.ItemType, List<Item>>();

    public readonly Dictionary<Item.ItemType, DateTime> Equipment
        = new Dictionary<Item.ItemType, DateTime>();

    public override SaveData VersionUp()
    {
        return null;
    }
}