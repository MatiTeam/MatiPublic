using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [Header("UI Btn Group")]
    public Button startBtn;
    public Button continueBtn;
    public Button optionBtn;
    public Button exitBtn;
    public Button closeOptionBtn;
    public Button warningConfirmBtn;

    [Header("Panel")]
    public GameObject optionPanel;
    public GameObject warningPanel;

    void Start()
    {
        startBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        exitBtn.onClick.AddListener(Exit);
        optionBtn.onClick.AddListener(OpenOptionPanel);
        closeOptionBtn.onClick.AddListener(CloseOptionPanel);
        warningConfirmBtn.onClick.AddListener(CloseWarningPanel);
    }

    // 게임 씬 이동
    //public void StartGame()
    //{
    //    Time.timeScale = 1;
    //    // SceneManager.LoadScene(3);
    //    DataManager.Instance.progressManager.locationData.sceneNumber = 0;
    //    DataManager.Instance.progressManager.locationData.playerPosX = 0;
    //    DataManager.Instance.progressManager.locationData.playerPosY = 0;
    //    SceneManager.LoadScene(4);
    //}

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LastSavedFlow");
        GameManager.Instance.uiManager.TransitionToScene("StoryScene", () => {
            DataManager.Instance.dialogueManager.isFirstDialogue = true;
            DataManager.Instance.flowManager.SetCurrentFlowIndex(0);
        });



        //if (DataManager.Instance.flowManager.GetCurrentFlow().StageType == "Story")
        //    GameManager.Instance.uiManager.TransitionToScene("StoryScene");
        //else if(DataManager.Instance.flowManager.GetCurrentFlow().StageType == "Battle")
        //    GameManager.Instance.uiManager.TransitionToScene("MainScene");
    }

    // 저장된 위치로 이동
    public void ContinueGame()
    {
        if (!DataManager.Instance.flowManager.HasSavedData())
        {
            OpenWarningPanel();
            return;
        }
        
        DataManager.Instance.flowManager.GetLastSavedFlow();
    }

    // 게임 종료
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 옵션창 활성화
    public void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
    }

    // 옵션창 비활성화
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
    }

    // 경고창 활성화
    public void OpenWarningPanel()
    {
        warningPanel.SetActive(true);
    }

    // 경고창 비활성화
    public void CloseWarningPanel()
    {
        warningPanel.SetActive(false);
    }
}