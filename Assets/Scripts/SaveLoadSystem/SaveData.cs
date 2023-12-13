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
    }

    public int Gold { get; set; } = 0;

    public int Quest { get; set; } = 1;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public override SaveData VersionUp()
    {
        var data = new SaveDataV4();
        data.Gold = Gold;
        data.Quest = Quest;
        data.WeaponInventory = WeaponInventory;
        data.ArmorInventory = ArmorInventory;

        return data;
    }
}

public class SaveDataV4 : SaveData
{
    public SaveDataV4() 
    {
        Version = 4;
    }

    public int Gold { get; set; } = 0;

    public int Quest { get; set; } = 1;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public List<Materials> MatInventory = new List<Materials>();

    public override SaveData VersionUp()
    {
        var data = new SaveDataV5();

        return data;
    }
}

public class SaveDataV5 : SaveData
{
    public SaveDataV5()
    {
        Version = 5;
    }

    public int Gold { get; set; } = 0;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public List<Materials> MatInventory = new List<Materials>();

    public override SaveData VersionUp()
    {
        return null;
    }
}