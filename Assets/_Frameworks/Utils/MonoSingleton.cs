using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static bool isDestroyOnLoad = false;
    protected static bool isInitialized = false;
    protected static T instance;

    public static T Instance
    {
        get 
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if(instance == null)
                {
                    instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                }

                if (!isInitialized)
                {
                    isInitialized = true;
                }
            }
            return instance; 
        }
    }

    protected void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            if (instance != null)
            {
                if (!isInitialized)
                {
                    if (!isDestroyOnLoad) DontDestroyOnLoad(this);
                    instance.Init();
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Init()
    {

    }

    protected virtual void OnDestroy()
    {
        Dispose();    
    }

    protected virtual void Dispose()
    {
        instance = null;
    }
}
