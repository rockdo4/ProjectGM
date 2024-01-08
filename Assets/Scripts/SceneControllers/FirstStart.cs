using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FirstStart : MonoBehaviour
{
    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    [Header("∏∂Ω∫≈Õ πÕº≠")]
    [SerializeField]
    private AudioMixer mixer;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        mixer.SetFloat("masterVol", Mathf.Log10(PlayDataManager.data.masterVol) * 20);
        mixer.SetFloat("musicVol", Mathf.Log10(PlayDataManager.data.musicVol) * 20);
        mixer.SetFloat("sfxVol", Mathf.Log10(PlayDataManager.data.sfxVol) * 20);
        mixer.SetFloat("uiVol", Mathf.Log10(PlayDataManager.data.uiVol) * 20);
    }

    public void StartGame()
    {
        BLACK.FadeOut((PlayDataManager.data.IsPlayed) ? "Title" : "Tutorial Scene");
    }
}
