using System.Collections.Generic;

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
        data.Gold = Gold;

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
        var data = new SaveDataV6();
        data.Gold = Gold;
        data.WeaponInventory = WeaponInventory;
        data.ArmorInventory = ArmorInventory;
        data.MatInventory = MatInventory;

        return data;
    }
}

public class SaveDataV6 : SaveData
{
    public SaveDataV6() 
    {
        Version = 6;
    }

    public int Gold { get; set; } = 0;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public List<Materials> MatInventory = new List<Materials>();

    public List<SkillCode> CodeInventory = new List<SkillCode>();

    public List<int> SkillCodes = new List<int>();

    public override SaveData VersionUp()
    {
        var data = new SaveDataV7();
        data.Gold = Gold;
        data.WeaponInventory = WeaponInventory;
        data.ArmorInventory = ArmorInventory;
        data.MatInventory = MatInventory;
        data.CodeInventory = CodeInventory;
        data.SkillCodes = SkillCodes;

        return data;
    }
}

public class SaveDataV7 : SaveData
{
    public SaveDataV7() 
    {
        Version = 7;
    }

    public int Gold { get; set; } = 0;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public List<Materials> MatInventory = new List<Materials>();

    public List<SkillCode> CodeInventory = new List<SkillCode>();

    public List<int> SkillCodes = new List<int>();

    public bool Vibration { get; set; } = false;

    public override SaveData VersionUp()
    {
        var data = new SaveDataV8();
        data.Gold = Gold;
        data.WeaponInventory = WeaponInventory;
        data.ArmorInventory = ArmorInventory;
        data.MatInventory = MatInventory;
        data.CodeInventory = CodeInventory;
        data.SkillCodes = SkillCodes;
        data.Vibration = Vibration;

        return data;
    }
}

public class SaveDataV8 : SaveData
{
    public SaveDataV8() 
    {
        Version = 8;
    }

    public int Gold { get; set; } = 0;

    public List<Weapon> WeaponInventory = new List<Weapon>();

    public List<Armor> ArmorInventory = new List<Armor>();

    public List<Materials> MatInventory = new List<Materials>();

    public List<SkillCode> CodeInventory = new List<SkillCode>();

    public List<int> SkillCodes = new List<int>();

    public bool Vibration { get; set; } = false;

    public bool IsPlayed { get; set; } = false;

    public List<Unlock> UnlockInfo = new List<Unlock>();

    public float masterVol { get; set; } = 1.0f;

    public float musicVol { get; set; } = 1.0f;

    public float sfxVol { get; set; } = 1.0f;

    public float uiVol { get; set; } = 1.0f;
    public override SaveData VersionUp()
    {
        var data = new SaveDataV8();
        data.Gold = Gold;
        data.WeaponInventory = WeaponInventory;
        data.ArmorInventory = ArmorInventory;
        data.MatInventory = MatInventory;
        data.CodeInventory = CodeInventory;
        data.SkillCodes = SkillCodes;

        return data;
    }
}