using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private GameObject prefab;
    private int poolSize = 20;
    private Transform poolParent;

    public void Initialize(GameObject prefabToUse, int initialSize)
    {
        prefab = prefabToUse;
        poolSize = initialSize;
        poolParent = transform;

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void CreateNewObject()
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, poolParent);
        obj.SetActive(false);
        obj.name = prefab.name + "_" + availableObjects.Count;
        availableObjects.Enqueue(obj);
    }

    public GameObject GetObject()
    {
        if (availableObjects.Count == 0)
        {
            CreateNewObject();
        }

        if (availableObjects.Count > 0)
        {
            GameObject obj = availableObjects.Dequeue();
            return obj;
        }
        else
        {
            Debug.LogError("Object pool is empty and failed to create new object!");
            return null;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null || !obj.activeInHierarchy)
        {
            return;
        }

        try
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            availableObjects.Enqueue(obj);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error returning object to pool: " + e.Message);
        }
    }
}
