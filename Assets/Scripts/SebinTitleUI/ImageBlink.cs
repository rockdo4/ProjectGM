using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageBlink : MonoBehaviour
{
    private Image imageComponent;
    public float blinkInterval = 0.5f;

    private bool isBlinking = false;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        StartCoroutine(BlinkImage());
    }

    IEnumerator BlinkImage()
    {
        while (true)
        {
            imageComponent.color = Color.clear;
            yield return new WaitForSeconds(blinkInterval);
            imageComponent.color = Color.white;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}