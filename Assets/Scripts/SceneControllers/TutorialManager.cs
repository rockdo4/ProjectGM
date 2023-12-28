using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    [SerializeField]
    private Image tutorialImage;

    [SerializeField]
    private Button touchArea;

    private List<int> tutorialSteps = new List<int>();
    private int currentStepIndex = 0;

    // 생각해봐야 될게 예를 들어 3번째 키값에서는 잠깐 전투가 진행될거라 그동안은 타임스케일이 1이 되어야한다 라던가
    // 5번째 키값에서는 퀘스트가 진행되어 뭔가 진행이 달라진다거나 등등

    // 몇초 재는거는 타임으로 할수 없음
    // 그래서 몇초 재는거는 코루틴으로 할 수 밖에 없음

    void Start()
    {
        touchArea.interactable = true;

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
            Debug.Log("튜토리얼이 끝났습니다.");
        }

    }

    private void ShowTutorialStep(int stepKey)
    {
        var table = CsvTableMgr.GetTable<DialogueTable>().dataTable;

        //Debug.Log(stepKey);

        if (stepKey == 1001007)
        {

            StartCoroutine(MoveImage(tutorialImage.transform, 1.0f));
        }

        if (stepKey == 1001007) // 대놓고 지정하는건데 이런 지정 없이 할 수가 있나?
        {
            Debug.Log("123123123123123");

            StartCoroutine(MoveImage(tutorialImage.transform, 1.0f));
        }

        if (table.ContainsKey(stepKey))
        {
            var dialogueID = table[stepKey].dialogueID;
            var dialogueText = CsvTableMgr.GetTable<StringTable>().dataTable[dialogueID];

            Debug.Log(dialogueText);
            tutorialText.text = dialogueText;

            if(table[stepKey].dialType == 2)
            {
                Debug.Log("다이얼타입 2 진입");

                StartCoroutine(DisableInputForSeconds(3.0f));
            }

        }
        else
        {
            Debug.Log("해당 키 값에 대한 데이터가 없습니다.");
        }
    }

    private void InitializeTutorial()
    {
        var table = CsvTableMgr.GetTable<DialogueTable>().dataTable;

        tutorialSteps.Clear();


        foreach (var pair in table)
        {
            if (pair.Value.dialType == 1)
            {
                tutorialSteps.Add(pair.Key);
            }
        }

        //tutorialSteps = table.Keys.ToList();
        tutorialSteps.Sort();
    }

    IEnumerator MoveImage(Transform imageTransform, float duration)
    {
        Vector3 startPosition = imageTransform.position;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + 300, startPosition.z);

        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            imageTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            yield return null;
        }

        imageTransform.position = endPosition;
    }

    IEnumerator DisableInputForSeconds(float seconds)
    {
        touchArea.interactable = false;

        yield return new WaitForSecondsRealtime(seconds);

        touchArea.interactable = true;
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
