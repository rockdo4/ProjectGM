using UnityEngine;

[CreateAssetMenu(menuName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("아이템 종류")]
    public Item.ItemType Type;

    [Header("아이템 ID")]
    public int[] ID;

    [Header("아이템 프리팹")]
    public GameObject[] Prefab;

    public GameObject MakeItem(Item item)
    {
        // Item.ItemType Exception
        if (item.type == Item.ItemType.None)
        {
            Debug.LogWarning("Wrong Item Type!");
            return null;
        }

        int index = -1;
        for (int i = 0; i < ID.Length; i++)
        {
            if (ID[i] == item.id)
            {
                index = i;
                break;
            }
        }

        // Item index Exception
        if (index < 0)
        {
            Debug.LogWarning("Not Exist Item!");
            return null;
        }

        var go = Instantiate(Prefab[index]);
        return go;
    }

    public GameObject MakeItem(Item item, Transform tr)
    {
        // Item.ItemType Exception
        if (item.type == Item.ItemType.None)
        {
            Debug.LogWarning("Wrong Item Type!");
            return null;
        }

        int index = -1;
        for (int i = 0; i < ID.Length; i++)
        {
            if (ID[i] == item.id)
            {
                index = i;
                break;
            }
        }

        // Item index Exception
        if (index < 0)
        {
            Debug.LogWarning("Not Exist Item!");
            return null;
        }

        var go = Instantiate(Prefab[index]);
        go.transform.SetParent(tr);
        return go;
    }
}
