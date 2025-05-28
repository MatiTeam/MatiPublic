using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset stageJson;
    public List<StageSpawnData> AllStageData { get; private set; }

    void Awake()
    {
        var parsed = JsonUtility.FromJson<StageSpawnDataList>(stageJson.text);
        AllStageData = parsed.datas;
    }

    public List<StageSpawnData> GetFlowData(string FlowID, string StageLvID)
    {
        return AllStageData.Where(d => d.FlowID == FlowID && d.StageLvID == StageLvID).ToList();
    }

    public string GetNextStageID(string FlowID, string StageLvID)
    {
        // 현재 FlowID에 해당하는 StageLvID 목록 (중복 제거 + 정렬)
        var stagesInFlow = AllStageData
            .Where(d => d.FlowID == FlowID)
            .Select(d => d.StageLvID)
            .Distinct()
            .OrderBy(id => id) // StageLvID 정렬 (필요 시)
            .ToList();

        // 현재 StageLvID의 인덱스 찾기
        int currentIndex = stagesInFlow.IndexOf(StageLvID);

        if (currentIndex == -1)
        {
            Debug.LogWarning($"현재 StageLvID '{StageLvID}' 가 FlowID '{FlowID}'에 없습니다.");
            return null;
        }

        // 다음 StageLvID가 존재하는지 확인
        int nextIndex = currentIndex + 1;

        if (nextIndex < stagesInFlow.Count)
        {
            return stagesInFlow[nextIndex];
        }
        else
        {
            // FlowID 내 StageLvID 끝 → null 반환
            Debug.Log("flow 끝남");
            return null;
        }
    }
}