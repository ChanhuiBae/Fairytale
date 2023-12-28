using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuffType
{
    None = -1,
    Burn = 0,
    Frozen = 1,
    Stun = 2,
    Rock = 3,
}
public class BuffState : MonoBehaviour
{
    private BuffType type;
    private List<Effect> effects;
    private CharacterAnimationController anim;
    private Material material;
    private void Awake()
    {
        if (!transform.parent.TryGetComponent<CharacterAnimationController>(out anim))
            Debug.Log("BuffState - Init - CharacterAnimationController");
        SkinnedMeshRenderer renderer = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null)
            Debug.Log("BuffState - Awake - SkinnedMeshRenderer");
        else
        {
            material = renderer.sharedMaterial;
            material.color = Color.white;
        }
        effects = new List<Effect>();
        if (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                effects.Add(transform.GetChild(i).GetComponent<Effect>());
            }

        }
    }
    public BuffType Buff
    {
        get => type;
    }

    public void SetBuff(BuffType value)
    {
        if(type != value)
        {
            if (type == BuffType.Rock)
            {
                material.color = Color.white;
                anim.StopRelax();
            }
            else if (type != BuffType.None)
            {
                if (type == BuffType.Stun)
                {
                    anim.StopStun();
                }
                effects[(int)type].StopEffect();
            }

            type = value;

            if (value == BuffType.None)
            {
                return;
            }
            else if (type == BuffType.Rock)
            {
                material.color = Color.gray;
                anim.Relax();
            }
            else
            {
                if (type == BuffType.Stun)
                {
                    anim.Stun();
                }
                effects[(int)type].PlayEffect();
            }
        }
    }
}
