using System.Collections;
using UnityEngine;

public class SpiderDemon : MonsterBase
{
    private int random;
    new public void InitMonster(TableEntity_Monster monster, Vector3 pos, SpawnManager spawnManager)
    {
        base.InitMonster(monster,  pos,  spawnManager);
        ai.SetIdle();
    }
    new public void Spawn()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        material.color = Color.white;
        ui.Spawn();
        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        yield return null;
        anim.LieDown();
        yield return YieldInstructionCache.WaitForSeconds(2);
        anim.StandUp();
        ai.Spawn();
    }

    new public void AttackRanged()
    {
        random = UnityEngine.Random.Range(0, 99);

        unit.state = State.Attack;
        unit.attackType = AttackType.Ranged;
        unit.rangedAttack = RangedAttack.ImmediatelyShoot;
        anim.BowAim();
        GetProjectile();
        anim.BowShoot();
        StartCoroutine(rangedAttackDelay());
    }

    new public void AttackOnehand()
    {
        unit.onehandAttack = OnehandAttack.Sting;
        anim.Sting();
        unit.onehand.Sting();
    }
}
