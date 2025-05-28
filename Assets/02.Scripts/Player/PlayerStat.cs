using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    private float maxHP = 4;
    private float curHP;
    private bool isDead = false;
    
    public float CurHP
    {
        get { return curHP; }
        set
        {
            curHP = Mathf.Clamp(value,0, maxHP);
        }
    }

    public List<GameObject> hpbars = new List<GameObject>();
    public GameObject hpbar;

    [SerializeField]private bool isShield;
    public bool IsShield
    {
        get
        {
            return isShield;
        }
        set
        {
            isShield = value;
            shieldSprite.gameObject.SetActive(isShield);
        }
    }
    public SpriteRenderer shieldSprite;

    [Header("Player Death")]
    public Animator animator;
    public GameOverUI gameOverUI;
    
    private void Start()
    {
        CurHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || IsShield) return;

        CurHP -= damage;
        Debug.Log("플레이어 현재 체력: " + CurHP);
        OnHPBar();
        SetHPBar(CurHP);


        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    public void Heal()
    {
        CurHP = maxHP;
        OnHPBar();
        SetHPBar(CurHP);

    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("IsDie");
        
        gameOverUI.ShowGameOver();
    }

    public void ActivateShield()
    {
        CancelInvoke("DeactivateShield"); // 기존 타이머 취소
        IsShield = true;
        Debug.Log("새로운 쉴드 생성");
        Invoke("DeactivateShield", DataManager.Instance.skillManager.skillDatas.skillDic["S0004"].BuffTime); // 5초 뒤에 자동 꺼짐
    }

    private void DeactivateShield()
    {
        IsShield = false;
        Debug.Log("쉴드 해제됨");
    }

    private void SetHPBar(float curHP)
    {
        if(curHP == 4)
        {
            foreach(var hp in hpbars)
            {
                hp.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 68, 255);
            }
            return;
        }
        for(int i = 0; i < maxHP-curHP; i++)
        {
            //Debug.Log(hpbars[i].name);
            hpbars[i].GetComponent<SpriteRenderer>().color = new Color32(42, 42, 42, 255);
            //hpbars[i].color = new Color32(42, 42, 42, 255);
        }
    }

    private void OnHPBar()
    {
        hpbar.SetActive(true);
        Invoke("OffHPBar", 1.5f);
    }

    private void OffHPBar()
    {
        hpbar.SetActive(false);
    }
}
