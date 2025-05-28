using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepId;
        public GameObject tutorialObject;    // 튜토리얼 UI 오브젝트 (화살표, 핑 등)
        public string imagePath;             // Resources/Tutorial 폴더 내 이미지 경로
        public bool isCompleted;
        
        public enum CompletionType
        {
            ImageInteraction,  // 이미지 상호작용 (F키)
            PositionReach,    // 특정 위치 도달
            EnemyKill,        // 적 처치
            ObjectDestroy,    // 오브젝트 파괴
            Custom           // 커스텀 조건
        }
        
        public CompletionType completionType;
        public Transform targetPosition;     // 도달해야 할 위치
        public float reachDistance = 1f;     // 도달 판정 거리
        public string targetTag;             // 타겟 오브젝트의 태그
        public Action customCompleteCheck;   // 커스텀 완료 조건 체크
        public ToNextFlow targetFlow;        // 타겟 ToNextFlow 컴포넌트
    }

    [Header("튜토리얼 설정")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    
    [Header("UI 설정")]
    [SerializeField] private GameObject tutorialImagePanel;  // 이미지를 표시할 패널
    [SerializeField] private Image tutorialImage;           // 이미지를 표시할 Image 컴포넌트
    [SerializeField] private GameObject pingPrefab;         // 핑 프리팹

    private int currentStepIndex = 0;
    private Transform playerTransform;
    private bool isTutorialActive = false;
    private bool isTimePaused = false;
    private List<GameObject> targetObjects = new List<GameObject>();
    private GameObject currentPing;          // 현재 생성된 핑
    private List<GameObject> currentPings = new List<GameObject>();  // 모든 핑을 관리하는 리스트

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        GameManager.Instance.OnTutorialMapLoaded(this);
        GameManager.Instance.StartTutorial();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTutorialMapUnloaded();
        ResumeTime();
        DestroyAllPings();
    }

    private void DestroyAllPings()
    {
        foreach (var ping in currentPings)
        {
            if (ping != null)
            {
                Destroy(ping);
            }
        }
        currentPings.Clear();
    }

    public void StartTutorial()
    {
        if (isTutorialActive) return;
        
        isTutorialActive = true;
        currentStepIndex = 0;
        ShowCurrentStep();
    }

    public void StopTutorial()
    {
        if (!isTutorialActive) return;
        
        isTutorialActive = false;
        HideAllUI();
        ResumeTime();
        DestroyAllPings();
    }

    private void PauseTime()
    {
        if (!isTimePaused)
        {
            Time.timeScale = 0f;
            isTimePaused = true;
        }
    }

    private void ResumeTime()
    {
        if (isTimePaused)
        {
            Time.timeScale = 1f;
            isTimePaused = false;
        }
    }

    private void ShowCurrentStep()
    {
        if (currentStepIndex >= tutorialSteps.Count)
        {
            StopTutorial();
            return;
        }

        var currentStep = tutorialSteps[currentStepIndex];

        // 이미지 로드 및 표시
        if (!string.IsNullOrEmpty(currentStep.imagePath))
        {
            Sprite tutorialSprite = Resources.Load<Sprite>($"Tutorial/{currentStep.imagePath}");
            if (tutorialSprite != null && tutorialImage != null)
            {
                tutorialImage.sprite = tutorialSprite;
                tutorialImagePanel.SetActive(true);

                // 이미지가 있는 경우 시간 정지
                if (currentStep.completionType == TutorialStep.CompletionType.ImageInteraction)
                {
                    PauseTime();
                }
            }
        }

        // 게임오브젝트는 항상 활성화
        if (currentStep.tutorialObject != null)
        {
            currentStep.tutorialObject.SetActive(true);
        }

        // EnemyKill 또는 ObjectDestroy 타입인 경우 타겟 오브젝트들을 찾아서 리스트에 추가
        if (currentStep.completionType == TutorialStep.CompletionType.EnemyKill || 
            currentStep.completionType == TutorialStep.CompletionType.ObjectDestroy)
        {
            FindTargetObjects(currentStep.targetTag);
            
            // 모든 타겟 오브젝트 위에 핑 생성
            if (currentStep.completionType == TutorialStep.CompletionType.EnemyKill && 
                targetObjects.Count > 0 && pingPrefab != null)
            {
                DestroyAllPings();
                foreach (var target in targetObjects)
                {
                    GameObject ping = Instantiate(pingPrefab, target.transform);
                    ping.transform.localPosition = Vector3.up; // 적 머리 위에 위치
                    currentPings.Add(ping);
                }
            }
        }
    }

    private void FindTargetObjects(string targetTag)
    {
        targetObjects.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        targetObjects.AddRange(objects);
    }

    private void HideAllUI()
    {
        if (tutorialImagePanel != null)
        {
            tutorialImagePanel.SetActive(false);
        }

        foreach (var step in tutorialSteps)
        {
            if (step.tutorialObject != null)
            {
                step.tutorialObject.SetActive(false);
            }
        }
    }

    public void CompleteCurrentStep()
    {
        if (!isTutorialActive || currentStepIndex >= tutorialSteps.Count) return;

        var currentStep = tutorialSteps[currentStepIndex];
        currentStep.isCompleted = true;
        
        if (currentStep.tutorialObject != null)
        {
            currentStep.tutorialObject.SetActive(false);
        }

        // 이미지 패널 비활성화
        if (tutorialImagePanel != null)
        {
            tutorialImagePanel.SetActive(false);
        }

        // 이미지가 있는 경우 시간 재개
        if (!string.IsNullOrEmpty(currentStep.imagePath) && 
            currentStep.completionType == TutorialStep.CompletionType.ImageInteraction)
        {
            ResumeTime();
        }

        // 핑 제거
        DestroyAllPings();
        
        currentStepIndex++;
        ShowCurrentStep();
    }

    private void Update()
    {
        if (!isTutorialActive || currentStepIndex >= tutorialSteps.Count) return;

        var currentStep = tutorialSteps[currentStepIndex];
        
        switch (currentStep.completionType)
        {
            case TutorialStep.CompletionType.ImageInteraction:
                if (Input.GetKeyDown(KeyCode.F))  // F키로 이미지 상호작용
                {
                    CompleteCurrentStep();
                }
                break;

            case TutorialStep.CompletionType.PositionReach:
                if (playerTransform != null && currentStep.targetPosition != null)
                {
                    float distance = Vector3.Distance(playerTransform.position, currentStep.targetPosition.position);
                    if (distance <= currentStep.reachDistance)
                    {
                        CompleteCurrentStep();
                    }
                }
                break;

            case TutorialStep.CompletionType.EnemyKill:
            case TutorialStep.CompletionType.ObjectDestroy:
                // 리스트에서 파괴된 오브젝트들을 제거
                if (targetObjects[0].transform.gameObject.activeInHierarchy == false)
                {
                    CompleteCurrentStep();
                }
                    
                break;

            case TutorialStep.CompletionType.Custom:
                if (currentStep.targetFlow != null)
                {
                    // ToNextFlow의 콜라이더에 플레이어가 들어갔는지 확인
                    if (playerTransform != null)
                    {
                        BoxCollider2D flowCollider = currentStep.targetFlow.GetComponent<BoxCollider2D>();
                        if (flowCollider != null)
                        {
                            Vector3 closestPoint = flowCollider.ClosestPoint(playerTransform.position);
                            float distance = Vector3.Distance(playerTransform.position, closestPoint);
                            if (distance < 0.1f) // 플레이어가 콜라이더 안에 있는지 확인
                            {
                                CompleteCurrentStep();
                            }
                        }
                    }
                }
                else
                {
                    currentStep.customCompleteCheck?.Invoke();
                }
                break;
        }
    }

    public void SetCustomCompleteCheck(int stepIndex, Action completeCheck)
    {
        if (stepIndex >= 0 && stepIndex < tutorialSteps.Count)
        {
            tutorialSteps[stepIndex].customCompleteCheck = completeCheck;
        }
    }

    private void OnDestroy()
    {
        foreach (var step in tutorialSteps)
        {
            step.customCompleteCheck = null;
        }
        ResumeTime();
        DestroyAllPings();
    }
} 