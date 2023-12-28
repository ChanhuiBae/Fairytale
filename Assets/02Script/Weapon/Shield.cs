using System.Collections;
using UnityEngine;

public class Shield : Weapon
{
    private BoxCollider col;

    public void InitShield(float ATK, float durability, bool enchant)
    {
        if (!transform.GetChild(0).TryGetComponent<BoxCollider>(out col))
            Debug.Log("Shield - InitShield - BoxCollider");

        this.durability = durability;
        this.enchant = enchant;
        if (enchant)
            this.ATK = ATK * 1.2f;
        else
            this.ATK = ATK;
        curATK = 0;

        col.enabled = false;
    }

    public void Defense()
    {
        curATK = ATK;
        col.enabled = true;
        StartCoroutine(SizeUp());
    }

    private IEnumerator SizeUp()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        col.center += Vector3.down;
        col.size *= 5;
    }

    public void StopDefense()
    {
        col.enabled = false;
        curATK = 0;
        col.size /= 5;
        col.center += Vector3.up;
    }

    public void WearOutDurability(float damage)
    {
        if (transform.root.CompareTag("Player"))
        {
            durability -= damage;
            GameManager.Inst.WarnShield(damage);
        }
    }

}
