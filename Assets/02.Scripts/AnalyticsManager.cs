using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();

            Debug.Log("Unity Services Initialized");

            // //(1) 퍼널 이벤트 전송 예시
            // SendFunnelStep("2");

            // //(2) 스킬 이벤트 전송 예시
            // analyticsSkill("1", "3");

            // //(3) 플레이더 등급 전송 예시
            // analyticsPlayerClass("5");

            // //(4) 플레이어 레벨 정보 전송 예시
            // analyticsPlayerUpgrade("2", "3");

        }
        catch (System.Exception e)
        {
            Debug.LogError("Unity Services failed to initialize: " + e.Message);
        }
    }

    //[1]퍼널
    public void SendFunnelStep(int stepNumber)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); //event
        funnelEvent["Funnel_Step_Num"] = stepNumber.ToString(); //parameter

        AnalyticsService.Instance.RecordEvent(funnelEvent); //custom event
        Debug.Log("퍼널 스텝 전달" + stepNumber);
    }

    //[2]스킬
    public void analyticsSkill(string WaveNumber, string SkillNumber)
    {
        var SkillEvent = new CustomEvent("Skill_Info"); //event
        SkillEvent["Wave_Number"] = WaveNumber; //parameter
        SkillEvent["Skill_Number"] = SkillNumber; //parameter

        AnalyticsService.Instance.RecordEvent(SkillEvent); //custom event
    }

    //[3]플레이어 등급
    public void analyticsPlayerClass(string PlayerClass)
    {
        var PlayerClassEvent = new CustomEvent("Player_Class_Info"); //event
        PlayerClassEvent["Player_Class"] = PlayerClass; //parameter

        AnalyticsService.Instance.RecordEvent(PlayerClassEvent); //custom event
    }

    //[4]플레이더 레벨 정보
    public void analyticsPlayerUpgrade(string PlayerType, string PlayerLevel)
    {
        var PlayerUpgradeEvent = new CustomEvent("Player_Upgrade_Info"); //event
        PlayerUpgradeEvent["Player_Type"] = PlayerType; //parameter
        PlayerUpgradeEvent["Player_Level"] = PlayerLevel; //parameter

        AnalyticsService.Instance.RecordEvent(PlayerUpgradeEvent); //custom event
    }
}
