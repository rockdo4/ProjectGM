using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    public int id;
    public int type;
    public Image image { get; private set; }
    public TextMeshProUGUI title { get; private set; }
    public TextMeshProUGUI mapName { get; private set; }
    public TextMeshProUGUI enemyName { get; private set; }
    public Button button { get; private set; }

    private void Awake()
    {
        image = transform.Find("Icon Image").GetComponent<Image>();
        button = GetComponentInChildren<Button>();

        var middle = transform.Find("MIDDLE");
        title = middle.GetChild(0).GetComponent<TextMeshProUGUI>();
        mapName = middle.GetChild(1).GetComponent<TextMeshProUGUI>();
        enemyName = middle.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
}
