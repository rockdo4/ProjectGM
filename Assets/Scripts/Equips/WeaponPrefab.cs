using UnityEngine;

public class WeaponPrefab : MonoBehaviour, IEquip
{
    public Item item = null;

    [Header("공격 속성")]
    public AttackType type = AttackType.None;

    [Header("애니메이션 SO")]
    public PlayerAnimationSO animationSO;

    public void OnEquip()
    {
        // Define AttackType
        type = item.id switch
        {
            // Hit
            (int)Weapon.WeaponID.Simple_Hammer => AttackType.Hit,
            (int)Weapon.WeaponID.Gold_Hammer => AttackType.Hit,

            // Slash
            (int)Weapon.WeaponID.Go_Work_Sword => AttackType.Slash,
            (int)Weapon.WeaponID.Vigil_Sword => AttackType.Slash,

            (int)Weapon.WeaponID.Glory_Sword => AttackType.Slash,
            (int)Weapon.WeaponID.Darkness_Sword => AttackType.Slash,

            // Pierce
            (int)Weapon.WeaponID.Simple_Spear => AttackType.Pierce,
            (int)Weapon.WeaponID.Gold_Spear => AttackType.Pierce,

            _ => AttackType.None,
        };
    }

    public void OnEquip(Item item)
    {
        this.item = item;
        OnEquip();
    }

    public void OnEquip(Item item, Animator anim)
    {
        OnEquip(item);

        anim.runtimeAnimatorController = animationSO.GetAnimator((Weapon.WeaponID)item.id);
    }
}
