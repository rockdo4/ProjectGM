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

    private List<int> tutorialSteps = new List<int>();
    private int currentStepIndex = 0;

    

    // 생각해봐야 될게 예를 들어 3번째 키값에서는 잠깐 전투가 진행될거라 그동안은 타임스케일이 1이 되어야한다 라던가
    // 5번째 키값에서는 퀘스트가 진행되어 뭔가 진행이 달라진다거나 등등

    // 몇초 재는거는 타임으로 할수 없음
    // 그래서 몇초 재는거는 코루틴으로 할 수 밖에 없음

    void Start()
    {
        PauseGame();
        InitializeTutorialSteps();
        ShowTutorialStep(tutorialSteps[currentStepIndex]);
    }

    public void NextStep()
    {
        currentStepIndex++; // 다음 단계 인덱스로 이동
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

        Debug.Log(stepKey);

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
        }
        else
        {
            Debug.Log("해당 키 값에 대한 데이터가 없습니다.");
        }
    }

    private void InitializeTutorialSteps()
    {
        var table = CsvTableMgr.GetTable<DialogueTable>().dataTable;

        tutorialSteps = table.Keys.ToList(); // 테이블의 모든 키를 리스트로 변환
        tutorialSteps.Sort();
    }

    IEnumerator MoveImage(Transform imageTransform, float duration)
    {
        Vector3 startPosition = imageTransform.position; // 시작 위치
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + 300, startPosition.z); // 끝 위치 (위로 100 단위 이동)

        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            imageTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            //..elapsedTime += Time.deltaTime;
            yield return null;
        }

        imageTransform.position = endPosition; // 최종 위치로 설정
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
