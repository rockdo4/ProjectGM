using UnityEngine;

public class TakeAttackAndEvade : MonoBehaviour, IAttackable
{
    private enum EvadeSuccess
    {
        None, Normal, Just
    }
    private EvadeSuccess evade;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        player.GetComponent<PlayerController>().LastEvadeState = PlayerController.EvadeState.None;
        if (player == null)
        {
            return;
        }

        var damage = attack.Damage;
        damage -= player.Stat.Defence;

        EvadeCheck();
        switch (evade)
        {
            case EvadeSuccess.None:
                player.Effects.PlayEffect(PlayerEffectType.Hit);
                player.IsGroggy = attack.IsGroggy;
                break;
            case EvadeSuccess.Normal:
                player.GetComponent<PlayerController>().LastEvadeState = PlayerController.EvadeState.Normal;
                player.Effects.PlayEffect(PlayerEffectType.Hit);
                damage = (int)(damage * player.Stat.evadeDamageRate);
                break;
            case EvadeSuccess.Just:
                player.GetComponent<PlayerController>().LastEvadeState = PlayerController.EvadeState.Just;
                player.Effects.PlayEffect(PlayerEffectType.JustEvade);
                damage = 0;
                break;
        }

        var blocked = Random.value < player.Stat.block;

        if (blocked || damage <= 0)
        {
            damage = 0;
        }
        player.HP -= damage;

        if (player.HP <= 0)
        {
            player.HP = 0;
            player.GetComponent<PlayerController>().SetState(PlayerController.State.Death);
            var destructables = player.GetComponents<IDestructable>();
            foreach (var destructable in destructables)
            {
                destructable.OnDestruction(attacker);
            }
        }
    }

    private void EvadeCheck()
    {
        if (player.GetComponent<PlayerController>().CurrentState != PlayerController.State.Evade)
        {
            evade = EvadeSuccess.None;
            return;
        }

        evade = player.evadeTimer switch
        {
            float x when (x < player.Stat.justEvadeTime) => EvadeSuccess.Just,
            float x when (x >= player.Stat.justEvadeTime && x < player.Stat.evadeTime) => EvadeSuccess.Normal,
            _ => EvadeSuccess.None
        };

        player.evadePoint += evade switch
        {
            EvadeSuccess.Just => player.Stat.justEvadePoint,
            EvadeSuccess.Normal => player.Stat.evadePoint,
            _ => player.Stat.hitEvadePoint
        };

        player.evadePoint = Mathf.Clamp(player.evadePoint, 0, player.Stat.maxEvadePoint);
    }
}
