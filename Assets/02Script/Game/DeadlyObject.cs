using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DeadlyObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(999);
        }
        else if (other.TryGetComponent<MonsterBase>(out MonsterBase monster))
        {
            monster.TakeDamage(999);
        }
    }
}
