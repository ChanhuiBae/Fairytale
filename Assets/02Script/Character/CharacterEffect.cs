using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Hit,
    Potion,
    Spawn,
    BreakWeapon,
}

public class CharacterEffect : MonoBehaviour
{
    private List<Effect> effects;
    private void Awake()
    {
        effects = new List<Effect>();
        Transform pointer = transform.Find("Effect");
        if(pointer != null ) 
        {
            for(int i = 0; i < pointer.childCount; i++) 
            { 
                effects.Add(pointer.GetChild(i).GetComponent<Effect>());
            }

        }
    }
    public void PlayEffect(int id)
    {
        effects[id].PlayEffect();
    }
    public void StopEffect(int id)
    {
        effects[id].StopEffect();
    }

    public void StopAllEffect()
    {
        for(int i= 0; i < effects.Count; i++)
        {
            effects[i].StopEffect();
        }
    }


}
