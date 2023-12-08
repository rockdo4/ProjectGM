using System.Collections;
using TMPro;
using UnityEngine;

public class MyNotice : MonoBehaviour
{
    [Header("지속시간")]
    public float duration = 0.5f;

    private TextMeshProUGUI text;
    private Coroutine coRun = null;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (coRun == null) 
        {
            coRun = StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0.0f;

        while (timer < duration) 
        {
            timer += Time.deltaTime;
           text.color = Color.Lerp(Color.black, Color.clear, timer);

            yield return null;
        }

        coRun = null;
        gameObject.SetActive(false);
    }
}
