using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public Image hPBarFront;

    public void SetHPFillAmount(float rate)
    {
        hPBarFront.fillAmount = rate;
    }
}
