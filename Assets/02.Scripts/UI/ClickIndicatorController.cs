using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClickIndicatorController : MonoBehaviour
{
    [SerializeField] private GameObject clickIndicatorPrefab;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private LayerMask obstacleLayer; // Obstacle 레이어 마스크

    private List<GameObject> indicatorPool;
    private float indicatorDuration = 0.5f; // 인디케이터가 표시되는 시간
    private float[] indicatorTimers;
    private Transform worldCanvas; // 월드 스페이스 캔버스를 위한 부모 오브젝트

    private void Awake()
    {
        // 월드 스페이스 캔버스를 위한 부모 오브젝트 생성
        worldCanvas = new GameObject("WorldCanvas").transform;
        worldCanvas.SetParent(null); // 씬의 루트에 배치

        InitializePool();
        indicatorTimers = new float[poolSize];
    }

    private void InitializePool()
    {
        indicatorPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject indicator = Instantiate(clickIndicatorPrefab, worldCanvas);
            indicator.SetActive(false);

            // Canvas 컴포넌트 설정
            Canvas canvas = indicator.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
            }

            indicatorPool.Add(indicator);
        }
    }

    private void Update()
    {
        // 모든 인디케이터의 타이머 업데이트
        for (int i = 0; i < poolSize; i++)
        {
            if (indicatorPool[i].activeSelf)
            {
                indicatorTimers[i] -= Time.deltaTime;
                if (indicatorTimers[i] <= 0)
                {
                    indicatorPool[i].SetActive(false);
                }
            }
        }
    }

    public void ShowClickIndicator(Vector2 position)
    {
        int nextIndex = FindNextAvailableIndex();
        GameObject indicator = indicatorPool[nextIndex];


        //float z = 0f - Camera.main.transform.position.z;
        //Vector3 screenPosition = new Vector3(position.x, position.y, 0f);
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        //worldPosition.z = 0f; // z값을 0으로 고정
        indicator.transform.position = position;
        //indicator.transform.position = position;
        indicator.SetActive(true);
        indicatorTimers[nextIndex] = indicatorDuration;

        Image image = indicator.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = 1f;

            // 레이어 체크하여 색상 설정
            Collider2D hit = Physics2D.OverlapPoint(position, obstacleLayer);
            if (hit != null)
            {
                // Obstacle 레이어일 경우 빨간색
                color = Color.red;
            }
            else
            {
                // 그 외의 경우 파란색
                color = Color.blue;
            }

            image.color = color;
        }
    }

    private int FindNextAvailableIndex()
    {
        // 비활성화된 인디케이터 찾기
        for (int i = 0; i < poolSize; i++)
        {
            if (!indicatorPool[i].activeSelf)
            {
                return i;
            }
        }

        // 모든 인디케이터가 활성화되어 있다면 가장 오래된 것 재사용
        int oldestIndex = 0;
        float oldestTime = indicatorTimers[0];

        for (int i = 1; i < poolSize; i++)
        {
            if (indicatorTimers[i] < oldestTime)
            {
                oldestTime = indicatorTimers[i];
                oldestIndex = i;
            }
        }

        return oldestIndex;
    }
}