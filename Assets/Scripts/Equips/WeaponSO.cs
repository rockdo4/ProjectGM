using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "weaponSO")]
public class WeaponSO : ScriptableObject
{
    [Header("아이템 ID")]
    public Weapon.WeaponID[] ID;

    [Header("아이템 프리팹")]
    public GameObject[] Prefab;

    public GameObject MakeWeapon(Weapon weapon)
    {
        // Item Null Exception
        if (weapon == null)
        {
            Debug.LogWarning("Not Exist Item!");
            return null;
        }

        // Item.ItemType Exception
        if (weapon.type == Item.ItemType.None)
        {
            Debug.LogWarning("Wrong Item Type!");
            return null;
        }

        int index = -1;
        for (int i = 0; i < ID.Length; i++)
        {
            int original = weapon.id / 100 * 100; // 레벨 초기화
            if (ID[i] == (Weapon.WeaponID)original)
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

    public GameObject MakeWeapon(Weapon weapon, Transform tr)
    {
        var go = MakeWeapon(weapon);
        go.transform.SetParent(tr, false);
        return go;
    }

    public WeaponPrefab MakeWeapon(Weapon weapon, Transform tr, Animator anim)
    {
        var go = MakeWeapon(weapon, tr);
        var weaponPrefab = go.GetComponent<WeaponPrefab>();
        weaponPrefab.OnEquip(weapon, anim);

        return weaponPrefab;
    }
}
