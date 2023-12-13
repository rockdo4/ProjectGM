using CsvHelper;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [System.Serializable]
    private class EffectInfo
    {
        public EffectType effectType;
        public ParticleSystem effectPrefab;
        [HideInInspector]
        public ParticleSystem effect = null;
        [Header("플레이어 기준 각도 보정")]
        public Vector3 directionOffset;

    }

    [Header("이펙트 종류, 이펙트")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;

    private void Awake()
    {
        foreach(var info in effectInfos)
        {
            info.effect = Instantiate(info.effectPrefab);
            info.effect.transform.SetParent(null);
            info.effect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void PlayEffect(EffectType type, Vector3 direction = default)
    {
        var effectInfo = effectInfos.Find((x) =>
        {
            return x.effectType == type;
        });

        if (effectInfo == null)
        {
            Debug.Log($"등록되지 않은 이펙트: {type}");
            return;
        }

        var effect = effectInfo.effect;
        effect.transform.position = transform.position;
        var rotation = transform.rotation;

        var correctedRotation = Quaternion.Euler(rotation.eulerAngles + effectInfo.directionOffset);
        if (direction != default)
        {
            var normalizedDirection = direction.normalized;
            var finalRotation = Quaternion.LookRotation(normalizedDirection) * correctedRotation;
            effect.transform.rotation = finalRotation;
        }
        else
        {
            effect.transform.rotation = correctedRotation;
        }


        effect.Play();
    }
}
