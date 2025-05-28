using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackController : MonoBehaviour
{
    public List<GameObject> stackObjectList = new List<GameObject>();

    bool isTakeSkillAttack; // 스킬 맞은 것에 대한 bool
    bool isUseSkill; // 스킬을 사용에 대한 bool

    public int currentStackIndex; // 현재 스택 위치

    public SkillInfoUI skillInfoUI;
    
    // 지금 스택이 한번에 다 쌓임
    // 스킬 한번에 하나의 스택이 쌓이도록 수정
    // 스킬을 사용하여 적을 맞추게 될때
    public void UsingSkill()
    {
        Debug.Log("스택쌓임");

        currentStackIndex = Mathf.Min(currentStackIndex + 1, 3);
        Debug.Log(currentStackIndex);
        skillInfoUI.TotalWStackSkill(currentStackIndex);

        if (currentStackIndex > stackObjectList.Count) return;

        GameObject obj = stackObjectList[currentStackIndex-1];

        if (obj != null)
        {
            Image image = obj.GetComponent<Image>();

            if (image != null)
                image.color = Color.red;
        }


    }

    // W 스킬을 사용할때
    // 스택의 갯수에 따라 소모됨
    public void UsingStack()
    {

        for(int i = 0; i < stackObjectList.Count; i++)
        {
            GameObject obj = stackObjectList[i];
            Image image = obj.GetComponent<Image>();
            image.color = Color.white;
        }

        currentStackIndex = 0;
        
        skillInfoUI.TotalWStackSkill(currentStackIndex);
    }
}