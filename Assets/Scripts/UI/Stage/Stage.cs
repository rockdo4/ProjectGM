using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    public Image image { get; private set; }
    public TextMeshProUGUI title { get; private set; }
    public TextMeshProUGUI description { get; private set; }
    public TextMeshProUGUI enemyName { get; private set; }
    public Button button { get; private set; }

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();

        var middle = transform.Find("MIDDLE");
        title = middle.GetChild(0).GetComponent<TextMeshProUGUI>();
        description = middle.GetChild(1).GetComponent<TextMeshProUGUI>();
        enemyName = middle.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
}
