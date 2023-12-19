using System.Collections.Generic;
using UnityEngine;
using SaveDataVC = SaveDataV5; // Version Change?

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
                var weapon = new Weapon(101001);
                weapon.instanceID = weapon.instanceID.AddSeconds(1);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(102001);
                weapon.instanceID = weapon.instanceID.AddSeconds(2);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(103001, true);
                weapon.instanceID = weapon.instanceID.AddSeconds(3);
                data.WeaponInventory.Add(weapon);
            }
            {
                var weapon = new Weapon(104001);
                weapon.instanceID = weapon.instanceID.AddSeconds(4);
                data.WeaponInventory.Add(weapon);
            }

            SaveLoadSystem.Save(data, "savefile.json");
        }

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
        SaveLoadSystem.Remove("savefile.json");
        curWeapon = null;
        curArmor.Clear();
        Init();
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

    /*
    public static void UnlockQuest(int quest)
    {
        if (data.Quest != quest)
        {
            return;
        }

        data.Quest++;
        Save();
    }
    */

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
                if (curWeapon == null)
                {
                    return;
                }
                curWeapon.isEquip = false;
                curWeapon = null;
                break;

            case Equip.EquipType.Armor:
                if (curArmor[armorType] == null)
                {
                    return;
                }
                curArmor[armorType].isEquip = false;
                curArmor[armorType] = null;
                break;

            default:
                return;
        }
        Save();
    }

    public static void AddGold(int value)
    {
        if (data.Gold + value >= int.MaxValue)
        {
            data.Gold = int.MaxValue;
            return;
        }
        data.Gold += value;
        Save();
    }

    public static void IncreaseMat(int id, int count)
    {
        if (count <= 0)
        {
            return;
        }

        var mat = data.MatInventory.Find(x => x.id == id);

        if (mat == null)
        {
            data.MatInventory.Add(new Materials(id, count));
        }
        else 
        {
            mat.IncreaseCount(count);
        }

        Save();
    }

    public static void DecreaseMat(int id, int count)
    {
        var mat = data.MatInventory.Find(x => x.id == id);

        if (mat == null || mat.count < count)
        {
            return;
        }
        mat.count -= count;

        if (mat.count <= 0)
        {
            data.MatInventory.Remove(mat);
        }
        Save();
    }

    public static void SellItem(Weapon item)
    {
        if (item == null || !data.WeaponInventory.Contains(item))
        {
            return;
        }
        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable;

        if (item.isEquip)
        {
            //item.isEquip = false;
            //curWeapon = null;
            return;
        }
        data.WeaponInventory.Remove(item);
        AddGold(table[item.id].sellgold);
    }

    public static void SellItem(Armor item)
    {
        if (item == null || !data.ArmorInventory.Contains(item))
        {
            return;
        }
        var table = CsvTableMgr.GetTable<ArmorTable>().dataTable;

        if (item.isEquip)
        {
            //item.isEquip = false;
            //curArmor[item.armorType] = null;
            return;
        }
        data.ArmorInventory.Remove(item);
        AddGold(table[item.id].sellgold);
    }

    public static void SellItem(Materials item)
    {
        if (item == null || !data.MatInventory.Contains(item))
        {
            return;
        }
        var table = CsvTableMgr.GetTable<MatTable>().dataTable;

        data.MatInventory.Remove(item);
        AddGold(table[item.id].sellgold * item.count);
    }

    public static void SellItem(Materials item, int count)
    {
        if (item == null || !data.MatInventory.Contains(item) || count < 0 || item.count < count)
        {
            return;
        }
        if (item.count - count == 0)
        {
            SellItem(item);
            return;
        }
        var table = CsvTableMgr.GetTable<MatTable>().dataTable;

        data.MatInventory.Find(x => x == item).count -= count;
        AddGold(table[item.id].sellgold * count);
    }

    public static bool IsExistItem(Materials item)
    {
        return (item != null && data.MatInventory.Contains(item));
    }

    public static bool IsExistMat(int id)
    {
        return GetMaterials(id) != null;
    }

    public static Materials GetMaterials(int id)
    {
        return data.MatInventory.Find(x => x.id == id);
    }
}