using UnityEngine;

public class AttackDebug : MonoBehaviour, IAttackable
{
    public void OnAttack(GameObject attacker, Attack attack)
    {
        Debug.Log($"{attacker.name} attacked {gameObject.name} for {attack.Damage} damage.\n{gameObject.name}_HP: {GetComponent<LivingObject>().HP}/{GetComponent<LivingObject>().stat.HP}\tIsCritical: {attack.IsCritical}");
    }
}
