using System.Collections;
using UnityEngine;

public class OneHandWeapon : Weapon
{
    private BoxCollider col;
    private float[] d_charge = { 0, 1, 2, 3, 4, 5, 6 };
    private float d_jump;
    private Vector3 size;
    public void InitOneHandWeapon(float ATK, float durability, bool enchant)
    {
        this.durability = durability;
        this.enchant = enchant;
        if (enchant)
            this.ATK = ATK * 1.2f;
        else
            this.ATK = ATK;
        curATK = 0f;
        d_jump = this.ATK / 10f;
        for (int i = 0; i < d_charge.Length; i++)
        {
            d_charge[i] = i * d_jump + this.ATK;
        }
        d_jump = this.ATK / 5f;
        d_jump = this.ATK + d_jump;

        if (!transform.GetChild(0).TryGetComponent<BoxCollider>(out col))
            Debug.Log("OnehandWeapon - Init - BoxCollider");
        else
            size = col.size;

        col.enabled = false;
    }

    public void Sting()
    {
        col.enabled = true;
        curATK = ATK;
        StopAllCoroutines();
        StartCoroutine(InitBoxCollider());
    }

    public void Swing(int charge)
    {
        col.enabled = true;
        curATK = d_charge[charge];
        StopAllCoroutines();
        StartCoroutine(InitBoxCollider());
    }

    public void JumpAttack()
    {
        col.enabled = true;
        curATK = d_jump;
        StopAllCoroutines();
        StartCoroutine(InitBoxCollider());
    }

    private IEnumerator InitBoxCollider()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        col.enabled = false;
    }

    public void WearOutDurability(float damage)
    {
        if (transform.root.CompareTag("Player"))
        {
            durability -= damage;
            GameManager.Inst.WarnOnehand(damage);
            if (durability <= 0)
            {
                col.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (curATK != 0) // 공격 상태가 아닐 때 부딪치면 데미지 없음
        {
            if (other.transform.root != transform.root) // 사용자에게 데미지 없음
            {
                if (other.TryGetComponent<MonsterBase>(out MonsterBase monster) && col.enabled)
                {
                    col.enabled = false;
                    monster.TakeDamage(curATK);
                    WearOutDurability(curATK);
                    InitCurrATK();
                }
                else if (other.TryGetComponent<PlayerController>(out PlayerController player) && col.enabled)
                {
                    col.enabled = false;
                    player.TakeDamage(curATK);
                    WearOutDurability(curATK);
                    InitCurrATK();
                }
                else if (col.enabled && other.CompareTag("Weapon") && other.transform.parent.TryGetComponent<Shield>(out Shield weapon))
                {
                    col.enabled = false;
                    weapon.WearOutDurability(curATK);
                    WearOutDurability(curATK);
                    InitCurrATK();
                    // todo: effect or 밀려나기
                }
            }
        }
    }

}
