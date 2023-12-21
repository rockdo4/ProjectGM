using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Animation SO")]
public class PlayerAnimationSO : ScriptableObject
{
    [Header("톤파 애니메이션")]
    public AnimatorController anim_Tonpa;

    [Header("대검 애니메이션")]
    public AnimatorController anim_Two_Hand_Sword;

    [Header("한손검 애니메이션")]
    public AnimatorController anim_One_Hand_Sword;

    [Header("창 애니메이션")]
    public AnimatorController anim_Spear;

    public AnimatorController GetAnimator(int id)
    {
        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable;
        switch (table[id].type)
        {
            case WeaponType.Tonpa: // 통파
                return anim_Tonpa;

            case WeaponType.Two_Hand_Sword: // 두손검
                return anim_Two_Hand_Sword;

            case WeaponType.One_Hand_Sword: // 한손검
                return anim_One_Hand_Sword;

            case WeaponType.Spear: // 창
                return anim_Spear;

            default:
                return null;
        }
    }
}
