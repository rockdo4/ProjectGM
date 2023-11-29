using UnityEngine;

public class TestLogManager : MonoBehaviour
{
    public TestLogManager Instance;

    public GameObject UIPrefab;
    public Transform targetTransform;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject AddUI()
    {
        var ui = Instantiate(UIPrefab);

        //SetText

        ui.transform.SetParent(targetTransform, false);
        return ui;
    }

    public void SetText(string text, Color? co = null)
    {
        var color = co ?? Color.black;

    }
}
