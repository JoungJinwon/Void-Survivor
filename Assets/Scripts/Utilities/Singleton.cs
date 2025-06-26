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
    /// </summary>
    protected void InitSingleton()
    {
        if (transform.parent != null || transform.root != null)
            DontDestroyOnLoad(transform.root.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }
}