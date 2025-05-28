using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PixelCameraController : MonoBehaviour
{
    private PixelPerfectCamera ppc;

    [SerializeField] private BattleFlowManager battleFlowManager;

    void Awake()
    {
        ppc = GetComponent<PixelPerfectCamera>();
    }

    void OnEnable()
    {
        if (battleFlowManager != null)
        {
            battleFlowManager.OnFlowStarted += HandleFlowStarted;
        }
    }

    void OnDisable()
    {
        if (battleFlowManager != null)
        {
            battleFlowManager.OnFlowStarted -= HandleFlowStarted;
        }
    }

    private void HandleFlowStarted()
    {
        string currentFlow = DataManager.Instance.flowManager.GetCurrentFlowID();

        if (currentFlow == "BA0101")
            ppc.enabled = true;
        else
            ppc.enabled = false;
    }
}
