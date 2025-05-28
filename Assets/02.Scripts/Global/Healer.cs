using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public void Heal(PlayerStat stat)
    {
        stat.Heal();
        Destroy(gameObject);
    }
}
