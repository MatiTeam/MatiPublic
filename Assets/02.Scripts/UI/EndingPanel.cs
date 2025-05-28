using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPanel : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button backBtn;
    // Start is called before the first frame update
    void Start()
    {
        exitBtn.onClick.AddListener(() => {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        backBtn.onClick.AddListener(BackToTitle);
    }

    private void ExitGame()
    {
        Debug.Log("ExitGame");
        exitBtn.gameObject.SetActive(false);
        GameManager.Instance.uiManager.ExitGame();
    }

    private void BackToTitle()
    {
        Debug.Log("BackToTitle");
        GameManager.Instance.uiManager.CloseEndingPanel();
        GameManager.Instance.uiManager.BackToTitle();
    }
    
    
}
