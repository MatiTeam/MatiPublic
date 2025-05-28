using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDialogueDataProvider
{
    List<DialogueLinesTableData> GetLineData(string flowID);
    List<DialogueLinesTableData> GetCurrentLine();

    void RegisterSpeakerData();

    bool IsSceneJustLoaded();
    void StartDialogueCoroutine(string flowID, float delay);

    bool IsDialogueLoaded();

    // 기타 필요한 데이터 접근 메서드들
}

public interface IFlowDataProvider
{
    FlowData GetCurrentFlow();
    List<FlowData> GetFlowDataList();
    string GetCurrentFlowID();
    int GetCurrentFlowIndex();
    void SetCurrentFlowIndex(int index);
    void MoveToNextFlow();

    event Action<string> OnFlowIDChanged;
}

/// <summary>
/// 전반적인 데이터를 관리하는 클래스입니다.
/// 이 곳에서 데이터를 로드합니다.
/// </summary>
public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    public DialogueManager dialogueManager { get; private set; }
    public FlowManager flowManager { get; private set; }
    public SkillManager skillManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 각 데이터 매니저 초기화
            dialogueManager = new DialogueManager();
            flowManager = new FlowManager();
            skillManager = new SkillManager();
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        LoadAllData();
        //#if UNITY_EDITOR
        //PlayerPrefs.DeleteKey("LastSavedFlow");
        //#endif

        //Debug.Log(flowManager.flowDataList[0].FlowID);
    }

    public void RegisterDialogueManager(DialogueManager manager)
    {
        dialogueManager = manager;
    }
    public void RegisterFlowManager(FlowManager manager)
    {
        flowManager = manager;
    }

    public void LoadAllData()
    {
        flowManager.LoadFlowData();
        skillManager.skillDatas.LoadJson_SkillData();
    }
}
