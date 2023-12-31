using Redcode.Pools;
using System;
using UnityEngine;

public class ProjectileInfo
{
    public ProjectilePool pool;
    public int attribute; // poolIndex
    public int uid;
    public string name;
    public Projectile projectile;
    public Vector3 projectileQuat;
    public Vector3 projectilePos;
    public float ATK;
    public Transform owner;
    public bool isUse;
}
public class Projectile : Weapon, IPoolObject
{
    private ProjectileInfo pInfo;
    private Rigidbody rig;
    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rig))
        {
            Debug.Log("Projectile - Init - Rigidbody");
        }
        curATK = 0f;
    }
    public void InitProjectile(ProjectileInfo info)
    {
        pInfo = info;
        pInfo.isUse = true;
        attribute = info.attribute;
        transform.position = pInfo.owner.position + pInfo.projectilePos + (pInfo.owner.forward * 0.3f) + (pInfo.owner.right * 0.15f);
        transform.rotation = pInfo.owner.rotation;
        rig.useGravity = false;
        rig.isKinematic = true;
        if (pInfo.owner.CompareTag("Enemy"))
        {
            transform.LeanScale(Vector3.zero, 0f);
        }
    }

    // Parabola
    public void ImmediatelyShoot(float power)
    {
        StopAllCoroutines();
        Vector3 target;
        curATK = pInfo.ATK + power;
        target = pInfo.owner.transform.forward * 20f + Vector3.up * 1.5f;
        rig.isKinematic = false;
        rig.useGravity = true;
        rig.velocity = target;
    }
    // Parabola
    public void AimShot(float power)
    {
        StopAllCoroutines();
        Vector3 target;
        curATK = pInfo.ATK + power;
        target = Camera.main.transform.forward * 30f + Vector3.up * 2f;
        rig.isKinematic = false;
        rig.useGravity = true;
        rig.velocity = target;
    }

    public void TargetShoot(float power, Vector3 angle)
    {
        StopAllCoroutines();
        if (pInfo.owner.CompareTag("Enemy"))
            transform.LeanScale(Vector3.one, 0.1f);
        Vector3 target;
        curATK = pInfo.ATK + power;
        target = angle * 20f + Vector3.up * 2f;
        rig.isKinematic = false;
        rig.useGravity = true;
        rig.velocity = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != pInfo.owner.tag) // 사용자에게 데미지 없음
        {
            if (other.TryGetComponent<IDamageControl>(out IDamageControl character) && pInfo.isUse)
            {
                pInfo.isUse = false;
                rig.velocity = Vector3.zero;
                if (character != null)
                {
                    character.TakeDamage(curATK);
                    int random = UnityEngine.Random.Range(0, 99);
                    if (random < 30)
                    {
                        switch (attribute)
                        {
                            case (int)Attribute.None:
                                character.TakeStun();
                                break;
                            case (int)Attribute.Fire:
                                character.TakeBurn(curATK / 5f);
                                break;
                            case (int)Attribute.Ice:
                                character.TakeFrozen();
                                break;
                            case (int)Attribute.Rock:
                                character.TakeRock(curATK / 6f);
                                break;
                        }
                    }
                }
                InitCurrATK();
                ReturnPool();
            }
            else if (pInfo.isUse && other.CompareTag("Weapon") && other.transform.parent.TryGetComponent<Shield>(out Shield weapon))
            {
                pInfo.isUse = false;
                rig.velocity = Vector3.zero;
                weapon.WearOutDurability(curATK);
                InitCurrATK();
                // todo: effect or 밀려나기
                ReturnPool();
            }
            else if(pInfo.isUse && other.CompareTag("Ground"))
            {
                pInfo.isUse = false;
                rig.velocity = Vector3.zero;
                InitCurrATK();
                ReturnPool();
            }
        }
    }

    private void ReturnPool()
    {
        if(this != null)
        {
            pInfo.pool.TakeProjectile(pInfo.name, this);
        }
    }

    public void TryTakePool()
    {
        if (pInfo.isUse)
        {
            pInfo.isUse = false;
            pInfo.pool.TakeProjectile(pInfo.name, this);
        }
    }

    void IPoolObject.OnCreatedInPool()
    {
        //throw new NotImplementedException();
    }

    void IPoolObject.OnGettingFromPool()
    {
        //throw new NotImplementedException();
    }
}
