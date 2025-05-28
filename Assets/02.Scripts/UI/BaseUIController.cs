using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseUIController : MonoBehaviour
{
    [Header("Game UI")]
    public Button restartBtn;
    public Button backTitleBtn;
    public Button optionBtn;
    public Button closeOptionBtn;

    [Header("Panel")]
    public GameObject optionPanel;
    public GameObject stopPanel;

    private bool isGameTime;
    private GameObject player;
    private GameObject canvas;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = gameObject;
    }

    void Start()
    {
        restartBtn.onClick.AddListener(RestartGame);
        backTitleBtn.onClick.AddListener(BackTitle);
        optionBtn.onClick.AddListener(OpenOptionPanel);
        closeOptionBtn.onClick.AddListener(CloseOptionPanel);
    }

    // 스탑 패널
    public void StopPanel()
    {
        if (!optionPanel.activeSelf)
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
                player.GetComponent<PlayerInput>().enabled = false;
            
            stopPanel.SetActive(true);
            Time.timeScale = 0;
            isGameTime = true;
        }
    }

    // 스탑 패널 비활성화
    public void RestartGame()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
            player.GetComponent<PlayerInput>().enabled = true;

        stopPanel.SetActive(false);
        Time.timeScale = 1;
        isGameTime = false;
    }

    // 타이틀 씬으로 이동
    public void BackTitle()
    {
        GameManager.Instance.uiManager.TransitionToScene("TitleScene");
        Time.timeScale = 1;
    }

    // 옵션창 활성화
    public void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
        stopPanel.SetActive(false);
    }

    // 옵션창 비활성화
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        stopPanel.SetActive(true);
    }
}
