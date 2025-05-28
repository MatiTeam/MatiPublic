using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using System;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;
using TMPro;

public enum EaseType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut
}
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [Header("Camera")]
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0.7f;
    private float decreaseFactor = 1.0f;

    public bool isFadeOut = false;

    [SerializeField] private Image fadePanel;
    [SerializeField] private Canvas canvas;
    public Image FadePanel => fadePanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private EaseType easeType = EaseType.EaseInOut;

    [Header("Panel")]
    public GameObject endingPanel;
    private GameObject endingPanelobj;
    [SerializeField] private TMPro.TextMeshProUGUI saveMessageText; // 저장 메시지 텍스트
    [SerializeField] private float saveMessageDuration = 2f; // 메시지 표시 시간

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // 새로 생성된 건 파괴
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        GameManager.Instance?.RegisterUIManager(this);

        // Canvas가 없으면 생성
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(960, 600);
            canvasObj.transform.SetParent(transform, false);
            canvas.sortingOrder = 1;
        }

        // 페이드 패널이 없으면 생성
        if (fadePanel == null)
        {
            GameObject panelObj = new GameObject("FadePanel");
            panelObj.transform.SetParent(canvas.transform, false);
            fadePanel = panelObj.AddComponent<Image>();
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.raycastTarget = false;

            RectTransform rect = fadePanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
        /*
        if (saveMessageText == null)
        {
            GameObject textObj = new GameObject("SaveMessageText");
            textObj.transform.SetParent(canvas.transform, false);
            saveMessageText = textObj.AddComponent<TextMeshProUGUI>();
            saveMessageText.font = Resources.Load<TMP_FontAsset>("Font/neodgm_code SDF");
            saveMessageText.fontSize = 36;
            saveMessageText.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색
            saveMessageText.alignment = TextAlignmentOptions.Center;
            saveMessageText.text = "저장되었습니다.";
            
            RectTransform rect = saveMessageText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.1f);
            rect.anchorMax = new Vector2(0.5f, 0.1f);
            rect.sizeDelta = new Vector2(200, 50);
            rect.anchoredPosition = Vector2.zero;
            
            saveMessageText.gameObject.SetActive(false);
        }
        */

        if(saveMessageText.transform.parent != canvas.transform)
        {
            saveMessageText.transform.SetParent(canvas.transform, false);
        }
    }

    private void Start()
    {
        // FlowManager의 저장 완료 이벤트 구독
        DataManager.Instance.flowManager.OnSaveCompleted += ShowSaveMessage;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            Camera.main.transform.localPosition = originalPosition + UnityEngine.Random.insideUnitSphere * shakeIntensity;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            Camera.main.transform.localPosition = originalPosition;
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            DataManager.Instance.flowManager.OnSaveCompleted -= ShowSaveMessage;
        }
    }

    public void ShakeCamera(float duration = 0.5f, float intensity = 0.7f)
    {
        originalPosition = Camera.main.transform.localPosition;
        shakeDuration = duration;
        shakeIntensity = intensity;
    }

    private float Ease(float t)
    {
        switch (easeType)
        {
            //기본
            case EaseType.Linear:
                return t;
            //천천히 시작
            case EaseType.EaseIn:
                return t * t;
            //천천히 끝
            case EaseType.EaseOut:
                return 1 - (1 - t) * (1 - t);
            //천천히 시작하고 천천히 끝
            case EaseType.EaseInOut:
                return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            default:
                return t;
        }
    }

    public IEnumerator FadeOut(Action onComplete = null)
    {
        isFadeOut = true;
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float easedT = Ease(t); // 이징 적용
            fadePanel.color = Color.Lerp(startColor, endColor, easedT);
            yield return null;
        }

        fadePanel.color = endColor;
        onComplete?.Invoke();
    }

    public IEnumerator FadeIn(Action onComplete = null)
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float easedT = Ease(t); // 이징 적용
            fadePanel.color = Color.Lerp(startColor, endColor, easedT);
            yield return null;
        }

        fadePanel.color = endColor;
        isFadeOut = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// 씬 전환시 페이드 아웃과 페이드 인을 적용합니다.
    /// ScneneManager.LoadScene 대신 이 함수를 호출합니다. 
    /// </summary>
    /// <param name="sceneName"></param>
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        // 1. 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 2. 씬 비동기 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // 3. 페이드 인
        yield return StartCoroutine(FadeIn());
        DataManager.Instance.flowManager.SaveCurrentFlowIndex();
    }

    /// <summary>
    /// 씬 전환시 페이드 아웃과 페이드 인을 적용합니다.
    /// 오버로드된 함수로, 씬 전환 후 특정 작업을 수행할 수 있습니다.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onFadeOutComplete"></param>
    public void TransitionToScene(string sceneName, Action onFadeOutComplete)
    {
        StartCoroutine(Transition(sceneName, onFadeOutComplete));
    }

    private IEnumerator Transition(string sceneName, Action onFadeOutComplete)
    {
        yield return StartCoroutine(FadeOut());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        onFadeOutComplete?.Invoke();

        yield return StartCoroutine(FadeIn());
    }

    public void ShowEndingPanel()
    {
        StartCoroutine(ShowEndingWithDelay());
    }

    private IEnumerator ShowEndingWithDelay()
    {
        yield return new WaitForSeconds(3f);
        if (endingPanelobj == null)
            endingPanelobj = Instantiate(endingPanel, canvas.transform);
        endingPanelobj.SetActive(true);
    }

    public void CloseEndingPanel()
    {
        if (endingPanelobj != null)
        {
            endingPanelobj.SetActive(false);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    public void BackToTitle()
    {
        TransitionToScene("TitleScene");
    }

    private void ShowSaveMessage()
    {
        StartCoroutine(ShowSaveMessageCoroutine());
    }

    private IEnumerator ShowSaveMessageCoroutine()
    {
        Debug.Log("ShowSaveMessageCoroutine");
        saveMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(saveMessageDuration);
        saveMessageText.gameObject.SetActive(false);
    }
}
