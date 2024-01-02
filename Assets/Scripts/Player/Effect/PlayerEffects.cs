using System.Collections.Generic;
using UnityEngine;
public enum PlayerEffectType
{
    Sprint,
    Evade,
    JustEvade,
    Hit,
    Attack,
    Super_Tonpa,
    Super_TwoHandSword_Charge,
    Super_TwoHandSword,
    Super_OneHandSword,
    Super_Spear,
    SlowMotion,
    Death
}

public class PlayerEffects : MonoBehaviour
{
    [System.Serializable]
    [RequireComponent(typeof(EffectBase))]
    private class EffectInfo
    {
        public PlayerEffectType effectType;
        public EffectBase[] effects;
    }

    [Header("¿Ã∆Â∆Æ µÓ∑œ")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;

    private void Start()
    {
        foreach(var info in effectInfos)
        {
            foreach(var effect in info.effects)
            {
                effect.Init(transform);
                effect.gameObject.SetActive(false);
            }
        }
    }

    public void PlayEffect(PlayerEffectType type, Vector3 direction = default)
    {
        var effectInfo = effectInfos.Find((x) =>
        {
            return x.effectType == type;
        });

        if (effectInfo == null)
        {
            return;
        }
        foreach(var effect in effectInfo.effects)
        {
            effect.PlayStart(direction);
        }
    }

    public void StopEffect(PlayerEffectType type)
    {
        var effectInfo = effectInfos.Find((x) =>
        {
            return x.effectType == type;
        });

        if (effectInfo == null)
        {
            return;
        }
        foreach (var effect in effectInfo.effects)
        {
            effect.PlayEnd();
        }
    }
}
