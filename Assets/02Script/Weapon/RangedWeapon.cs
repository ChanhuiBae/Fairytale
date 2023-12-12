using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    private void Awake()
    {
        if (!transform.Find("Ranged/Break").TryGetComponent<ParticleSystem>(out breakEffect))
            Debug.Log("RangedWeeapon - Init - ParticleSystem");
    }
    public void InitRangedWeapon(float ATK, float durability, bool enchant)
    {
        this.durability = durability;
        this.enchant = enchant;
        if (enchant)
            this.ATK = ATK * 1.2f;
        else
            this.ATK = ATK;
        gameObject.SetActive(true);
    }

    public float ImmediatelyShoot()
    {
        return curATK = ATK;
    }
    public float Shoot()
    {
        return curATK = ATK * 1.5f;
    }

    public void WearOutDurability()
    {
        if (transform.root.CompareTag("Player"))
        {
            durability -= ATK;
            GameManager.Inst.WarnRanged(ATK);
            if (durability < 0)
            {
                StartCoroutine(WaitDestroy());
            }
        }
    }

    private IEnumerator WaitDestroy()
    {
        breakEffect.Play();
        yield return YieldInstructionCache.WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
