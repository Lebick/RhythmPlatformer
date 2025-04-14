using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{
    public struct PoolingInfo
    {
        public GameObject originalPrefab;
        public GameObject myObject;

        public PoolingInfo(GameObject originalPrefab, GameObject myObject)
        {
            this.originalPrefab = originalPrefab;
            this.myObject = myObject;
        }
    }

    private List<Transform> pullingParents = new();

    public void SetPooling(GameObject originalPrefab, GameObject myObject)
    {
        if (!IsExistParent(originalPrefab.name))
        {
            GameObject newParent = new GameObject(originalPrefab.name);
            newParent.transform.SetParent(transform);
            pullingParents.Add(newParent.transform);
        }

        myObject.transform.SetParent(GetMyParent(originalPrefab.name));
    }

    public GameObject GetPooling(GameObject originalPrefab)
    {
        if (IsExistParent(originalPrefab.name))
        {
            Transform parent = GetMyParent(originalPrefab.name);

            if (parent.childCount > 0)
            {
                if (parent.GetChild(0).TryGetComponent(out PoolingObject obj))
                {
                    obj.gameObject.SetActive(true);
                    obj.Initialize();
                    return obj.gameObject;
                }
            }
        }

        return Instantiate(originalPrefab);
    }

    private bool IsExistParent(string name)
    {
        for (int i = 0; i < pullingParents.Count; i++)
        {
            if (pullingParents[i].name == name)
                return true;
        }

        return false;
    }

    private Transform GetMyParent(string name)
    {
        for (int i = 0; i < pullingParents.Count; i++)
        {
            if (pullingParents[i].name == name)
                return pullingParents[i];
        }

        return transform;
    }
}