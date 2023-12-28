using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using CsvHelper.Configuration.Attributes;

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

    private List<int> tutorialSteps = new List<int>();
    private int currentStepIndex = 0;
    
    private int dialogueType2Count = 0;

    // 첫번재 다이얼 타입2는 공격 실행이다.
    // 1. 그러면 처음에는 타임스케일을 정상적으로 돌려야한다
    // 2. 공격이 실행될때까지는 버튼은 비활성해야한다.
    // 3. 조건이 수행되면 다시 버튼을 활성화 시킨다.
    // 4. 이어서 다음이 되는 다이얼로그 타입 1을 재생시킨다

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

        if (table.ContainsKey(stepKey))
        {
            var dialogueID = table[stepKey].dialogueID;
            var dialogueText = CsvTableMgr.GetTable<StringTable>().dataTable[dialogueID];

            //Debug.Log(dialogueText);
            tutorialText.text = dialogueText;

            if(table[stepKey].dialType == 2)
            {
                Debug.Log("다이얼타입 2 진입");

                StartCoroutine(DisableInputForSeconds(3.0f));

                //ResumeGame();
                //touchArea.interactable = false; // 임시

                switch (dialogueType2Count)
                {
                    case 0:

                        StartCoroutine(WaitForAttackState());
                        break;

                    case 1:

                        StartCoroutine(MoveImage(tutorialImage.transform, 1.0f));
                        StartCoroutine(WaitForEvadeState());
                        break;

                    case 2:
                    
                        // 스테이지 진입
                        // 타이틀씬으로 가면 됨

                        break;

                    case 3:
                        
                        break;

                }

                dialogueType2Count++;

            }

        }
        else
        {
            Debug.Log("해당 키 값에 대한 데이터가 없습니다.");
        }
    }

    private IEnumerator WaitForAttackState()
    {
        var controller = player.GetComponent<PlayerController>();

        yield return new WaitUntil(() => controller.CurrentState == PlayerController.State.Attack);
        yield return new WaitForSeconds(1f);
        Debug.Log("공격 실행완료");

        touchArea.interactable = true;
        blocker.enabled = true;

        PauseGame();
        NextStep();
    }

    private IEnumerator WaitForEvadeState()
    {
        var controller = player.GetComponent<PlayerController>();

        yield return new WaitUntil(() => controller.CurrentState == PlayerController.State.Evade);
        yield return new WaitForSeconds(1f);

        Debug.Log("회피 실행완료");

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
        Vector3 startPosition = imageTransform.position;
        Vector3 endPosition = new Vector3(startPosition.x + 300, startPosition.y, startPosition.z);

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
