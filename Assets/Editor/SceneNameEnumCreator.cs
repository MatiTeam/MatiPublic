using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class SceneNameEnumCreator
{
    [InitializeOnLoadMethod]
    public static void RegisterSceneListChangedCallback()
    {
        EditorBuildSettings.sceneListChanged -= CreateEnum;
        EditorBuildSettings.sceneListChanged += CreateEnum;
    }
    [MenuItem("Tools/Create scene enum file")]
    public static void CreateEnum()
    {
        EditorBuildSettingsScene[] sceneList = EditorBuildSettings.scenes;

        if (sceneList.Length == 0) return;

        if (Directory.Exists("Assets/Scripts/AutoCreated/Enum/") == false)
        {
            Directory.CreateDirectory("Assets/Scripts/AutoCreated/Enum/");
        }
        List<string> sceneNameList = new List<string>();
        foreach (var scene in sceneList)
        {
            string[] paths = scene.path.Split('/');
            sceneNameList.Add(paths[paths.Length - 1].Split('.')[0]);
        }

        StreamWriter sw = new StreamWriter("Assets/Scripts/AutoCreated/Enum/ESceneName.cs");

        sw.WriteLine("// This enum is auto created by SceneNameEnumCreator.cs");
        sw.WriteLine();
        sw.WriteLine("public enum ESceneName");
        sw.WriteLine('{');
        foreach (var sceneName in sceneNameList)
        {
            sw.WriteLine($"\t{sceneName},");
        }
        sw.WriteLine("}");
        sw.Close();
        UnityEngine.Debug.Log("Successfully created scene enum file.");
    }
}