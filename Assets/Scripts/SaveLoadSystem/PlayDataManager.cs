using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SaveDataVC = SaveDataV8; // Version Change?

public static class PlayDataManager
{
    public static SaveDataVC data;

    public static Weapon curWeapon = null;

    public static readonly Dictionary<Armor.ArmorType, Armor> curArmor
        = new Dictionary<Armor.ArmorType, Armor>();

    public static Dictionary<int, int> curSkill 
        = new Dictionary<int, int>();

    public struct SetSkillInfo
    {
        public int id;
        public int level;

        public SetSkillInfo(int id = -1, int level = 0)
        {
            this.id = id;
            this.level = level;
        }
    }

    public static SetSkillInfo curSetSkill = new SetSkillInfo();

    #region Inventory Capacity
    // 무기 최대 소지 개수
    public static readonly int weaponsCapacity = 32;

    // 방어구 최대 소지 개수
    public static readonly int armorsCapacity = 140;

    // 스킬 코드 최대 소지 개수
    public static readonly int skillcodesCapacity = 260;

    // 재료 최대 소지 개수
    public static readonly int materialsCapacity = 40;
    #endregion

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

            // 재료 아이템 임시 지급 test code
            if (data.MatInventory.Count == 0)
            {
                IncreaseMat(610001, 50);
                IncreaseMat(611001, 50);
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

        OrganizeSkill();

        OrganizeSetSkill();
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

        OrganizeSkill();
        OrganizeSetSkill();
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

        OrganizeSkill();
        OrganizeSetSkill();

        // 스킬코드 소켓에 대한 예외처리 추가 필요
    }

    public static void AddGold(int value)
    {
        if ((uint)data.Gold + (uint)value >= int.MaxValue)
        {
            data.Gold = int.MaxValue;
            return;
        }
        data.Gold += value;
        Save();
    }

