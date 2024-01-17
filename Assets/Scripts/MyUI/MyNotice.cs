using System.Collections;
using TMPro;
using UnityEngine;

public class MyNotice : MonoBehaviour
{
    public static MyNotice Instance;

    [Header("지속시간")]
    public float duration = 0.5f;

    private Color startColor;
    private TextMeshProUGUI text;
    private Coroutine coRun = null;

    public void Notice(string str)
    {
        text.text = str;
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;

        text = GetComponent<TextMeshProUGUI>();
        startColor = text.color;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (coRun == null) 
        {
            coRun = StartCoroutine(FadeOut());
        }
        else
        {
            StopCoroutine(coRun);
            coRun = StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0.0f;

        while (timer < duration) 
        {
           timer += Time.deltaTime;
           text.color = Color.Lerp(startColor, Color.clear, timer / duration);

            yield return null;
        }

        coRun = null;
        gameObject.SetActive(false);
    }
}
