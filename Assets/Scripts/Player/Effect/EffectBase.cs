using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public abstract class EffectBase : MonoBehaviour
{
    [Header("오디오 믹서")]
    public AudioMixerGroup audioMixer;

    [Header("이펙트 유지 시간"), Tooltip("파티클 시스템은 무시")]
    [SerializeField]
    public float duration;

    public float Timer { get; private set; }
    public bool IsPlay { get; protected set; } = false;

    [Header("효과음")]
    [SerializeField]
    protected AudioClip[] audioClips;
    protected AudioSource audioSource;
    protected Coroutine coEffectAudio;

    [Header("재생 옵션"), Tooltip("효과음이 여러개일 경우의 처리, 이어서 재생/모두 동시에 재생/랜덤하게 하나 재생")]
    public PlayerEffectAudioMode playMode = PlayerEffectAudioMode.Sequential;

    protected bool HasAudio
    {
        get
        {
            return audioClips.Length > 0;
        }
    }

    protected virtual void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioMixer;
    }

    protected virtual void Update()
    {
        if (!IsPlay)
        {
            return;
        }
        Timer += Time.deltaTime;
        if (Timer > duration)
        {
            PlayEnd();
        }
    }

    public virtual void PlayStart(Vector3 direction = default)
    {
        Timer = 0f;
        gameObject.SetActive(IsPlay = true);

        if (HasAudio)
        {
            switch(playMode)
            {
                case PlayerEffectAudioMode.Sequential:
                    coEffectAudio = StartCoroutine(CoEffectAudio());
                    break;
                case PlayerEffectAudioMode.All:
                    AllEffectAudio();
                    break;
                case PlayerEffectAudioMode.Random:
                    RandomEffectAudio();
                    break;
            }
        }
    }
    public virtual void PlayEnd()
    {
        if (HasAudio)
        {
            switch (playMode)
            {
                case PlayerEffectAudioMode.Sequential:
                    if (coEffectAudio != null)
                    {
                        StopCoroutine(coEffectAudio);
                        coEffectAudio = null;
                    }
                    break;
                default:
                    audioSource.Stop();
                    break;
            }
        }

        gameObject.SetActive(IsPlay = false);
    }

    protected virtual IEnumerator CoEffectAudio()
    {
        int index = 0;

        while (index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);

            index++;
        }
    }

    protected virtual void RandomEffectAudio()
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    protected virtual void AllEffectAudio()
    {
        foreach(var clip in audioClips)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public abstract void Init(Transform targetTransform);
}