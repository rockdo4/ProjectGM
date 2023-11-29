using UnityEngine;

public class AttackDebug : MonoBehaviour, IAttackable
{
    private enum EvadeSuccesss
    {
        None, Normal, Just
    }
    private EvadeSuccesss evade;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        Debug.Log($"{attacker.name} attacked {gameObject.name} for {attack.Damage} damage.\n{gameObject.name}_HP: {GetComponent<LivingObject>().HP}/{GetComponent<LivingObject>().stat.HP}\tIsCritical: {attack.IsCritical}");

        EvadeLog();
    }

    private void EvadeLog()
    {
        if (player == null)
        {
            return;
        }

        evade = player.evadeTimer switch
        {
            float x when (x < player.Stat.justEvadeTime) => EvadeSuccesss.Just,
            float x when (x >= player.Stat.justEvadeTime && x < player.Stat.evadeTime) => EvadeSuccesss.Normal,
            _ => EvadeSuccesss.None
        };

        switch (evade)
        {
            case EvadeSuccesss.None:
                Debug.Log("피격");
                break;
            case EvadeSuccesss.Normal:
                Debug.Log("일반 회피");
                break;
            case EvadeSuccesss.Just:
                Debug.Log("저스트 회피");
                break;
        }
    }
}
