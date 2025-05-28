using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup gameOverPanel;
    public Button titleBtn;
    public Button reTryBtn;
    public GameObject skillSlots;

    private bool isGameOver;
    
    private void Start()
    {
        titleBtn.onClick.AddListener(MoveTitle);
        reTryBtn.onClick.AddListener(ReTry);
    }

    public void ShowGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        skillSlots.SetActive(false);
        
        Time.timeScale = 0;
        
        gameOverPanel.alpha = 0f;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;

        gameOverPanel.gameObject.SetActive(true);

        StartCoroutine(FadeInGameOver());
    }

    IEnumerator FadeInGameOver()
    {
        float duration = 2f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            gameOverPanel.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        AudioManager.Instance.StopBGM();

        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;
        
        yield return new WaitForSecondsRealtime(0.1f);
    }

    public void MoveTitle()
    {
        SceneManager.LoadScene("TitleScene");
        Time.timeScale = 1;
    }

    public void ReTry()
    {
        DataManager.Instance.flowManager.GetLastSavedFlow();
        Time.timeScale = 1;
    }
}
