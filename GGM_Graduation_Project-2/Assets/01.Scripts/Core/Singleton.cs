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
                return null;        // 없어요 없어 그냥 이거 나중에 지워도 되고 .... 

                instance = GameObject.FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    instance = singleton.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
