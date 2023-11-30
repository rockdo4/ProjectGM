using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestLogManager : MonoBehaviour
{
    public static TestLogManager Instance;
    public List<GameObject> UIPrefab;
    private Dictionary<GameObject, Action> uiList = new Dictionary<GameObject, Action> ();

    private bool isHide = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        foreach (var go in uiList)
        {
            if (go.Value != null)
            {
                go.Value.Invoke ();
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ActiveToggle();
        }
    }

    public GameObject MakeUI(Action updateAction = null, int index = 0)
    {
        var ui = Instantiate(UIPrefab[index]);


        ui.transform.SetParent(transform, false);
        uiList.Add(ui, updateAction);

        return ui;
    }

    public void ActiveToggle()
    {
        foreach(var ui in uiList)
        {
            ui.Key.gameObject.SetActive(isHide);
        }

        isHide = !isHide;
    }
}
