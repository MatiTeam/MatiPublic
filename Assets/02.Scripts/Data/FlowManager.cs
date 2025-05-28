using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

/// <summary>
/// FlowManager는 게임의 흐름을 관리하는 클래스입니다.
/// DontDestroyOnLoad를 사용하며 씬 전환 시에도 유지됩니다.
/// </summary>
public class FlowManager : MonoBehaviour, IFlowDataProvider
{
    private static FlowManager instance;
    public List<FlowData> flowDataList { get; private set; } = new List<FlowData>(); // FlowData를 담는 리스트
    private string flowPath = "DialogueData/Flow";
    public int CurrentFlowIndex { get; private set; } = 0;
    public event Action<string> OnFlowIDChanged; // FlowID가 변경될 때 발생하는 이벤트
    public event Action OnSaveCompleted; // 저장 완료 시 발생하는 이벤트
    private bool isSceneTransitioning = false;

    /// <summary>
    /// Awake 메서드에서 DataManager에 FlowManager를 등록합니다.
    /// 씬 전환시에도 FlowManager가 유지되도록 DontDestroyOnLoad를 설정합니다.
    /// StartScene에만 배치합니다.
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // 새로 생성된 건 파괴
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DataManager.Instance?.RegisterFlowManager(this);
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneloaded;
    }

    private void OnSceneloaded(Scene scene)
    {
        if (scene.name == "StoryScene")
        {
            StartCoroutine(ResetTransitionState());
        }
    }



    /// <summary>
    /// FlowData를 로드합니다.
    /// </summary>
    public void LoadFlowData()
    {
        TextAsset json = Resources.Load<TextAsset>(flowPath);
        if (json == null)
        {
            Debug.LogError($"Failed to load flow data from {flowPath}");
            return;
        }

        FlowDataList list = JsonUtility.FromJson<FlowDataList>(json.text);
        flowDataList = list.flow;
    }

    /// <summary>
    /// 현재 flowData를 가져옵니다. 
    /// 스토리 진행 시, 혹은 스테이지 변경에 따른 맵 변경시 호출합니다. 
    /// </summary>
    /// <returns></returns>
    public FlowData GetCurrentFlow()
    {
        if (CurrentFlowIndex < 0 || CurrentFlowIndex >= flowDataList.Count)
        {
            Debug.LogWarning("Current flow index is out of range");
            return null;
        }

        return flowDataList[CurrentFlowIndex];
    }

    /// <summary>
    /// flowDataList를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public List<FlowData> GetFlowDataList()
    {
        return flowDataList;
    }

    /// <summary>
    /// 현재FlowID를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentFlowID()
    {
        var currentFlow = GetCurrentFlow();
        return currentFlow != null ? currentFlow.FlowID : string.Empty;
    }

    /// <summary>
    /// 현재 FlowIndex를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentFlowIndex()
    {
        return CurrentFlowIndex;
    }

    public string GetCurrentMapID() { return flowDataList[CurrentFlowIndex].MapID; }

    /// <summary>
    /// currentFlowIndex를 설정하는 함수입니다. 
    /// Flow가 변경될 때 사용합니다. 
    /// </summary>
    /// <param name="index"></param>
    public void SetCurrentFlowIndex(int index)
    {
        if (index >= 0 && index < flowDataList.Count)
        {
            CurrentFlowIndex = index;
            GetCurrentFlow();
            if (GetCurrentFlowID().StartsWith("ST"))
            {
                OnFlowIDChanged?.Invoke(GetCurrentFlowID());
            }
            else if (GetCurrentFlowID().StartsWith("BA"))
            {
                DataManager.Instance.dialogueManager.ClearSpeakers();
                GameManager.Instance.uiManager.TransitionToScene("MainScene");
            }
            else if (GetCurrentFlowID().StartsWith("BS"))
            {
                DataManager.Instance.dialogueManager.ClearSpeakers();
                GameManager.Instance.uiManager.TransitionToScene("BossScene");
            }
            else
                Debug.Log($"Current FlowID: {GetCurrentFlowID()}");
        }
        else
        {
            Debug.LogError($"Invalid flow index: {index}. Valid range is 0-{flowDataList.Count - 1}");
        }
    }

    /// <summary>
    /// 다음 Flow로 전환하는 함수입니다.
    /// </summary>
    public void MoveToNextFlow()
    {
        if (CurrentFlowIndex < flowDataList.Count - 1)
        {
            CurrentFlowIndex++;
            AnalyticsManager.Instance.SendFunnelStep(CurrentFlowIndex);
        }
        else
        {
            Debug.Log("Reached the end of flow data list");
        }
    }

    /// <summary>
    /// 특정 Index에 해당하는 흐름 데이터를 가져옵니다.
    /// 특정 Index에 해당하는 데이터 강제 불러오기, 디버깅용 함수입니다. 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public FlowData GetFlowData(int index)
    {
        return flowDataList[index] != null ? flowDataList[index] : new FlowData();
    }

    public void MoveToStoryScene()
    {
        isSceneTransitioning = true;
        GameManager.Instance.uiManager.TransitionToScene("StoryScene", () =>
        {
            DataManager.Instance.dialogueManager.isFirstDialogue = true;
            SetCurrentFlowIndex(GetCurrentFlowIndex());
        });
    }

    public void SaveCurrentFlowIndex()
    {
        PlayerPrefs.SetInt("LastSavedFlow", GetCurrentFlowIndex());
        PlayerPrefs.Save();
        OnSaveCompleted?.Invoke();
    }

    public void GetLastSavedFlow()
    {
        if (PlayerPrefs.HasKey("LastSavedFlow"))
        {
            int index = PlayerPrefs.GetInt("LastSavedFlow");
            CurrentFlowIndex = index;
            
            if (GetCurrentFlowID().StartsWith("ST"))
                MoveToStoryScene();
            else
                SetCurrentFlowIndex((int)CurrentFlowIndex);
        }
    }

    public bool HasSavedData()
    {
        return PlayerPrefs.HasKey("LastSavedFlow");
    }

    private IEnumerator ResetTransitionState()
    {
        // 1초 정도 대기하여 씬 전환이 완전히 안정화되도록 함
        yield return new WaitForSeconds(1f);
        isSceneTransitioning = false;

    }
}
