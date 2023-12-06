using UnityEngine;
using Redcode.Pools;

public class HitEffect : MonoBehaviour, IPoolObject
{

    [SerializeField]
    private string poolName;
    private ParticleSystem particle;
    private PoolManager pools;
    private bool doPlay;
    private float lifeTime;

    private void Awake()
    {
        if (!TryGetComponent<ParticleSystem>(out particle))
            Debug.Log("HitEffect - Awake - ParticleSystem");

        // todo: get poolname
    }

    public void InitHitEffect(GameObject pool)
    {
        if (!pool.TryGetComponent<PoolManager>(out pools))
            Debug.Log("HitEffect - Init - PoolManager");
        doPlay = false;
        lifeTime = Time.time + 1f;
    }
    public void PlayEffect()
    {
        particle.Play();
        doPlay = true;
    }

    private void Update()
    {
        if (doPlay && !particle.isPlaying)
        {
            doPlay = false;
            pools.TakeToPool<HitEffect>(poolName, this);
        }
    }

    public void OnCreatedInPool()
    {
        // throw new System.NotImplementedException();
    }

    public void OnGettingFromPool()
    {
        //throw new System.NotImplementedException();
    }

}
