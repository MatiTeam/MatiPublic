using System.Collections.Generic;
using UnityEngine;

public class SkillDataBase
{
    public TextAsset SkillJson;
    public Dictionary<string, SkillData> skillDic = new Dictionary<string, SkillData>();

    public void LoadJson_SkillData()
    {
        SkillJson = Resources.Load<TextAsset>($"SkillData/SkillTable");

        var wrapper  = JsonUtility.FromJson<SkillDataListWrapper>(SkillJson.text);
        foreach (var skill in wrapper.Skills)
        {
            skillDic.Add(skill.SkillID, skill);
        }
    }

    public SkillData GetSkillData(string id)
    {
        return skillDic.TryGetValue(id, out var skill) ? skill : null;
    }

}

[System.Serializable]
public class SkillDataListWrapper
{
    public List<SkillData> Skills;
}