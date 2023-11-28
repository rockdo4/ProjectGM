using Unity.VisualScripting;
using static UnityEditor.Progress;
using SaveDataVC = SaveDataV3; // Version Change?

public static class PlayDataManager
{
    public static SaveDataVC data;

    public static void Init()
    {
        data = SaveLoadSystem.Load("savefile.json") as SaveDataVC;
        if (data == null)
        {
            data = new SaveDataVC();
        }
        SaveLoadSystem.Save(data, "savefile.json");
    }

    public static void Save()
    {
        SaveLoadSystem.Save(data, "savefile.json");
    }

    public static void Reset()
    {
        data = new SaveDataVC();
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

    public static void WearItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (data.Equipment[item.type] == null)
        {
            item.isEquip = true;
            data.Equipment.Add(item.type, item.instanceID);
            Save();

            return;
        }

        var eqiup = GetCurrentItem(item.type);
        if (eqiup != null)
        {
            eqiup.isEquip = false;
        }

        data.Equipment[item.type] = item.instanceID;
        item.isEquip = true;

        Save();
    }

    public static Item GetCurrentItem(Item.ItemType type)
    {
        return data.Inventory[type].Find(x => x.instanceID == data.Equipment[type]);
    }
}