using UnityEngine;
using Redcode.Pools;

public class ProjectilePool : MonoBehaviour
{
    private PoolManager poolManager;

    private void Awake()
    {
        if (!TryGetComponent<PoolManager>(out poolManager))
            Debug.Log("ProjectilePool - Awake - PoolManager");
    }

    public GameObject SpawnProjectile(int index)
    { 
        return poolManager.GetFromPool<Projectile>(index).gameObject;
    }

    //public GameObject SpawnHitEffect()
    //{
    //    return poolManager.GetFromPool<HitEffect>(1).gameObject;
    //}

    public void TakeProjectile(string name, Component projectile)
    {
        poolManager.TakeToPool<Projectile>(name, projectile);
    }
}
