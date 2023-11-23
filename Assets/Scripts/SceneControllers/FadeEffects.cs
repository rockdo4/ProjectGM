using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeEffects : MonoBehaviour
{
    public float t;
    private Image image;
    private Coroutine coRun = null;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        if (coRun == null)
        {
            coRun = StartCoroutine(CoFadeIn());
        }
    }

    public void FadeOut(string sceneName)
    {
        if (coRun == null)
        {
            coRun = StartCoroutine(CoFadeOut(sceneName));
        }
    }

    private IEnumerator CoFadeIn()
    {
        float time = 0.0f;

        while (time < t)
        {
            time += Time.unscaledDeltaTime;
            image.color = Color.Lerp(Color.black, Color.clear, time / t);

            yield return null;
        }
        coRun = null;
    }

    private IEnumerator CoFadeOut(string sceneName)
    {
        float time = 0.0f;

        while (time < t)
        {
            time += Time.unscaledDeltaTime;
            image.color = Color.Lerp(Color.clear, Color.black, time / t);

            yield return null;
        }
        coRun = null;
        SceneManager.LoadScene(sceneName);
    }
}
