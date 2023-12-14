using CsvHelper;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [System.Serializable]
    private class EffectInfo
    {
        [Header("이펙트 종류")]
        public EffectType effectType;
        [Header("이펙트 프리펩")]
        public ParticleSystem effectPrefab;
        [HideInInspector]
        public ParticleSystem effect = null;
        [Header("각도 보정")]
        public Vector3 directionOffset;
        [Header("위치 보정")]
        public Vector3 positionOffset;

    }

    [Header("이펙트 등록")]
    [SerializeField]
    private List<EffectInfo> effectInfos = null;

    private void Awake()
    {
        foreach(var info in effectInfos)
        {
            info.effect = Instantiate(info.effectPrefab);
            info.effect.transform.SetParent(transform);
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
            effect.transform.localPosition += new Vector3(
                effectInfo.positionOffset.x * normalizedDirection.x,
                effectInfo.positionOffset.y,
                effectInfo.positionOffset.z * normalizedDirection.z
            );
        }
        else
        {
            effect.transform.localPosition += effectInfo.positionOffset;
            effect.transform.rotation = correctedRotation;
        }


        effect.Play();
    }
}
