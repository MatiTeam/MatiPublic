using System.Collections;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    private Coroutine walkSoundCoroutine;

    public void PlayWalkSound()
    {
        if (walkSoundCoroutine == null)
        {
            AudioManager.Instance.PlaySFX("WalkSound");
            
            walkSoundCoroutine = StartCoroutine(PlayWalkSoundCoroutine());
        }
    }

    IEnumerator PlayWalkSoundCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        walkSoundCoroutine = null;
    }

    public void PlayerDieSound()
    {
        AudioManager.Instance.PlaySFX("Failed");
    }

    public void AttackSound()
    {
        AudioManager.Instance.PlaySFX("AttackSound");
    }

    public void PlayerHitSound()
    {
        AudioManager.Instance.PlaySFX("HitSound");
    }
    
    public void PlayerJumpSound()
    {
        AudioManager.Instance.PlaySFX("JumpSound");
    }

    public void WStack1Sound()
    {
        AudioManager.Instance.PlaySFX("WStack1Sound");
    }

    public void WStack3Sound()
    {
        AudioManager.Instance.PlaySFX("WStack3Sound");
    }

    public void SkillRSound()
    {
        AudioManager.Instance.PlaySFX("SkillR");
    }
}
