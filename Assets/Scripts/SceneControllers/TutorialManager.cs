using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using CsvHelper.Configuration.Attributes;
using UnityEngine.SceneManagement;
using System.Data;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    [SerializeField]
    private Image tutorialImage;

    [SerializeField]
    private Button touchArea;

    [SerializeField]
    private Image blocker;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Image dialogueType2ImageOne;

    [SerializeField]
    private Image dialogueType2ImageTwo;

    private List<int> tutorialSteps = new List<int>();
    private int currentStepIndex = 0;
    
    private int dialogueType2Count = 0;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
    }

    void Start()
    {
        touchArea.interactable = true;
        dialogueType2ImageOne.enabled = false;
        dialogueType2ImageTwo.enabled = false;

        PauseGame();
        InitializeTutorial();
        ShowTutorialStep(tutorialSteps[currentStepIndex]);
    }

    public void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex < tutorialSteps.Count)
        {
            ShowTutorialStep(tutorialSteps[currentStepIndex]);
        }
        else
        {
            Debug.Log("Ʃ�丮���� �������ϴ�.");
        }
    }

    private void ShowTutorialStep(int stepKey)
    {
        var table = CsvTableMgr.GetTable<DialogueTable>().dataTable;

        if (table.ContainsKey(stepKey))
        {
            var dialogueID = table[stepKey].dialogueID;

            if (dialogueID == 90020001)
            {
                PlayDataManager.data.IsPlayed = true;
                PlayDataManager.Save();
                SceneManager.LoadScene("Title");
                return;
            }
            
            var dialogueText = CsvTableMgr.GetTable<StringTable>().dataTable[dialogueID];
            tutorialText.text = dialogueText;

            if (table[stepKey].dialType == 1)
            {
                StartCoroutine(WaitForSecend());
            }

            if (table[stepKey].dialType == 2)
            {
                switch (dialogueType2Count)
                {
                    case 0:

                        StartCoroutine(DisableInputForSeconds(3.0f));
                        StartCoroutine(WaitForAttackState());
                        break;

                    case 1:
                        Debug.Log("123");
                        StartCoroutine(MoveImage(dialogueType2ImageTwo.transform, 3.0f));
                        StartCoroutine(WaitForEvadeState());
                        break;

                    case 2:

                        SceneManager.LoadScene("Title", LoadSceneMode.Additive);
                        break;
                }

                dialogueType2Count++;

            }
        }
        else
        {
            Debug.Log("�ش� Ű ���� ���� �����Ͱ� �����ϴ�.");
        }
    }

    private IEnumerator WaitForSecend()
    {
        Debug.Log("1��");

        touchArea.interactable = false;

        yield return new WaitForSecondsRealtime(1f);

        touchArea.interactable = true;
    }

    private IEnumerator WaitForAttackState()
    {
        var controller = player.GetComponent<PlayerController>();

        yield return new WaitUntil(() => controller.CurrentState == PlayerController.State.Attack);
        yield return new WaitForSeconds(1f);

        Debug.Log("���� ����Ϸ�");

        touchArea.interactable = true;
        blocker.enabled = true;

        PauseGame();
        NextStep();
    }

    private IEnumerator WaitForEvadeState()
    {
        var controller = player.GetComponent<PlayerController>();
        int count = 0;

        while(count < 3)
        {
            if (controller.LastEvadeState == PlayerController.EvadeState.Just)
            {
                count++;
                Debug.Log(count);

                controller.LastEvadeState = PlayerController.EvadeState.None;

                Debug.Log(controller.LastEvadeState);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.7f); 

        Debug.Log("ȸ�� ����Ϸ�");

        touchArea.interactable = true;
        blocker.enabled = true; 

        PauseGame();
        NextStep();
    }

    private void InitializeTutorial()
    {
        var table = CsvTableMgr.GetTable<DialogueTable>().dataTable;
        tutorialSteps.Clear();

        

        foreach (var pair in table)
        {
            if (pair.Value.dialType == 1 || pair.Value.dialType == 2)
            {
                tutorialSteps.Add(pair.Key);
            }
        }

        tutorialSteps.Sort();
    }

    IEnumerator MoveImage(Transform imageTransform, float duration)
    {
        touchArea.interactable = false;
        dialogueType2ImageTwo.enabled = true;

        Vector3 startPosition = imageTransform.position;
        Vector3 endPosition = new Vector3(startPosition.x + 650, startPosition.y, startPosition.z);

        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            imageTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

            yield return null;
        }

        if (dialogueType2ImageTwo != null)
        {
            dialogueType2ImageTwo.enabled = false;
            blocker.enabled = false;
        }

        ResumeGame();
        imageTransform.position = endPosition;
    }

    IEnumerator DisableInputForSeconds(float seconds)
    {
        touchArea.interactable = false;
        dialogueType2ImageOne.enabled = true;

        yield return new WaitForSecondsRealtime(seconds);

        if(dialogueType2ImageOne != null)
        {
            dialogueType2ImageOne.enabled = false;
            blocker.enabled = false;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
