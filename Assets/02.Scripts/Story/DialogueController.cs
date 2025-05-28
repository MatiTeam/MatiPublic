using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;
using System.Collections;

public class DialogueEventArgs : EventArgs
{
    public string Speaker { get; set; }
    public string Text { get; set; }
    public int CurrentIndex { get; set; }
    public int TotalCount { get; set; }
}

public class DialogueController
{
    #region 이벤트
    public event Action<IDialogue> OnDialogueStarted;
    public event EventHandler<DialogueEventArgs> OnDialogueChanged;
    public event Action<IDialogue> OnDialogueEnded;
    #endregion

    public int currentIndex = 0; // 현재 불러온 페이즈의 대사 인덱스 =/ json의 고유ID와 관계없음
    public List<DialogueLinesTableData> currentLine; // 현재 대사

    public IDialogue currentSpeaker; // 현재 대사 화자
    private readonly IDialogueDataProvider dialogueDataProvider;
    private readonly IFlowDataProvider flowDataProvider;

    #region 생성자
    public DialogueController(IDialogueDataProvider dialogueDataProvider, IFlowDataProvider flowDataProvider)
    {
        this.dialogueDataProvider = dialogueDataProvider;
        this.flowDataProvider = flowDataProvider;
        flowDataProvider.OnFlowIDChanged += StartDialogue;
    }
    #endregion


    /// <summary>
    /// ST로 시작되는 플로우면 자동으로 대화가 시작됩니다.
    /// </summary>
    /// <param name="id"></param>
    public void StartDialogue(string FlowID)
    {
        DataManager.Instance.flowManager.SaveCurrentFlowIndex();
        // FlowID가 ST로 시작하는 경우에만, 예외처리
        if (FlowID.StartsWith("ST"))
        {
            // 씬이 방금 로드된 경우인지 확인
            bool isFirstDialogue = dialogueDataProvider.IsSceneJustLoaded();

            // 씬이 방금 로드된 경우에는 페이드 효과 없이 바로 대화 시작
            if (isFirstDialogue)
            {
                if (!dialogueDataProvider.IsDialogueLoaded())
                {
                    Debug.LogWarning("데이터 로드가 완료되지 않았습니다. 잠시 후 다시 시도합니다.");
                    // 0.1초 후에 다시 시도
                    DataManager.Instance.dialogueManager.isFirstDialogue = true;
                    DataManager.Instance.dialogueManager.StartToRetryStartDialogue(FlowID);
                    return;

                }
                SetCurrentData(FlowID, currentLine);
                Debug.Log($"현재 블록 대사 개수: {currentLine.Count}, 현재 인덱스: {currentIndex}");

                //화자 등록  
                if (DataManager.Instance.dialogueManager.speakerDataDic.Count == 0)
                {
                    Debug.LogWarning("화자 데이터가 없습니다. 등록을 시도합니다.");
                    dialogueDataProvider.RegisterSpeakerData();
                }

                currentIndex = 0;
                ShowCurrentDialogue();
            }
            else
            {
                if (FlowID == "ST0105")
                    dialogueDataProvider.StartDialogueCoroutine(FlowID, 1.0f);
                else
                    dialogueDataProvider.StartDialogueCoroutine(FlowID, 0.3f);
            }
            if (!string.IsNullOrEmpty(flowDataProvider.GetCurrentFlow().BGM))
            {
                DataManager.Instance.dialogueManager.dialogueView.eventHandler.BGMEvent(flowDataProvider.GetCurrentFlow().BGM);
            }
            else if (string.IsNullOrEmpty(flowDataProvider.GetCurrentFlow().BGM) && isFirstDialogue)
            {
                string previousBGM = FindPreviousBGM(flowDataProvider.GetCurrentFlowIndex());
                DataManager.Instance.dialogueManager.dialogueView.eventHandler.BGMEvent(previousBGM);
            }
        }
    }

    public IEnumerator StartDialogueWithDelay(string FlowID, float delay)
    {
        yield return GameManager.Instance.uiManager.FadeOut(() => { });
        yield return new WaitForSeconds(delay);

        SetCurrentData(FlowID, currentLine);
        Debug.Log($"현재 대사 개수: {currentLine.Count}, 현재 인덱스: {currentIndex}");

        //블록 또는 대사가 없다면 오류 발생
        if (currentLine == null || currentLine.Count == 0)
        {
            Debug.LogWarning("블록 또는 대사가 없습니다.");
            yield break;
        }

        currentIndex = 0;
        ShowCurrentDialogue();
        yield return GameManager.Instance.uiManager.FadeIn(() => { });

    }

    public void ShowCurrentDialogue()
    {
        if (currentIndex >= currentLine.Count)
        {
            EndDialogue();
            return;
        }

        if (DataManager.Instance.dialogueManager.speakerDataDic.Count == 0)
        {
            Debug.LogWarning("화자 데이터가 없습니다. 등록을 시도합니다.");
            dialogueDataProvider.RegisterSpeakerData();
        }

        OnDialogueStarted?.Invoke(currentSpeaker);

        Debug.Log($"현재 대사 개수: {currentLine.Count}, 현재 인덱스: {currentIndex}");
        var line = currentLine[currentIndex];


        //복합 정보를 담은 이벤트 인자 사용
        var args = new DialogueEventArgs
        {
            Speaker = line.Speaker,
            Text = line.Text,
            CurrentIndex = currentIndex,
            TotalCount = currentLine.Count
        };

        OnDialogueChanged?.Invoke(args.Speaker, args);
    }

    /// <summary>
    /// 인덱스 증가시키고 다음 대사 출력
    /// </summary>
    public void ProceedToNextLineDialogue()
    {
        if (currentIndex + 1 < currentLine.Count)
        {
            currentIndex++;
            ShowCurrentDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// 대화 종료 시 이벤트 발생
    /// </summary>
    public void EndDialogue()
    {
        OnDialogueEnded?.Invoke(currentSpeaker);
    }



    public void SetCurrentData(string flowID, List<DialogueLinesTableData> line)
    {
        var id = flowDataProvider.GetCurrentFlowID();
        switch (id)
        {
            case "ST0101":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0101 블록 불러오기");
                break;
            case "ST0102":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0102 블록 불러오기");
                break;
            case "ST0103":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0103 블록 불러오기");
                break;
            case "ST0104":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0104 블록 불러오기");
                break;
            case "ST0105":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0105 블록 불러오기");
                break;
            case "ST0106":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0106 블록 불러오기");
                break;
            case "ST0107":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0107 블록 불러오기");
                break;
            case "ST0108":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0108 블록 불러오기");
                break;
            case "ST0109":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0109 블록 불러오기");
                break;
            case "ST0110":
                line = dialogueDataProvider.GetLineData(id);
                currentLine = line;
                Debug.Log("ST0110 블록 불러오기");
                break;
            default: return;
        }
    }

    private string FindPreviousBGM(int currentIndex)
    {
        // 현재 인덱스부터 0까지 역순으로 검색
        for (int i = currentIndex; i >= 0; i--)
        {
            var flow = flowDataProvider.GetFlowDataList()[i];
            if (!string.IsNullOrEmpty(flow.BGM))
            {
                return flow.BGM;
            }
        }
        return null; // 이전 BGM을 찾지 못한 경우
    }


}
