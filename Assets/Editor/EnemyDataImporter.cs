using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EnemyDataImporter : EditorWindow
{
    private TextAsset jsonFile;

    [MenuItem("Tools/Import Enemy JSON to SO")]
    public static void ShowWindow()
    {
        GetWindow<EnemyDataImporter>("Enemy JSON Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enemy Data JSON to ScriptableObject", EditorStyles.boldLabel);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

        if (GUILayout.Button("Import"))
        {
            if (jsonFile != null)
            {
                CreateSOFromJSON(jsonFile.text);
            }
            else
            {
                Debug.LogError("JSON 파일을 넣어주세요.");
            }
        }
    }

    private void CreateSOFromJSON(string json)
    {
        // datas를 감싸는 구조체 정의
        EnemyDataWrapper wrapper = JsonUtility.FromJson<EnemyDataWrapper>(json);

        // SO 생성
        EnemyDataTableSO asset = ScriptableObject.CreateInstance<EnemyDataTableSO>();
        asset.datas = wrapper.datas;

        string path = EditorUtility.SaveFilePanelInProject("Save EnemyDataTableSO", "EnemyDataTableSO", "asset", "Save SO");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            Debug.Log("적 데이터 테이블 SO 생성 완료!");
        }
    }

    [System.Serializable]
    public class EnemyDataWrapper
    {
        public List<EnemyData> datas;
    }
}
