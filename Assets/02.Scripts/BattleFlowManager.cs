using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class BattleFlowManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform mati;

    [Header("UI")]
    [SerializeField] private FadeController fadeController;


    private StageManager stageManager;
    private PlatformGraphBuilder platformGraphBuilder;
    private GameObject map;

    public event Action OnFlowStarted;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "NextMapTestScene")
        {
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            DataManager.Instance.flowManager.MoveToNextFlow();
            Debug.Log("플로우 : " + DataManager.Instance.flowManager.GetCurrentFlowID());
        }
        stageManager = GetComponentInChildren<StageManager>();
        platformGraphBuilder = GetComponentInChildren<PlatformGraphBuilder>();
        StartCoroutine(StartNextStage());
    }

    public IEnumerator StartNextStage()
    {
        Debug.Log("StartNextStage 코루틴");
        yield return fadeController.FadeOut(1f);
        DataManager.Instance.flowManager.SaveCurrentFlowIndex();

        var rb = player.GetComponent<Rigidbody2D>();
        player.transform.parent = null;

        var rfMati = mati.GetComponent<Rigidbody2D>();
        mati.transform.parent = null;

        if (map != null)
        {
            Addressables.ReleaseInstance(map);
            Destroy(map);
        }

        Debug.Log("맵 아이디 : " + DataManager.Instance.flowManager.GetCurrentMapID());

        var handle = Addressables.InstantiateAsync(DataManager.Instance.flowManager.GetCurrentMapID(),
        Vector3.zero, Quaternion.identity, this.transform);
        yield return handle;

        // 어드레서블 맵 로드 비동기 처리

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("맵 로딩 콜백");
            map = handle.Result;

            var mapNodes = map.GetComponent<MapNodes>();
            if (mapNodes != null && mapNodes.PreMapNodes != null)
            {
                Debug.Log("프리 노드 " + mapNodes.PreMapNodes.Count);
                platformGraphBuilder.AddMapNode(mapNodes.PreMapNodes);
            }

            platformGraphBuilder.BuildGraphFromAllTilemaps();
            stageManager.StartNextFlow();

            rb.velocity = Vector2.zero;
            player.transform.position = new Vector2(0, 0);
            player.GetComponent<NewPlayerContorller>().targetX = 0;

            rfMati.velocity = rb.velocity;
            mati.transform.position = new Vector2(-0.5f, 0);
            mati.GetComponent<MatiPathFollower>().ClearPath();

            OnFlowStarted?.Invoke(); // 옵저버
        }
        else
        {
            Debug.LogError("맵 로딩 실패: " + handle.Status);
        }

        // 3초 후에 엔딩 체크
        if (DataManager.Instance.flowManager.GetCurrentFlowID() == "BA0106")
        {
            GameManager.Instance.uiManager.ShowEndingPanel();
        }

        yield return fadeController.FadeIn(1f);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "NextMapTestScene")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(TestStageLoad());
            }
        }
    }

    public void StartNextStageCoroutine() { StartCoroutine(StartNextStage()); }

    private IEnumerator TestStageLoad()
    {
        DataManager.Instance.flowManager.MoveToNextFlow();
        while (DataManager.Instance.flowManager.GetCurrentMapID() == null) { DataManager.Instance.flowManager.MoveToNextFlow(); }

        yield return fadeController.FadeOut(1f);

        var rb = player.GetComponent<Rigidbody2D>();
        player.transform.parent = null;

        var rfMati = mati.GetComponent<Rigidbody2D>();
        mati.transform.parent = null;


        if (map != null)
        {
            Addressables.ReleaseInstance(map);
            Destroy(map);
        }

        Debug.Log("맵 아이디 : " + DataManager.Instance.flowManager.GetCurrentMapID());

        var handle = Addressables.InstantiateAsync(DataManager.Instance.flowManager.GetCurrentMapID(),
        Vector3.zero, Quaternion.identity, this.transform);
        yield return handle;

        // 어드레서블 맵 로드 비동기 처리

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            map = handle.Result;

            var mapNodes = map.GetComponent<MapNodes>();
            if (mapNodes != null && mapNodes.PreMapNodes != null)
            {
                Debug.Log("프리 노드 " + mapNodes.PreMapNodes.Count);
                platformGraphBuilder.AddMapNode(mapNodes.PreMapNodes);
            }

            platformGraphBuilder.BuildGraphFromAllTilemaps();
            stageManager.StartNextFlow();

            rb.velocity = Vector2.zero;
            player.transform.position = new Vector2(0, 0);
            player.GetComponent<NewPlayerContorller>().targetX = 0;

            rfMati.velocity = rb.velocity;
            mati.transform.position = new Vector2(-0.5f, 0);
            mati.GetComponent<MatiPathFollower>().ClearPath();

            OnFlowStarted?.Invoke(); // 옵저버
        }
        else
        {
            Debug.LogError("맵 로딩 실패: " + handle.Status);
        }

        yield return fadeController.FadeIn(1f);
    }
}