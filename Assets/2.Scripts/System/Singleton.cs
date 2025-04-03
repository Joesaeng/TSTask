using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T ins;
    public static T Ins
    {
        get
        {
            if (ins == null)
            {
                ins = (T)FindAnyObjectByType(typeof(T));
                if (ins == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    ins = obj.GetComponent<T>();
                }
            }
            return ins;
        }
    }
    protected virtual void Awake()
    {
        if (ins != null)
        {
            Debug.Log(gameObject.name.ToString());
            Destroy(gameObject);
            return;
        }
        if (transform.parent != null && transform.root != null)
            DontDestroyOnLoad(this.transform.root.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }
}