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

public enum PlayerEffectAudioMode
{
    Sequential, All, Random
}

public class PlayerEffects : MonoBehaviour
{
    [System.Serializable]
    [RequireComponent(typeof(EffectBase))]
    private class EffectInfo
    {
        public PlayerEffectType effectType;
        public GameObject[] newEffects;
        public List<EffectBase> copyEffects = new List<EffectBase>();
    }

    [Header("¿Ã∆Â∆Æ µÓ∑œ")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;
    [Header("Effect Transform")]
    [SerializeField]
    private Transform targetTransform;

    private void Start()
    {
        var audio = GetComponent<AudioSource>();
        foreach(var info in effectInfos)
        {
            foreach(var newEffect in info.newEffects)
            {
                var effect = Instantiate(newEffect, targetTransform).GetComponent<EffectBase>();
                effect.Init(transform);
                effect.SetAudio(audio);
                effect.gameObject.SetActive(false);
                info.copyEffects.Add(effect);
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
        foreach(var effect in effectInfo.copyEffects)
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
        foreach (var effect in effectInfo.copyEffects)
        {
            effect.PlayEnd();
        }
    }
}
