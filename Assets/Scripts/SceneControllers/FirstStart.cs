using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStart : MonoBehaviour
{
    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        
    }

    public void StartGame()
    {
        BLACK.FadeOut((PlayDataManager.data.IsPlayed) ? "Title" : "Tutorial Scene");
    }
}
