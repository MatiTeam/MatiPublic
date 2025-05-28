using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [Header("Audio Settings")] public Slider masterVolSlider;
    public TextMeshProUGUI masterVolText;
    [Space] public Slider bgmVolSlider;
    public TextMeshProUGUI bgmVolText;
    [Space] public Slider sfxVolSlider;
    public TextMeshProUGUI sfxVolText;

    AudioManager audioManager;

    float originalMaster;
    float originalBgm;
    float originalSfx;

    private void Awake()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        float defaultVolume = 0.5f;

        float masterVol = PlayerPrefs.HasKey("MasterVol") ? PlayerPrefs.GetFloat("MasterVol") : defaultVolume;
        float bgmVol = PlayerPrefs.HasKey("BgmVol") ? PlayerPrefs.GetFloat("BgmVol") : defaultVolume;
        float sfxVol = PlayerPrefs.HasKey("SfxVol") ? PlayerPrefs.GetFloat("SfxVol") : defaultVolume;

        masterVolSlider.value = masterVol;
        bgmVolSlider.value = bgmVol;
        sfxVolSlider.value = sfxVol;

        masterVolText.text = Mathf.RoundToInt(masterVol * 100).ToString();
        bgmVolText.text = Mathf.RoundToInt(bgmVol * 100).ToString();
        sfxVolText.text = Mathf.RoundToInt(sfxVol * 100).ToString();

        originalMaster = masterVol;
        originalBgm = bgmVol;
        originalSfx = sfxVol;
    }

    void Start()
    {
        masterVolSlider.onValueChanged.AddListener(OnMasterVolChanged);
        bgmVolSlider.onValueChanged.AddListener(OnBgmVolChanged);
        sfxVolSlider.onValueChanged.AddListener(OnSfxVolChanged);

        AudioManager.Instance.SetMasterVol(masterVolSlider.value);
        AudioManager.Instance.SetBGMVol(bgmVolSlider.value);
        AudioManager.Instance.SetSFXVol(sfxVolSlider.value);

        SceneManager.sceneLoaded += OnSceneLoaded;

        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScene":
                audioManager.PlayBGM(BGM.TitleSound);
                break;
            case "MainScene":
                audioManager.PlayBGM(BGM.PlaySceneSound);
                break;
            case "BossScene":
				audioManager.PlayBGM(BGM.BossSceneBGM);
				break;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region 볼륨 조절

    public void OnMasterVolChanged(float vol)
    {
        masterVolText.text = Mathf.RoundToInt(vol * 100).ToString();
        audioManager.SetMasterVol(vol);
    }

    public void OnBgmVolChanged(float vol)
    {
        bgmVolText.text = Mathf.RoundToInt(vol * 100).ToString();
        audioManager.SetBGMVol(vol);
    }

    public void OnSfxVolChanged(float vol)
    {
        sfxVolText.text = Mathf.RoundToInt(vol * 100).ToString();
        audioManager.SetSFXVol(vol);
    }

    #endregion

    #region 볼륨 저장

    public void SaveVolumSettings()
    {
        PlayerPrefs.SetFloat("MasterVol", masterVolSlider.value);
        PlayerPrefs.SetFloat("BgmVol", bgmVolSlider.value);
        PlayerPrefs.SetFloat("SfxVol", sfxVolSlider.value);
        PlayerPrefs.Save();

        originalMaster = masterVolSlider.value;
        originalBgm = bgmVolSlider.value;
        originalSfx = sfxVolSlider.value;
    }

    public void CancelVolumeSettings()
    {
        masterVolSlider.value = originalMaster;
        bgmVolSlider.value = originalBgm;
        sfxVolSlider.value = originalSfx;
        
        audioManager.SetMasterVol(originalMaster);
        audioManager.SetBGMVol(originalBgm);
        audioManager.SetSFXVol(originalSfx);
    }
#endregion
    
    // 플레이어가 클릭시 소리 발생
    public void PlayerClickSound()
    {
        AudioManager.Instance.PlaySFX("TouchSound");
    }
}
