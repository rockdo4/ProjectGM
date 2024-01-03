using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

//Not Use
public class PlayerEffectPool : MonoBehaviour
{
    [System.Serializable]
    private class EffectInfo
    {
        public PlayerEffectType effectType;
        public ParticleSystem effectPrefab;
        public int defaultCount = 1;
    }

    [Header("이펙트 타입, 이펙트, 초기 생성 수")]
    [SerializeField]
    private EffectInfo[] effectInfos = null;
    private Dictionary<PlayerEffectType, ObjectPool<ParticleSystem>> poolDictionary = new Dictionary<PlayerEffectType, ObjectPool<ParticleSystem>>();
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
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

    private ParticleSystem CreateEffect(PlayerEffectType effectType, ParticleSystem prefab)
    {
        var effect = Instantiate(prefab);        
        effect.transform.SetParent(transform);
        effect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //effect.transform.localPosition = Vector3.zero;
        //effect.transform.localRotation = Quaternion.identity;
        //effect.GetComponent<EffectScript>().PlayEndListeners += () =>
        //{
        //    poolDictionary[effectType].Release(effect);
        //};
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

    public void PlayEffect(PlayerEffectType type, Vector3 direction = default)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.Log($"등록되지 않은 이펙트: {type}");
            return;
        }
        var effect = poolDictionary[type].Get();

        if (direction != default)
        {
            Vector3 normalizedDirection = direction.normalized;
            Vector3 flattenedDirection = new Vector3(normalizedDirection.x, 0f, normalizedDirection.z);
            effect.transform.localRotation = Quaternion.LookRotation(flattenedDirection);
        }

        effect.Play();
    }
}
