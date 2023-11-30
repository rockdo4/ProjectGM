using Unity.VisualScripting;
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
        // Item Null Exception
        if (item == null)
        {
            Debug.LogWarning("Not Exist Item!");
            return null;
        }

        // Item.ItemType Exception
        if (item.type == Item.ItemType.None)
        {
            Debug.LogWarning("Wrong Item Type!");
            return null;
        }

        int index = -1;
        for (int i = 0; i < ID.Length; i++)
        {
            int original = item.id / 100 * 100; // 레벨 초기화
            if (ID[i] == original)
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
        var go = MakeItem(item);
        go.transform.SetParent(tr, false);
        return go;
    }

    public WeaponPrefab MakeItem(Item item, Transform tr, Animator anim)
    {
        var go = MakeItem(item, tr);
        var weapon = go.GetComponent<WeaponPrefab>();
        weapon.OnEquip(item, anim);
        return weapon;
    }
}
