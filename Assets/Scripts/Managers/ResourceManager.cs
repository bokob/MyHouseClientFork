using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    // Prefabs의 특정 폴더에 있는 모든 것들 가져오기
    public T[] LoadAllInFolder<T>(string path) where T : Object
    {
        if (path.Contains("Prefabs/") == false)
            path = $"Prefabs/{path}";

        T[] list = Resources.LoadAll<T>(path);

        if (list.Length == 0)
            Debug.Log($"{path}에는 아무것도 없다.");

        return list;
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
