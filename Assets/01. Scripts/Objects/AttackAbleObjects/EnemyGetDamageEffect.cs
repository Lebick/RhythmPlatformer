using UnityEngine;

public class EnemyGetDamageEffect : PoolingObject
{
    private void OnParticleSystemStopped()
    {
        PoolingManager.instance.SetPooling(originalPrefab, gameObject);
        gameObject.SetActive(false);
    }
}
