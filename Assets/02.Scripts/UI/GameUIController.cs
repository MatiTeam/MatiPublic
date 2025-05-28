using UnityEngine;

public class GameUIController : BaseUIController
{
    // 플레이 시간 계산
    private float playTimeSec;
    private int playTimeMin;

    // 플레이 타임 계산, 현재 사용은 안함
    public void SetPlayTime()
    {
        playTimeSec += Time.unscaledDeltaTime;

        if (playTimeSec >= 60f)
        {
            playTimeMin += 1;
            playTimeSec = 0;
        }
    }
}