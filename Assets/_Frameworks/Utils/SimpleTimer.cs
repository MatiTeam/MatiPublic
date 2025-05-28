using System;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    private static int timerId;

    public static SimpleTimer StartTimer(float time, Action onComplete, Action<float> onUpdate = null, bool autoStart = true)
    {
        timerId++;
        SimpleTimer timer = new GameObject().AddComponent<SimpleTimer>();
        timer.name = "SimpleTimer_" + timerId;

        timer.Init(time, onComplete, onUpdate);

        timer.DestroyOnComplete = true;

        if (autoStart)
            timer.StartTimer();

        return timer;
    }

    private float m_startTime;
    private float m_time;
    private float m_cTime;
    private Action m_completeHandler;
    private Action<float> m_onUpdate;

    public float CurrentTime
    {
        get { return m_cTime; }
    }

    public bool DestroyOnComplete { set; get; }
    public bool IsRunning { private set; get; }
    public bool Loop { set; get; }

    public void Init(float time, Action onComplete, Action<float> onUpdate)
    {
        m_time = time;
        m_completeHandler = onComplete;
        m_onUpdate = onUpdate;

        IsRunning = false;
    }

    public void StartTimer()
    {
        m_startTime = Time.time;
        m_cTime = m_time;
        IsRunning = true;
    }

    public void StopTimer()
    {
        m_cTime = m_time;
        IsRunning = false;

        if (Loop)
            StartTimer();
    }

    void Update()
    {
        if (!IsRunning) return;

        m_cTime = (m_startTime + m_time) - Time.time;

        if (m_cTime <= 0)
        {
            if (m_completeHandler != null)
            {
                if (m_onUpdate != null)
                    m_onUpdate(m_cTime);
                m_completeHandler();
                StopTimer();
            }

            if (DestroyOnComplete)
                Destroy(gameObject);

            return;
        }
        if (m_onUpdate != null)
            m_onUpdate(m_cTime);
    }
}