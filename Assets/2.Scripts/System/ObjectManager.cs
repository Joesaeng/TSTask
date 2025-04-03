using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    #region Pool
    class Pool
    {
        public GameObject Origin { get; private set; }

        Stack<Poolable> poolStack = new();

        public void Init(GameObject origin, int count = 1)
        {
            Origin = origin;
            for (int i = 0; i < count; ++i)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Origin);
            go.name = Origin.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;
            poolable.transform.SetParent(null);

            poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent = null)
        {
            Poolable poolable;

            if (poolStack.Count > 0)
                poolable = poolStack.Pop();
            else
                poolable = Create();

            poolable.transform.SetParent(parent);
            poolable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            poolable.gameObject.SetActive(true);

            poolable.isUsing = true;

            return poolable;
        }
    }
    #endregion

    Dictionary<string,Pool> objectPool = new();

    private void CreatePool(GameObject origin, int count = 1)
    {
        Pool pool = new Pool();
        pool.Init(origin, count);

        objectPool.Add(origin.name, pool);
    }

    private void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (objectPool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        objectPool[name].Push(poolable);
    }

    private Poolable Pop(GameObject origin, Transform parent = null)
    {
        if (objectPool.ContainsKey(origin.name) == false)
        {
            CreatePool(origin);
        }

        return objectPool[origin.name].Pop(parent);
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Spawn(string path, Transform parent = null)
    {
        GameObject origin = Load<GameObject>($"Prefabs/{path}");
        return Spawn(origin, parent);
    }

    public GameObject Spawn(string path, Vector3 pos)
    {
        GameObject origin = Load<GameObject>($"Prefabs/{path}");
        return Spawn(origin, pos);
    }

    public GameObject Spawn(string path, Vector3 pos, Quaternion rot)
    {
        GameObject origin = Load<GameObject>($"Prefabs/{path}");
        return Spawn(origin, pos, rot);
    }

    public GameObject Spawn(GameObject origin, Transform parent = null)
    {
        if (origin.TryGetComponent<Poolable>(out _))
            return Pop(origin, parent).gameObject;

        return Object.Instantiate(origin, parent);
    }

    public GameObject Spawn(GameObject origin, Vector3 pos)
    {
        GameObject go;
        if (origin.TryGetComponent<Poolable>(out _))
        {
            go = Pop(origin).gameObject;
            go.transform.SetPositionAndRotation(pos, Quaternion.identity);
            return go;
        }
        go = Object.Instantiate(origin, pos, Quaternion.identity);

        return go;
    }

    public GameObject Spawn(GameObject origin, Vector3 pos, Quaternion rot)
    {
        GameObject go;
        if (origin.TryGetComponent<Poolable>(out _))
        {
            go = Pop(origin).gameObject;
            go.transform.SetPositionAndRotation(pos, rot);
            return go;
        }
        go = Object.Instantiate(origin, pos, rot);

        return go;
    }

    public void Kill(GameObject go)
    {
        if (go == null || !go.activeSelf)
            return;

        if (go.TryGetComponent<Poolable>(out var poolable))
        {
            Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
