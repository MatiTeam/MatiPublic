using UnityEngine;

public class StoryUIController : BaseUIController
{
    void Update()
    {
        StopPanelOpen();
    }
    
    public void StopPanelOpen()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !stopPanel.activeSelf)
            stopPanel.SetActive(true);
    }
}
