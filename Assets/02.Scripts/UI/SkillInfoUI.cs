using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoUI : MonoBehaviour
{
    [Header("Panel Info")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillInfoText;
    public TextMeshProUGUI skillCoolTimeText;
    public GameObject skillInfoPanel;

    [Header("Cool Time")]
    public Image[] skillCoolTimeImage;
    
    [Header("SkillIDs")] 
    // 데이터를 배열로 생성
    public string[] skillIDs = new string[3];

    [Header("WSkill Info")]
    public Image wSkillImage;
    public Sprite[] wSkillSprites;
    public List<string> wSkillIDs = new List<string>();

    public NewPlayerContorller player;
    public Transform skillRange;
    public Transform qSkillRange;
    StackController stackController;
    
    private void Awake()
    {
        skillRange = player.transform.Find("SkillRange");
        qSkillRange = player.transform.Find("QSkillRange");
        stackController = FindObjectOfType<StackController>();
    }

    // Q E R 에 대한 작업
    public void RefreshSkillInfo(int slotIndex)
    {
        var skillData = DataManager.Instance.skillManager.skillDatas;

        var skill = skillData.GetSkillData(skillIDs[slotIndex]);

        if (skill != null)
        {
            skillNameText.text = $"{skill.Name}";
            skillInfoText.text = skill.Description;
            skillCoolTimeText.text = $"쿨타임 : {skill.CoolTime}초";

            skillInfoPanel.SetActive(true);
            if(slotIndex != 0)
            {
                SetSkillRange(skill);
            }
            else
            {
                SetQSkillRange();
            }

        }
    }
    
    // W에 대한 작업
    public void RefreshWSkillInfo(int slotIndex)
    {
        if (stackController == null)
            return;

        var skillData = DataManager.Instance.skillManager.skillDatas;

        int stackIndex = Mathf.Clamp(stackController.currentStackIndex, 0, wSkillIDs.Count - 1);
        
        var skill = skillData.GetSkillData(wSkillIDs[stackIndex]);

        if (skill != null)
        {
            skillNameText.text = $"{skill.Name}";
            skillInfoText.text = skill.Description;
            skillCoolTimeText.text = $"쿨타임 : {skill.CoolTime}초";
            skillInfoPanel.SetActive(!skillInfoPanel.activeSelf);
            SetWSkillRange(stackIndex, skill);
        }
    }

    //E,R스킬 범위 표시 메서드
    public void SetSkillRange(SkillData data)
    {
        skillRange.transform.localScale = new Vector3(data.EffectRange*2, data.EffectRange*2);
        skillRange.gameObject.SetActive(true);
    }
    //Q스킬 범위 표시 메서드
    public void SetQSkillRange()
    {
        Vector3 center;
        if(player.isLeft == true)
        {
            center = new Vector3(-2f, 1f, 0f);
            qSkillRange.transform.localPosition = center;
        }
        else
        {
            center = new Vector3(2f,1f, 0f);
            qSkillRange.transform.localPosition = center;
        }
        qSkillRange.gameObject.SetActive(true);
    }
    //W스킬 범위 표시 메서드
    public void SetWSkillRange(int stackCount,SkillData data)
    {
        if(stackCount == 2 || stackCount == 3)
        {
            skillRange.transform.localScale = new Vector3(data.EffectRange * 2, data.EffectRange * 2);
            skillRange.gameObject.SetActive(true);
        }
    }

    // 마우스 올리면 패널창 보이기
    public void ToggleSkillInfoPanel()
    {
        skillInfoPanel.SetActive(false);
        skillRange.gameObject.SetActive(false);
        qSkillRange.gameObject.SetActive(false);
    }


    // 스킬 사용 시 쿨타임 이미지 돌아감
    public IEnumerator SkillCoolTime(int skillIndex, float coolTime, string skillID)
    {
        Image skillImage = skillCoolTimeImage[skillIndex];
        skillImage.gameObject.SetActive(true);
        skillImage.fillAmount = 1f;
        
        while (skillImage.fillAmount > 0)
        {
            float remain = (coolTime - (Time.time - DataManager.Instance.skillManager.controller.lastUsedTime[skillID])) / coolTime;
            if(remain < 0) remain = 0;
            skillImage.fillAmount = remain;
            yield return null;
        }
    }

    public void TotalWStackSkill(int stackCount)
    {
        UpdateSkillImage(stackCount);
        UpdateSkillInfo(stackCount);
    }

    // 스택에 따른 스킬 이미지 변경
    void UpdateSkillImage(int stackCount)
    {
        if (stackCount >= 0 && stackCount < wSkillSprites.Length)
            wSkillImage.sprite = wSkillSprites[stackCount];
    }

    // 스택에 따른 스킬 설명 변경
    // 문제는 스위치 문을 돌다보니 스택이 순차적으로 다 쌓임
    void UpdateSkillInfo(int stackCount)
    {
        int index = Mathf.Clamp(stackCount, 0, wSkillIDs.Count - 1);
        
        var skill = DataManager.Instance.skillManager.skillDatas.GetSkillData(wSkillIDs[index]);

        if (skill != null)
        {
            skillNameText.text = skill.Name;
            skillInfoText.text = skill.Description;
            skillCoolTimeText.text = skill.CoolTime.ToString();
        }
    }

}