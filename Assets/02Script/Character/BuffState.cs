using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffState 
{
    private CharacterBase character;
    private bool atkUp;
    private bool stun;
    private bool burn;
    private bool freeze;
    private bool rock;

    public void InitBuffState(CharacterBase character)
    {
        this.character = character;
        atkUp = false;
        stun = false;
        burn = false;
        freeze = false;
        rock = false;
    }

    public void InitDebuff()
    {
        stun = false;
        burn = false;
        freeze = false;
        rock = false;
    }
}
