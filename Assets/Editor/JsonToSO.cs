
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class JsonToSO : MonoBehaviour
{
	[MenuItem("Tools/JsonToSO/CreateEnemySO")]
	static void EnemyDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<EnemyData>("Enemy.json", typeof(EnemySO));
	}
	[MenuItem("Tools/JsonToSO/CreateStageSO")]
	static void StageDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<StageData>("Stage.json", typeof(StageSO));
	}
	[MenuItem("Tools/JsonToSO/CreateSpeakerDataSO")]
	static void SpeakerDataDataInit()
	{
		DynamicMenuCreator.CreateMenusFromJson<SpeakerDataData>("SpeakerData.json", typeof(SpeakerDataSO));
	}

}
