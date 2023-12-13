using System;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    private ParticleSystem effect;

    public event Action PlayEndListeners;

    private void Awake()
    {
        effect = GetComponent<ParticleSystem>();
        var main = effect.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        if (PlayEndListeners != null)
        {
            PlayEndListeners();
        }
    }
}
