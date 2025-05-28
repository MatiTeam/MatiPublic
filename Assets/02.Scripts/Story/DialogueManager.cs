using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.Networking;
using System.Collections;


#region IDialogue
public interface IDialogue
{
    GameObject Balloon { get; }
    public TextMeshProUGUI DialogueText { get; }
    public void SetDialogue(string data);
    public void ShowBalloon();
    public void HideBalloon();
    void PlayAnimation(string triggerName);
}
#endregion


public class DialogueManager : MonoBehaviour, IDialogueDataProvider
{
    private static DialogueManager instance;
    public Dictionary<string, IDialogue> speakerMap = new Dictionary<string, IDialogue>();
    // 화자 레지스트리 추가
    public Dictionary<string, IDialogue> speakerRegistry = new Dictionary<string, IDialogue>();

    // 화자 데이터 저장
    public Dictionary<string, SpeakerDataSO> speakerDataDic = new Dictionary<string, SpeakerDataSO>();
    public SpeakerDataSO[] Data;

    //DialogueID를 키로 하는 딕셔너리 (대화 내용 저장)
    public Dictionary<string, List<DialogueLinesTableData>> _dialogueLines = new Dictionary<string, List<DialogueLinesTableData>>();

    private bool isLoadedLines = false;
    public DialogueController controller;
    public DialogueView dialogueView;

    private bool isSceneJustLoaded = true;
    public bool isFirstDialogue = true;  // 새로운 변수 추가

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // 새로 생성된 건 파괴
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DataManager.Instance?.RegisterDialogueManager(this);
    }


    /// <summary>
    /// DialogueController의 생성자에서 IDialogueDataProvider와 IFlowDataProvider를 주입합니다.
    /// DialogueView를 가져옵니다.
    /// </summary>
    private async void Start()
    {

        IDialogueDataProvider dataProvider = this;
        IFlowDataProvider flowDataProvider = DataManager.Instance.flowManager;
        controller = new DialogueController(dataProvider, flowDataProvider);
        dialogueView = GetComponent<DialogueView>();

        // 씬 로드 시 대화 시작
        await AsyncLoadLinesData();
        //if (DataManager.Instance.flowManager.CurrentFlowIndex == 0)
        //    flowDataProvider.SetCurrentFlowIndex(5);
        //else
        //    flowDataProvider.SetCurrentFlowIndex(DataManager.Instance.flowManager.CurrentFlowIndex);

        // 씬이 로드되면 true로 설정
        isFirstDialogue = true;
    }

    private void Update()
    {
        //if(!isFirstDialogue)
        //{
        //    isFirstDialogue = true;
        //}
    }

    public bool IsSceneJustLoaded()
    {
        if (isFirstDialogue)
        {
            isFirstDialogue = false;
            return true;
        }
        return false;
    }

    public void RegisterDialogueView(DialogueView view)
    {
        dialogueView = view;

    }

    #region LoadData
    public async Task AsyncLoadLinesData()
    {
        if (isLoadedLines) return;
        string path = $"{Application.streamingAssetsPath}/DialogueData/DialogueLinesTable.json";
        using UnityWebRequest www = UnityWebRequest.Get(path);
        var asyncOp = www.SendWebRequest();

        while (!asyncOp.isDone)
            await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success)
        {
            DialogueDataList list = JsonUtility.FromJson<DialogueDataList>(www.downloadHandler.text);
            foreach (var line in list.lines)
            {
                if (!_dialogueLines.ContainsKey(line.FlowID))
                {
                    _dialogueLines[line.FlowID] = new List<DialogueLinesTableData>();
                }
                _dialogueLines[line.FlowID].Add(line);
            }
            isLoadedLines = true;
        }
        else
        {
            Debug.LogError($"Failed to load dialogue lines data from {path}");
        }
    }
    #endregion

    #region GetData

    /// <summary>
    /// 라인 데이터를 가져옵니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<DialogueLinesTableData> GetLineData(string id)
    {
        return _dialogueLines.TryGetValue(id, out var lines) ? lines : new List<DialogueLinesTableData>();
    }

    /// <summary>
    /// 현재 흐름에 해당하는 라인 데이터를 가져오는 메서드입니다.
    /// DataManager.Instance.dialogueManager.GetCurrentLine()를 통해 호출할 수 있습니다.
    /// 수정예정
    /// </summary>
    /// <returns></returns>
    public List<DialogueLinesTableData> GetCurrentLine()
    {
        int currentIndex = controller.currentIndex; //대사 인덱스
        string flowID = DataManager.Instance.flowManager.flowDataList[DataManager.Instance.flowManager.CurrentFlowIndex].FlowID;
        return GetLineData(flowID);
    }

    /// <summary>
    /// 화자 데이터를 가져오는 메서드입니다.
    /// </summary>
    /// <param name="id">가져올 화자의 ID</param>
    /// <returns>화자 데이터</returns>
    public SpeakerDataSO GetSpeakerByID(string id)
    {
        return speakerDataDic.TryGetValue(id, out var data) ? data : null;
    }

    #endregion

    #region RegisterSpeaker

    /// <summary>
    /// 화자 데이터를 등록하는 메서드입니다.
    /// </summary>
    public void RegisterSpeakerData()
    {
        foreach (var character in Data)
        {
            string id = character.CharacterID;
            if (!speakerDataDic.ContainsKey(id))
            {
                speakerDataDic[id] = character;
                Debug.Log($"화자 데이터 등록: {id}");
            }
            else
                Debug.LogWarning($"이미 등록된 화자 ID: {id}");
        }
    }
    /// <summary>
    /// 화자를 제거하는 메서드입니다.
    /// 씬 전환에 사용합니다.
    /// </summary>
    public void ClearSpeakers()
    {
        speakerDataDic.Clear();
    }
    #endregion

    public void StartDialogueCoroutine(string flowID, float delay)
    {
        StartCoroutine(controller.StartDialogueWithDelay(flowID, delay));
    }

    public bool IsDialogueLoaded()
    {
        return isLoadedLines;
    }

    public void StartToRetryStartDialogue(string flowID)
    {
        StartCoroutine(RetryStartDialogue(flowID));
    }

    public IEnumerator RetryStartDialogue(string flowID)
    {
        yield return new WaitForSeconds(0.1f);
        controller.StartDialogue(flowID);
    }
}