    public static void IncreaseMat(int id, int count)
    {
        if (id <= 0 || count <= 0)
        {
            return;
        }

        var mat = data.MatInventory.Find(x => x.id == id);

        if (mat == null)
        {
            if (data.MatInventory.Count >= materialsCapacity)
            {
                return;
            }
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

    public static void IncreaseCode(int id, int count)
    {
        if (count <= 0)
        {
            return;
        }

        var code = data.CodeInventory.Find(x => x.id == id);

        if (code == null)
        {
            if (data.CodeInventory.Count >= skillcodesCapacity)
            {
                return;
            }
            data.CodeInventory.Add(new SkillCode(id, count));
        }
        else
        {
            code.IncreaseCount(count);
        }

        Save();
    }

    public static void DecreaseCode(SkillCode code, int count)
    {
        if (code == null || code.count < count)
        {
            return;
        }
        code.count -= count;

        if (code.count <= 0)
        {
            data.CodeInventory.Remove(code);
        }
        Save();
    }

    public static void DecreaseCode(int id, int count)
    {
        var code = data.CodeInventory.Find(x => x.id == id);

        if (code == null || code.count < count)
        {
            return;
        }
        code.count -= count;

        if (code.count <= 0)
        {
            data.CodeInventory.Remove(code);
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

    public static void SellItem(SkillCode item)
    {
        if (item == null || !data.CodeInventory.Contains(item))
        {
            return;
        }
        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;

        data.CodeInventory.Remove(item);
        AddGold(table[item.id].sellgold * item.count);
    }

    public static void SellItem(SkillCode item, int count)
    {
        if (item == null || !data.CodeInventory.Contains(item) || count < 0 || item.count < count)
        {
            return;
        }
        if (item.count - count == 0)
        {
            SellItem(item);
            return;
        }
        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;

        data.CodeInventory.Find(x => x == item).count -= count;
        AddGold(table[item.id].sellgold * count);
    }

    public static bool IsExistItem(SkillCode item)
    {
        return (item != null && data.CodeInventory.Contains(item));
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

    public static void AddItem(Weapon weapon)
    {
        if (weapon == null || data.WeaponInventory.Count >= weaponsCapacity)
        {
            return;
        }

        data.WeaponInventory.Add(weapon);
    }

    public static void AddItem(Armor armor)
    {
        if (armor == null || data.ArmorInventory.Count >= armorsCapacity)
        {
            return;
        }

        data.ArmorInventory.Add(armor);
    }

    public static int GetSocket()
    {
        var table = CsvTableMgr.GetTable<ArmorTable>().dataTable;
        var count = 0;
        foreach (var armor in curArmor)
        {
            if (armor.Value != null)
            {
                count += table[armor.Value.id].socket;
            }
        }
        return count;
    }

    public static bool EquipSkillCode(SkillCode code)
    {
        if (code == null || 
            data.SkillCodes.Count + 1 > GetSocket() || 
            !IsExistItem(code) || 
            code.count - 1 <= 0)
        {
            return false;
        }

        data.SkillCodes.Add(code.id);
        DecreaseCode(code, 1);

        data.SkillCodes.Sort();

        OrganizeSkill();
        Save();

        return true;
    }

    public static void UnEquipSkillCode(int id)
    {
        if (!data.SkillCodes.Contains(id))
        {
            return;
        }

        data.SkillCodes.Remove(id);
        IncreaseCode(id, 1);

        data.SkillCodes.Sort();

        OrganizeSkill();
        Save();

    }

    public static void OrganizeSkill()
    {
        curSkill.Clear();

        var ct = CsvTableMgr.GetTable<CodeTable>().dataTable;
        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;

        foreach (var id in data.SkillCodes) 
        {
            var id1 = ct[id].skill1_id;
            var lv1 = ct[id].skill1_lv;
            if (!curSkill.ContainsKey(id1))
            {
                curSkill.Add(id1, lv1);
            }
            else 
            {
                curSkill[id1] = Mathf.Clamp(curSkill[id1] + lv1, 1, skt[id1].max_lv);
            }
            
            var id2 = ct[id].skill2_id;
            var lv2 = ct[id].skill2_lv;
            if (id2 != -1)
            {
                if (!curSkill.ContainsKey(id2))
                {
                    curSkill.Add(id2, lv2);
                }
                else
                {
                    curSkill[id2] = Mathf.Clamp(curSkill[id2] + lv2, 1, skt[id2].max_lv);
                }
            }

        }

        curSkill = curSkill.OrderByDescending((x) => x.Value).ThenBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
    }

    public static void OrganizeSetSkill()
    {
        int[] arr = new int[curArmor.Count];

        int index = 0;
        foreach (var armor in curArmor.Values)
        {
            arr[index] = (armor == null) ? -1 : armor.setSkill;
            index++;
        }

        curSetSkill.id = -1;
        curSetSkill.level = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == -1)
            {
                continue;
            }

            int count = 1;

            for (int j = i + 1; j < arr.Length; j++)
            {
                if (arr[i] == arr[j])
                {
                    count++;
                    arr[j] = -1;
                }
            }

            // 현재까지 최다 중복 횟수를 가진 값보다 많이 중복되었다면 갱신
            if (count > curSetSkill.level)
            {
                curSetSkill.level = count;
                curSetSkill.id = arr[i];
            }
        }

    }

        public static bool StageUnlockCheck(int id)
    {
        var unlockList = data.UnlockInfo;
        foreach (var unlock in unlockList)
        {
            if (unlock.id == id && unlock.unlocked)
            {
                return true;
            }
        }

        return false;
    }

    public static void StageUnlock(int id)
    {
        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        var unlockList = data.UnlockInfo;

        //이미 클리어 했으면 스킵
        var unlock = unlockList.Find(x => x.id == id);
        if (unlock.cleared)
        {
            return;
        }

        foreach (var stage in stageTable)
        {
            if (stage.Value.unlock != id)
            {
                continue;
            }

            unlockList.Find(x => x.id == stage.Key).unlocked = true;
        }
        unlock.cleared = true;

        Save();
    }

    public static void StageInfoRefresh()
    {
        //스테이지가 추가/제거 됐는지 확인
        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        var unlockList = data.UnlockInfo;

        foreach (var stage in stageTable)
        {
            //데이터가 있는지 체크
            var unlock = unlockList.Find((x) => x.id == stage.Key);
            if (unlock != null)
            {
                continue;
            }

            //0보다 작으면 자동 해금
            var newUnlock = new Unlock(stage.Key, stage.Value.unlock < 0);
            if (!newUnlock.unlocked)
            {
                //해금 조건이 이미 클리어가 되어있는가
                var cleared = unlockList.Find(x => x.id == stage.Value.unlock && x.cleared);
                newUnlock.unlocked = cleared != null;
            }
            unlockList.Add(newUnlock);
        }

        Save();
    }
}