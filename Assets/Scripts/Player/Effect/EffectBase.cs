using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public abstract class EffectBase : MonoBehaviour
{
    [Header("����� �ͼ�")]
    public AudioMixerGroup audioMixer;

    [Header("����Ʈ ���� �ð�"), Tooltip("��ƼŬ �ý����� ����")]
    [SerializeField]
    public float duration;

    public float Timer { get; private set; } = 0f;
    public bool IsPlay { get; protected set; } = false;

    [Header("ȿ����")]
    [SerializeField]
    protected AudioClip[] audioClips;
    protected AudioSource audioSource;
    protected Coroutine coEffectAudio;

    [Header("��� �ɼ�"), Tooltip("ȿ������ �������� ����� ó��, �̾ ���/��� ���ÿ� ���/�����ϰ� �ϳ� ���")]
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