using UnityEngine;

[CreateAssetMenu(menuName = "Player Animation SO")]
public class PlayerAnimationSO : ScriptableObject
{
    [Header("톤파 애니메이션")]
    public AnimatorOverrideController anim_Tonpa;

    [Header("대검 애니메이션")]
    public AnimatorOverrideController anim_Two_Hand_Sword;

    [Header("한손검 애니메이션")]
    public AnimatorOverrideController anim_One_Hand_Sword;

    [Header("창 애니메이션")]
    public AnimatorOverrideController anim_Spear;

    public AnimatorOverrideController GetAnimator(Weapon.WeaponID id)
    {
        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable;
        switch (table[id].type)
        {
            case 1: // 통파
                return anim_Tonpa;

            case 2: // 두손검
                return anim_Two_Hand_Sword;

            case 3: // 한손검
                return anim_One_Hand_Sword;

            case 4: // 창
                return anim_Spear;

            default:
                return null;
        }
    }
}
