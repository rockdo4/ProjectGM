using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EffectType
{
    Evade, JustEvade
}
public class PlayerEffectPool : MonoBehaviour
{
    [System.Serializable]
    private class EffectInfo
    {
        public EffectType effectType;
        public ParticleSystem effectPrefab;
        public int defaultCount = 1;
    }

    [Header("이펙트 타입, 이펙트, 초기 생성 수")]
    [SerializeField]
    private EffectInfo[] effectInfos = null;
    private Dictionary<EffectType, ObjectPool<ParticleSystem>> poolDictionary = new Dictionary<EffectType, ObjectPool<ParticleSystem>>();

    private void Awake()
    {
        Debug.Log("Effect Pool Load......");
        for (int i = 0; i < effectInfos.Length; i++)
        {
            int loopScopedi = i;
            var effectPool = new ObjectPool<ParticleSystem>(
                () => CreateEffect(effectInfos[loopScopedi].effectType, effectInfos[loopScopedi].effectPrefab), 
                GetEffect, 
                ReleaseEffect,
                DestroyEffect, 
                true, 
                effectInfos[i].defaultCount
            );
            poolDictionary.Add(effectInfos[i].effectType, effectPool);
        }
        Debug.Log("......Complete");
    }

    private ParticleSystem CreateEffect(EffectType effectType, ParticleSystem prefab)
    {
        var effect = Instantiate(prefab);
        effect.GetComponent<EffectScript>().PlayEndListeners += () =>
        {
            poolDictionary[effectType].Release(effect);
        };
        return effect;
    }

    private void GetEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void ReleaseEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void DestroyEffect(ParticleSystem effect)
    {
        return;
    }

    public void PlayEffect(EffectType type, Transform tr = null)
    {
        var effect = poolDictionary[type].Get();
        if (tr != null)
        {
            effect.transform.position = tr.position;
        }
        else
        {
            effect.transform.position = transform.position;
        }
        effect.Play();
    }
}
