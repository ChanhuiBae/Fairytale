using UnityEngine;

public class Weapon : MonoBehaviour 
{
    protected float ATK;
    protected float durability;
    protected bool enchant;

    protected float curATK;


    public float GetATK() // 정보 제공
    {
        return ATK;
    }
    public float GetInflictATK()
    {
        return curATK;
    }

    public float GetDurability()
    {
        return durability;
    }

    public void InitCurrATK()
    {
        curATK = 0f;
    }

    public float ShowEnchant() // 강화 시 변화한 공격력
    {
        return ATK * 1.2f;
    }

    public bool TryEnchant()
    {
        if (!enchant)
        {
            int i = UnityEngine.Random.Range(0, 100);
            if (i > 70)
            {
                enchant = true;
                ATK *= 1.2f;
            }
            return enchant;
        }
        return false;

    }
}
