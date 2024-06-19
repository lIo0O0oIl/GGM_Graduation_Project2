using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour       // 그냥 싱글턴
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                return null;    
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(instance);
            instance = this as T;
        }
    }
}
