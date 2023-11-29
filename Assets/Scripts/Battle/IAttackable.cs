using UnityEngine;

public interface IAttackable
{
    void OnAttack(GameObject attacker, Attack attack);
}
