using UnityEngine;

public class WeaponPrefab : MonoBehaviour, IEquip
{
    public Item item = null;

    public PlayerAnimationSO animationSO;

    [Header("무기 속성"), Tooltip("None: -, Hit: 타격, Slash: 참격, Pierce: 관통")]
    public AttackType type = AttackType.None;

    [Header("공격력")]
    public float attack;

    [Header("사거리")]
    public float attackRange = 2f;

    [Header("속성 배율")]
    public float weakDamage;

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
