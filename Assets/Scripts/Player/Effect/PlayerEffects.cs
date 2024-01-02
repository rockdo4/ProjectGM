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
    Death
}

public class PlayerEffects : MonoBehaviour
{
    [System.Serializable]
    [RequireComponent(typeof(EffectBase))]
    private class EffectInfo
    {
        public PlayerEffectType effectType;
        public EffectBase effect;
    }

    [Header("¿Ã∆Â∆Æ µÓ∑œ")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;

    private void Start()
    {
        foreach(var info in effectInfos)
        {
            info.effect.Init(transform);
            info.effect.gameObject.SetActive(false);
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
