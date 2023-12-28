using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageControl
{
    public void TakeDamage(float damage);

    public void TakeFrozen();
    public void TakeStun();

    public void TakeBurn(float damage);

    public void TakeRock(float damage);
}
