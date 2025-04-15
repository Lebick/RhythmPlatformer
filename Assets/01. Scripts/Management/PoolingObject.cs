using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    protected GameObject originalPrefab;

    public virtual void Initialize(GameObject originalPrefab)
    {
        if (this.originalPrefab == null)
            this.originalPrefab = originalPrefab;
    }
}
