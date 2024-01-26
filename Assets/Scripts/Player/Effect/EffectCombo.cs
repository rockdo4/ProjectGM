using UnityEngine;

public class EffectCombo : EffectBase
{
    [System.Serializable]
    private class AttackAudioByWeapon
    {
        public WeaponType weaponType;
        public AudioClip[] audioClips;
    }

    [Header("무기별 사운드")]
    [SerializeField]
    private AttackAudioByWeapon[] attackAudios;

    protected override void Update()
    {
        //Not Use baseUpdate
    }

    public override void Init(Transform targetTransform = null)
    {
        var weapon = targetTransform?.GetComponent<PlayerController>().EquipWeapon;
        foreach (var attackAudio in attackAudios)
        {
            if (attackAudio.weaponType != weapon.weaponType)
            {
                continue;
            }

            if (attackAudio != null)
            {
                audioClips = attackAudio.audioClips;
            }
        }
    }

    public override void PlayStart(Vector3 direction = default)
    {
        base.PlayStart(direction);
    }
}
