using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI 표시를 담당하는 대화 뷰 클래스입니다.
/// </summary>
public class DialogueView : MonoBehaviour
{
    public DialogueEventHandler eventHandler; // 이벤트 핸들러 참조

    private DialogueController controller;
    public IDialogue currentSpeaker; // public으로 변경

    public SpeakerDataSO data;

    [Header("대화 뷰 설정")]
    [SerializeField] private TextMeshProUGUI speakerName; //이름
    [SerializeField] private TextMeshProUGUI dialogueText; //대사
    [SerializeField] private Image currentSpeakerImage; //화자 이미지
    [SerializeField] private Image prevSpeakerImage; //이전 화자 이미지

    private Dictionary<string, bool> speakerPosition = new Dictionary<string, bool>();
    private List<string> assignedSpeakers = new List<string>();

    private Coroutine typingCoroutine;

    private bool isFirstDialogue = true;
    private bool isTypingEffect = false;
    private string currentText;

    [Header("대화 입력 처리")]
    public static bool isProcessingInput = false;
    private float lastInputTime = 0f;
    private float inputCooldown = 0.1f;
    private bool isFlowTransitioning = false;

    public float typingSpeed = 0.04f;

    private void Start()
    {
        DataManager.Instance.dialogueManager.RegisterDialogueView(this);
        controller = DataManager.Instance.dialogueManager.controller;

        // 이벤트 핸들러가 연결되지 않은 경우 찾거나 생성
        if (eventHandler == null)
        {
            eventHandler = gameObject.GetComponent<DialogueEventHandler>();
            if (eventHandler == null)
            {
                eventHandler = gameObject.AddComponent<DialogueEventHandler>();
            }
        }

        // Start 메서드에서 컨트롤러가 초기화된 후 이벤트 구독
        if (controller != null)
        {
            controller.OnDialogueStarted += OnDialogueStarted;
            controller.OnDialogueChanged += OnDialogueChanged;
            controller.OnDialogueEnded += OnDialogueEnded;
        }
    }

