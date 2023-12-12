using UnityEngine;

public enum Attribute
{
    None = 0,
    Fire = 1,
    Ice = 2,
    Rock = 3,
}

public class Weapon : MonoBehaviour 
{
    protected float ATK;
    protected float durability;
    protected bool enchant;
    protected float curATK;
    protected int attribute;
    protected ParticleSystem breakEffect;
    public float GetDurability()
    {
        return durability;
    }

    public void InitCurrATK()
    {
        curATK = 0f;
    }
}
