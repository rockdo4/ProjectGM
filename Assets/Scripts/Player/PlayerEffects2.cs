using System.Collections.Generic;
using UnityEngine;
public enum PlayerEffectType
{
    Sprint,
    Evade,
    JustEvade,
    Hit,
    Attack,
    SuperAttack,
    Death
}

public class PlayerEffects2 : MonoBehaviour
{
    [System.Serializable]
    private class EffectInfo
    {
        public PlayerEffectType effectType;
        public Effect effect;
    }

    [Header("¿Ã∆Â∆Æ µÓ∑œ")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;

    private void Awake()
    {
        foreach(var info in effectInfos)
        {
            info.effect.Init(transform);
        }
    }

    public void PlayEffect(PlayerEffectType type, Vector3 direction = default)
    {
        var infos = effectInfos.FindAll((x) =>
        {
            return x.effectType == type;
        });
        foreach(var info in infos)
        {
            info.effect.PlayStart(direction);
        }
    }

    public void StopEffect(PlayerEffectType type)
    {
        var infos = effectInfos.FindAll((x) =>
        {
            return x.effectType == type;
        });
        foreach (var info in infos)
        {
            info.effect.PlayEnd();
        }
    }
}
