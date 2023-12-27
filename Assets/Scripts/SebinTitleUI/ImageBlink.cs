using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageBlink : MonoBehaviour
{
    public Image imageComponent;
    public float blinkInterval = 0.5f;

    private bool isBlinking = false;

    void Start()
    {
        if (imageComponent == null)
        {
            Debug.LogError("Image component not assigned!");
            return;
        }

        StartCoroutine(BlinkImage());
    }

    IEnumerator BlinkImage()
    {
        while (true)
        {
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0f);
            yield return new WaitForSeconds(blinkInterval);
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1f);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}