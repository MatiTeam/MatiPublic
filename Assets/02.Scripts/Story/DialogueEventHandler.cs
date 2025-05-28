using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대화 시스템의 이벤트를 처리하는 클래스입니다.
/// </summary>
public class DialogueEventHandler : MonoBehaviour
{
    private Coroutine currentTriggerEvent;
    private ObjectController objectController;

    private AudioSource bgm;
    private AudioSource sfx;

    [SerializeField] private Image DrawMatiandWeapon;        // 회상 시 보여줄 이미지
    [SerializeField] private GameObject InputIndicator;
    [SerializeField] private Image BackgroundImage;
    private float fadeDuration = 2.0f;


    /// <summary>
    /// 이벤트 실행 중인지 확인합니다.
    /// </summary>
    public bool IsEventRunning => currentTriggerEvent != null;


    public Action onCompleted;

    private void Start()
    {
        bgm = AudioManager.Instance.curBgm;
        sfx = AudioManager.Instance.curSfx;

        if (objectController == null)
        {
            objectController = gameObject.GetComponent<ObjectController>();
            if (objectController == null)
            {
                objectController = gameObject.AddComponent<ObjectController>();
            }
        }
    }

    /// <summary>
    /// 지정된 이벤트를 실행합니다.
    /// </summary>
    /// <param name="eventName">실행할 이벤트의 이름</param>
    public void TriggerEvent(string eventName)
    {
        // 이전에 실행 중인 이벤트가 있다면 중지
        if (currentTriggerEvent != null)
        {
            StopCoroutine(currentTriggerEvent);
            currentTriggerEvent = null;
        }

        if (string.IsNullOrEmpty(eventName)) // 이미 조건을 체크했지만, 일단 null 예외처리
        {
            Debug.LogWarning("Event name is empty or null");
            return;
        }

        if (eventName.StartsWith("ChangeBackGround_"))
        {
            string backgroundName = eventName.Substring("ChangeBackGround_".Length);
            currentTriggerEvent = StartCoroutine(ChangeBackground(backgroundName));
            return;
        }

        switch (eventName)
        {
            case "ShowEKey":
                currentTriggerEvent = StartCoroutine(ShowEKey());
                break;
            case "ShowPicture":
                currentTriggerEvent = StartCoroutine(ShowPicture());
                break;
            default:
                Debug.LogWarning($"Unknown event name: {eventName}");
                currentTriggerEvent = null;
                break;
        }
    }

    public void BGMEvent(string bgmName)
    {
        if (string.IsNullOrEmpty(bgmName))
        {
            Debug.LogWarning("BGM name is empty or null");
            return;
        }


        switch (bgmName)
        {
            case "CareFree":
            case "YSB_Entrance":
            case "MeetMati":
            case "CircusShow":
            case "MeetBoss":
            case "AfterBossStage":
                PlayBGM(bgmName);
                break;
        }
    }

    public void SFXEvent(string sfxName)
    {
        if (string.IsNullOrEmpty(sfxName))
        {
            Debug.LogWarning("SFX name is empty or null");
            return;
        }

        switch (sfxName)
        {
            case "Explosion":
                AudioManager.Instance.PlaySFX(sfxName, 0.6f);
                break;
            case "Cheer":
            case "YSB_Laughter":
            case "Umm":
                AudioManager.Instance.PlaySFX(sfxName);
                break;
        }
    }

    private void PlayBGM(string bgmName)
    {
        if (string.IsNullOrEmpty(bgmName))
        {
            Debug.LogWarning("BGM name is empty or null");
            return;
        }

        if (bgm.clip != null)
        {
            AudioManager.Instance.StopBGM();
        }

        bgm.clip = AudioManager.Instance.GetBGMClip((BGM)Enum.Parse(typeof(BGM), bgmName));
        bgm.loop = true;
        bgm.Play();
    }

    // private void PlaySFX(string sfxName, float volume = 1.0f, float pitch = 1.0f)
    // {
    //     if (string.IsNullOrEmpty(sfxName))
    //     {
    //         Debug.LogWarning("SFX name is empty or null");
    //         return;
    //     }
    //
    //     if (sfx.clip != null)
    //     {
    //         sfx.Stop();
    //     }
    //
    //     sfx.clip = AudioManager.Instance.GetSFXClip((SFX)Enum.Parse(typeof(SFX), sfxName));
    //     sfx.volume = volume;
    //     sfx.pitch = pitch;
    //     sfx.loop = false;
    //     sfx.Play();
    // }
    /// <summary>
    /// 카메라 흔들기 이벤트만 가능, 줌인/줌아웃 효과등이 필요하다면 구조 변경 필요
    /// </summary>
    public void CameraEvent()
    {
        Debug.Log("카메라 흔들기 이벤트 실행");
        GameManager.Instance.uiManager.ShakeCamera(1.0f, 1.0f);
    }

    #region 각 이벤트 코루틴 구현

    /// <summary>
    /// E키 이미지를 보여주는 코루틴입니다.
    /// </summary>
    private IEnumerator ShowEKey()
    {
        Debug.Log("E키 보여주기 시작");
        InputIndicator.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/E");
        objectController.ShowImage(InputIndicator);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E)); //나중에 텍스트가 다 나오면 ~이라고 수정. 지금 상태로는 E를 두번 눌러야 함
        objectController.HideImage(InputIndicator);
        currentTriggerEvent = null;
        DialogueView.isProcessingInput = false;
    }

    private IEnumerator ChangeBackground(string backgroundName)
    {
        if (BackgroundImage != null)
        {
            BackgroundImage.sprite = Resources.Load<Sprite>("Sprite/" + backgroundName);
            BackgroundImage.color = new Color(1, 1, 1, 200f / 255f);

            yield return null;
        }
        else
        {
            Debug.LogWarning("BackgroundImage가 설정되지 않았습니다.");
            yield return null;
        }
        currentTriggerEvent = null;
        DialogueView.isProcessingInput = false;
    }
    private IEnumerator ShowPicture()
    {
        Debug.Log("그림 보여주기 시작");

        if(BackgroundImage != null)
        {
            BackgroundImage.sprite = Resources.Load<Sprite>("Sprite/UnfamiliarSpace");
            BackgroundImage.color = new Color(1, 1, 1, 200f / 255f);
        }

        if (DrawMatiandWeapon != null)
        {
            Image image = DrawMatiandWeapon;

            // Radial 360 설정
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Radial360;
            image.fillOrigin = 0; // 원하는 방향으로 조정
            image.fillClockwise = true;
            image.fillAmount = 0f;

            image.color = new Color(1, 1, 1, 1); // 불투명하게

            objectController.ShowImage(image.gameObject);

            // 등장
            float duration = 1f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }

            image.fillAmount = 1f;

            // 인풋 표시
            InputIndicator.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Mouse0");
            objectController.ShowImage(InputIndicator);

            // 클릭 대기
            while (!Input.GetMouseButtonDown(0))
                yield return null;

            // 사라짐
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }

            image.fillAmount = 0f;

            objectController.HideImage(image.gameObject);
            objectController.HideImage(InputIndicator);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        currentTriggerEvent = null;
        DialogueView.isProcessingInput = false;
    }
    #endregion

    private void OnDisable()
    {
        // 모든 코루틴 정지
        if (currentTriggerEvent != null)
        {
            StopCoroutine(currentTriggerEvent);
            currentTriggerEvent = null;
        }

        if (bgm.clip != null)
        {
            AudioManager.Instance.StopBGM();
        }
    }
}