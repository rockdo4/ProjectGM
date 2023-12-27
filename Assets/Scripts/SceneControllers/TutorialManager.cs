using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    private List<int> tutorialSteps = new List<int>();
    private int currentStepIndex = 0;


    // 생각해봐야 될게 예를 들어 3번째 키값에서는 잠깐 전투가 진행될거라 그동안은 타임스케일이 1이 되어야한다 라던가
    // 5번째 키값에서는 퀘스트가 진행되어 뭔가 진행이 달라진다거나 등등

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
        if (table.ContainsKey(stepKey))
        {
            var dialogueID = table[stepKey].dialogueID;
            var dialogueText = CsvTableMgr.GetTable<StringTable>().dataTable[dialogueID];

            Debug.Log(dialogueText);
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

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
