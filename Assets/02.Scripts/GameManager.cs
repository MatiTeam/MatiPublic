using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public UIManager uiManager { get; private set; }

    private PlayerStat playerStat;

    [Header("튜토리얼")]
    [SerializeField] private TutorialController tutorialController;
    private bool isTutorialMap = false;

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public void RegisterUIManager(UIManager manager)
    {
        uiManager = manager;
    }
    public void SetTutorialMap(bool isTutorial)
    {
        isTutorialMap = isTutorial;
        if (tutorialController != null)
        {
            tutorialController.gameObject.SetActive(isTutorial);
        }
    }

    public void StartTutorial()
    {
        if (!isTutorialMap || tutorialController == null) return;
        tutorialController.StartTutorial();
    }

    public void StopTutorial()
    {
        if (!isTutorialMap || tutorialController == null) return;
        tutorialController.StopTutorial();
    }

    // 튜토리얼 맵 로드 시 호출
    public void OnTutorialMapLoaded(TutorialController newTutorialController)
    {
        if (tutorialController != null)
        {
            tutorialController.gameObject.SetActive(false);
        }
        tutorialController = newTutorialController;
        SetTutorialMap(true);
    }

    // 튜토리얼 맵 언로드 시 호출
    public void OnTutorialMapUnloaded()
    {
        if (tutorialController != null)
        {
            tutorialController.StopTutorial();
            tutorialController.gameObject.SetActive(false);
        }
        tutorialController = null;
        SetTutorialMap(false);
    }

    // 새로운 씬 자체를 만들어서 게임 시작 시 타이틀 씬으로 불러오게 하고 씬은 소멸시키는 방식
    // 함수로써 호출하고 오디오나 그런 것은 Resorce를 통해 불러오게 하기

}
