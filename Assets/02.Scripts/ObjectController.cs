using UnityEngine;
using UnityEngine.UI;
public class ObjectController : MonoBehaviour
{
    [Header("Main")]
    private GameObject player;
    private Animator playerAnimator;

    public Camera mainCamera;
    private Vector3 originCameraPos;
    private float originCameraSize;
    
    [Header("Map")]
    private GameObject currentMap;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originCameraPos = mainCamera.transform.position;
            originCameraSize = mainCamera.orthographicSize;
        }

        //player = gameObject.CompareTag("Player") ? gameObject : null;
        //playerAnimator = player.GetComponent<Animator>();
    }
    // Tutorial_01 끝날 때
    // Main Camera에서
    // Sub Camera로 전환
    public void CameraPastChange(GameObject obj)
    {
        if (mainCamera == null) return;
        
        // 회상 맵 위치로 카메라 이동
        Vector3 recollectionPosition = new Vector3(-50, 0, -10);
        mainCamera.transform.position = recollectionPosition;
        
        // 필요에 따라 카메라 줌 조정
        mainCamera.orthographicSize = 5.0f;
    
        
    }

    // Tutorial_02 끝날 때
    // Sub Camera에서
    // Main Camera로 전환
    public void CameraTodayChange(GameObject obj)
    {
        if (mainCamera == null) return;
        
        // 원래 카메라 위치와 줌으로 복원
        mainCamera.transform.position = originCameraPos;
        mainCamera.orthographicSize = originCameraSize;
    }

    //특정 위치에 오브젝트 생성
    public GameObject SpawnObjectAt(GameObject prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }

    // 특정 위치로 플레이어 이동
    public void MovePlayerTo(Vector3 position)
    {
        if (player == null) return;
        player.transform.position = position;
    }
    
    // 플레이어 애니메이션 제어
    public void PlayPlayerAnimation(string animationTrigger)
    {
        if (playerAnimator == null) return;
        playerAnimator.SetTrigger(animationTrigger);
    }

    public void ChangeMap(GameObject newMap)
    {
        if(currentMap != null)
        {
            currentMap.SetActive(false);
        }

        newMap.SetActive(true);
        currentMap = newMap;
    }

    public void ShowImage(GameObject image)
    {
        image.SetActive(true);
    }

    public void HideImage(GameObject image)
    {
        image.SetActive(false);
    }

    public void ChangeImage(GameObject image, Sprite sprite)
    {
        image.GetComponent<Image>().sprite = sprite;
    }
}