    private void OnEnable()
    {
        if (controller != null)
        {
            controller.OnDialogueStarted += OnDialogueStarted;
            controller.OnDialogueChanged += OnDialogueChanged;
            controller.OnDialogueEnded += OnDialogueEnded;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.uiManager.isFadeOut)
        {
            return;
        }

        // 키/마우스 입력 처리
        bool inputDetected = false;

        // E키 입력 감지
        if (Input.GetKeyDown(KeyCode.E))
            inputDetected = true;

        // 입력이 감지된 경우 처리
        if (inputDetected && !isProcessingInput)
        {
            if (Time.time - lastInputTime < inputCooldown)
            {
                return;
            }

            isProcessingInput = true;
            lastInputTime = Time.time;

            if (eventHandler.IsEventRunning)
            {
                Debug.Log("이벤트 실행중");
                isProcessingInput = false;
                return;
            }

            if (isTypingEffect)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                dialogueText.text = currentText;
                isTypingEffect = false;
                isProcessingInput = false;
            }
            else
            {
                // 다음 대화로 진행
                StartCoroutine(ProcessNextDialogue());
            }
        }
    }

    /// <summary>
    /// 테스트 씬에서는 DataManager에 Component로 등록돼 있기 때문에, 누수가 발생할 수 있습니다.
    /// 오브젝트를 분리하거나, 스토리 구간에만 등록되도록 수정해야 합니다. 
    /// </summary>
    private void OnDestroy()
    {
        // 이벤트 해제
        if (controller != null)
        {
            controller.OnDialogueStarted -= OnDialogueStarted;
            controller.OnDialogueChanged -= OnDialogueChanged;
            controller.OnDialogueEnded -= OnDialogueEnded;
        }

        // 혹시 모를 코루틴 누수 방지
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (controller != null)
        {
            controller.OnDialogueStarted -= OnDialogueStarted;
            controller.OnDialogueChanged -= OnDialogueChanged;
            controller.OnDialogueEnded -= OnDialogueEnded;
        }
    }

    /// <summary>
    /// 대화 시작 이벤트 처리
    /// </summary>
    /// <param name="sender"></param>
    private void OnDialogueStarted(object sender)
    {
        if (isFirstDialogue)
        {
            assignedSpeakers.Clear();
            speakerPosition.Clear();

            currentSpeakerImage.gameObject.SetActive(true);
            prevSpeakerImage.gameObject.SetActive(true);
            currentSpeakerImage.color = new Color(1f, 1f, 1f, 0f);
            prevSpeakerImage.color = new Color(1f, 1f, 1f, 0f);

            dialogueText.gameObject.SetActive(false);

            // 스프라이트도 null로 초기화 (중요!)
            currentSpeakerImage.sprite = null;
            prevSpeakerImage.sprite = null;

            isFirstDialogue = false;
            Debug.Log("새 대화 시작, 화자 정보 초기화");
        }

        // 현재 대화 블록의 이벤트 트리거 확인
        string flowId = DataManager.Instance.flowManager.GetCurrentFlowID();
        var lines = DataManager.Instance.dialogueManager.GetCurrentLine();

        // 이벤트 트리거가 있는지 확인하고 있다면 EventHandler를 통해 실행
        if (lines != null && lines.Count > 0)
        {
            var currentLine = lines[controller.currentIndex];
            if (!string.IsNullOrEmpty(currentLine.EventCall))
            {
                eventHandler?.TriggerEvent(currentLine.EventCall);
            }
            if (!string.IsNullOrEmpty(currentLine.CameraEvent))
            {
                eventHandler?.CameraEvent();
            }
            if (!string.IsNullOrEmpty(currentLine.SFX))
            {
                eventHandler?.SFXEvent(currentLine.SFX);
            }
            
        }

    }

    /// <summary>
    /// 대화 블록 변경 이벤트 처리
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDialogueChanged(object sender, DialogueEventArgs e)
    {
        var currentSpeaker = e.Speaker;

        bool isExtraSpeaker = string.IsNullOrEmpty(DataManager.Instance.dialogueManager.GetSpeakerByID(currentSpeaker).Sprite);

        if (isExtraSpeaker)
        {
            // 엑스트라 대사는 이미지 없이 텍스트만 표시
            currentSpeakerImage.color = new Color(1f, 1f, 1f, 0f);
            prevSpeakerImage.color = new Color(1f, 1f, 1f, 0f);

            speakerName.text = DataManager.Instance.dialogueManager.GetSpeakerByID(currentSpeaker).Name;
            currentText = e.Text; // 현재 텍스트 저장
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypingText(currentText));
            dialogueText.gameObject.SetActive(true);
            return;
        }

        if (!assignedSpeakers.Contains(currentSpeaker))
        {
            // 2명까지만 관리
            if (assignedSpeakers.Count >= 2)
            {
                string oldestSpeaker = assignedSpeakers[0];
                assignedSpeakers.RemoveAt(0);
                speakerPosition.Remove(oldestSpeaker);
            }

            bool isLeft = assignedSpeakers.Count % 2 == 0;
            speakerPosition[currentSpeaker] = isLeft;
            assignedSpeakers.Add(currentSpeaker);

            var sprite = Resources.Load<Sprite>($"Sprite/{DataManager.Instance.dialogueManager.GetSpeakerByID(currentSpeaker).Sprite}");
            if (sprite != null)
            {
                if (isLeft)
                    currentSpeakerImage.sprite = sprite;
                else
                    prevSpeakerImage.sprite = sprite;
            }
            else
            {
                Debug.Log($"스프라이트 로드 실패: {currentSpeaker}");
                currentSpeakerImage.color = new Color(1f, 1f, 1f, 0f);
                prevSpeakerImage.color = new Color(1f, 1f, 1f, 0f);
            }

        }

        foreach (var speakerId in assignedSpeakers)
        {
            bool isCurrentSpeaker = speakerId == currentSpeaker;
            bool isLeft = speakerPosition[speakerId];

            Image speakerImage = isLeft ? currentSpeakerImage : prevSpeakerImage;

            speakerImage.color = isCurrentSpeaker ?
            new Color(1f, 1f, 1f, 1f) :
            new Color(1f, 1f, 1f, 0.5f);
        }

        speakerName.text = DataManager.Instance.dialogueManager.GetSpeakerByID(currentSpeaker).Name;
        currentText = e.Text; // 현재 텍스트 저장
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypingText(currentText));
        dialogueText.gameObject.SetActive(true);
    }


    /// <summary>
    /// 대화 종료 이벤트 처리
    /// </summary>
    private void OnDialogueEnded(object sender)
    {
        Debug.Log("대화 종료 이벤트 발생");

        //다음 대화를 위한 플래그 재설정
        isFlowTransitioning = true;
        isFirstDialogue = true;
        DataManager.Instance.flowManager.MoveToNextFlow();
        DataManager.Instance.flowManager.SetCurrentFlowIndex(DataManager.Instance.flowManager.CurrentFlowIndex);
    }

    private IEnumerator TypingText(string e)
    {
        isTypingEffect = true;
        dialogueText.text = "";
        currentText = e;

        foreach (char c in currentText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        isProcessingInput = false; // 타이핑이 완료되면 입력 처리 상태 해제
    }

    private IEnumerator ProcessNextDialogue()
    {
        isFlowTransitioning = true;

        controller.ProceedToNextLineDialogue();
        if (!isTypingEffect)
        {
            isProcessingInput = false;
        }
        else
        {
            yield return new WaitUntil(() => !isTypingEffect);
            isProcessingInput = false;
        }

        yield return new WaitForSeconds(2f);

        isFlowTransitioning = false;

        
    }
}

