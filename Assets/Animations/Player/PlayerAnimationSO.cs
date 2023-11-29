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
        switch (id)
        {
            case Weapon.WeaponID.Simple_Hammer:
            case Weapon.WeaponID.Gold_Hammer:
                return anim_Tonpa;

            case Weapon.WeaponID.Go_Work_Sword:
            case Weapon.WeaponID.Vigil_Sword:
                return anim_Two_Hand_Sword;

            case Weapon.WeaponID.Glory_Sword:
            case Weapon.WeaponID.Darkness_Sword:
                return anim_One_Hand_Sword;

            case Weapon.WeaponID.Simple_Spear:
            case Weapon.WeaponID.Gold_Spear:
                return anim_Spear;
 
            default:
                return null;
        }
    }
}
