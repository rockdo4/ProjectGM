using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageBlink : MonoBehaviour
{
    private Image image;
    public float blinkInterval = 0.5f;

    private void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(BlinkImage());
    }

    private IEnumerator BlinkImage()
    {
        while (true)
        {
            image.color = Color.clear;
            yield return new WaitForSeconds(blinkInterval);
            image.color = Color.white;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}