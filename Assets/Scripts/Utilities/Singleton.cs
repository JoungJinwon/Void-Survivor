using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // private Singleton instance
    private static T instance = null;

    // public Singleton property
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Singleton을 상속한 오브젝트를 DontDestroyOnLoad로 설정합니다.
    /// 중복된 인스턴스가 있을 경우 파괴합니다.
    /// </summary>
    protected void InitSingleton()
    {
        // 이미 인스턴스가 존재하고 현재 오브젝트가 그 인스턴스가 아닌 경우
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Duplicate {typeof(T).Name} detected. Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }

        // 현재 오브젝트를 인스턴스로 설정
        instance = this as T;

        if (transform.parent != null || transform.root != null)
            DontDestroyOnLoad(transform.root.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 인스턴스를 재설정합니다. 씬 재시작 시 사용됩니다.
    /// </summary>
    public static void ResetInstance()
    {
        instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}