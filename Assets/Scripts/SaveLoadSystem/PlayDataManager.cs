using System.Collections.Generic;
using SaveDataVC = SaveDataV4; // Version Change?

public static class PlayDataManager
{
    public static SaveDataVC data;

    public static Weapon curWeapon = null;

    public static readonly Dictionary<Armor.ArmorType, Armor> curArmor
        = new Dictionary<Armor.ArmorType, Armor>();

    public static void Init()
    {
        data = SaveLoadSystem.Load("savefile.json") as SaveDataVC;
        if (data == null)
        {
            data = new SaveDataVC();

            // 기본 무기 4종 지급
            {
                var weapon = new Weapon(8100);
                weapon.instanceID.AddSeconds(1);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(8300);
                weapon.instanceID.AddSeconds(2);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(8500);
                weapon.instanceID.AddSeconds(3);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(8700);
                weapon.instanceID.AddSeconds(4);
                data.WeaponInventory.Add(weapon);
            }
        }
        SaveLoadSystem.Save(data, "savefile.json");

        // 무기 인벤토리를 순회해서 curWeapon에 할당
        foreach (var weapon in data.WeaponInventory)
        {
            if (weapon.isEquip)
            {
                curWeapon = weapon;
                break;
            }
        }

        // 방어구 인벤토리를 순회해서 curArmor 5종에 할당
        for (Armor.ArmorType i = Armor.ArmorType.Head; i <= Armor.ArmorType.Glove; i++)
        {
            curArmor.Add(i, null);
        }

        foreach (var armor in data.ArmorInventory)
        {
            if (armor.isEquip)
            {
                curArmor[armor.armorType] = armor;
            }
        }
    }

    public static void Save()
    {
        SaveLoadSystem.Save(data, "savefile.json");
    }

    public static void Reset()
    {
        data = new SaveDataVC();
        curWeapon = null;
        curArmor.Clear();
        Save();
    }

    public static bool Purchase(int pay)
    {
        if (pay > data.Gold)
        {
            return false;
        }

        data.Gold -= pay;
        Save();
        return true;
    }

    public static void UnlockQuest(int quest)
    {
        if (data.Quest != quest)
        {
            return;
        }

        data.Quest++;
        Save();
    }

    public static void WearItem(Equip item)
    {
        if (item == null)
        {
            // Null Exception
            return;
        }

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                var weapon = item as Weapon;

                if (curWeapon == null)
                {
                    curWeapon = weapon;
                    curWeapon.isEquip = true;

                    break;
                }

                if (curWeapon.instanceID == weapon.instanceID)
                {
                    return;
                }

                curWeapon.isEquip = false;
                curWeapon = weapon;
                curWeapon.isEquip = true;

                break;

            case Equip.EquipType.Armor:
                var armor = item as Armor;

                if (curArmor[armor.armorType] == null)
                {
                    curArmor[armor.armorType] = armor;
                    curArmor[armor.armorType].isEquip = true;

                    break;
                }

                if (curArmor[armor.armorType].instanceID == armor.instanceID)
                {
                    return;
                }

                curArmor[armor.armorType].isEquip = false;
                curArmor[armor.armorType] = armor;
                curArmor[armor.armorType].isEquip = true;

                break;

            default:
                return;
        }
        Save();
    }

    public static void UnWearItem(Equip.EquipType type, Armor.ArmorType armorType = Armor.ArmorType.None)
    {
        switch (type)
        {
            case Equip.EquipType.Weapon:
                curWeapon.isEquip = false;
                curWeapon = null;
                break;

            case Equip.EquipType.Armor:
                curArmor[armorType].isEquip = false;
                curArmor[armorType] = null;
                break;

            default:
                return;
        }
        Save();
    }
}