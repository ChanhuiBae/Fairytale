using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
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
        }
    }
}
