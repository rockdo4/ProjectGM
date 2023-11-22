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

        return null;
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
    public readonly List<Item> Inventory = new List<Item>();
    public readonly Dictionary<Item.ItemType, DateTime> Equipment = new Dictionary<Item.ItemType, DateTime>();

    public override SaveData VersionUp()
    {
        return null;
    }
}
/*
public class SaveDataV4 : SaveData
{
    public SaveDataV4()
    {
        Version = 4;
    }

    public int HighScore { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Upgrade_HealthUP { get; set; } = 0;
    public int Upgrade_GoldUP { get; set; } = 0;
    public int Upgrade_SpeedDown { get; set; } = 0;
    public int Stage { get; set; } = 1;

    public override SaveData VersionUp()
    {
        var data = new SaveDataV5();
        data.HighScore = HighScore;
        data.Gold = Gold;
        data.Upgrade_HealthUP = Upgrade_HealthUP;
        data.Upgrade_GoldUP = Upgrade_GoldUP;
        data.Upgrade_SpeedDown = Upgrade_SpeedDown;
        data.Stage = Stage;

        return data;
    }
}

public class SaveDataV5 : SaveData
{
    public SaveDataV5()
    {
        Version = 5;
    }

    public int HighScore { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Upgrade_HealthUP { get; set; } = 0;
    public int Upgrade_GoldUP { get; set; } = 0;
    public int Upgrade_SpeedDown { get; set; } = 0;
    public int Stage { get; set; } = 1;
    public WeaponID EquipWeapon { get; set; } = WeaponID.Starter;
    public List<WeaponID> UnlockList { get; set; } = new List<WeaponID>();

    public override SaveData VersionUp()
    {
        var data = new SaveDataV6();
        data.HighScore = HighScore;
        data.Gold = Gold;
        data.Upgrade_HealthUP = Upgrade_HealthUP;
        data.Upgrade_GoldUP = Upgrade_GoldUP;
        data.Upgrade_SpeedDown = Upgrade_SpeedDown;
        data.Stage = Stage;
        data.EquipWeapon = EquipWeapon;
        data.UnlockList = UnlockList;

        return data;
    }
}

public class SaveDataV6 : SaveData // Current
{
    public SaveDataV6()
    {
        Version = 6;
    }

    public int HighScore { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Upgrade_HealthUP { get; set; } = 0;
    public int Upgrade_GoldUP { get; set; } = 0;
    public int Upgrade_SpeedDown { get; set; } = 0;
    public int Stage { get; set; } = 1;
    public WeaponID EquipWeapon { get; set; } = WeaponID.Starter;
    public List<WeaponID> UnlockList { get; set; } = new List<WeaponID>();
    public float masterVol { get; set; } = 1.0f;
    public float musicVol { get; set; } = 1.0f;
    public float sfxVol { get; set; } = 1.0f;

    public override SaveData VersionUp()
    {
        return null;
    }
}
*/