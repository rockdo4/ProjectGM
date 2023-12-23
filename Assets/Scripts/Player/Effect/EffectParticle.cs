using UnityEngine;

public class EffectParticleSystem : EffectBase
{
    private ParticleSystem particle;
    [Header("각도 보정")]
    public Vector3 directionOffset;
    [Header("위치 보정")]
    public Vector3 positionOffset;
    private Transform targetTransform;

    protected override void Update()
    {
        
    }
    public override void Init(Transform playerTransform = null)
    {
        targetTransform = playerTransform;
        particle = GetComponent<ParticleSystem>();
        //particle.transform.SetParent(targetTransform);
        //particle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public override void PlayStart(Vector3 direction = default)
    {
        particle.transform.position = targetTransform.position;

        var rotation = targetTransform.rotation;
        var correctedRotation = Quaternion.Euler(rotation.eulerAngles + directionOffset);

        if (direction != default)
        {
            var normalizedDirection = direction.normalized;
            var finalRotation = Quaternion.LookRotation(normalizedDirection) * correctedRotation;
            particle.transform.rotation = finalRotation;
            particle.transform.localPosition += new Vector3(
                positionOffset.x * normalizedDirection.x,
                positionOffset.y,
                positionOffset.z * normalizedDirection.z
            );
        }
        else
        {
            particle.transform.localPosition += positionOffset;
            particle.transform.rotation = correctedRotation;
        }
        particle.gameObject.SetActive(true);
        particle.Play();
    }

    public override void PlayEnd()
    {
        particle.Stop();
        particle.gameObject.SetActive(false);
    }
}
