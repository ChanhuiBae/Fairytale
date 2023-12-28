using System.Collections;
using UnityEngine;

public class SpiderDemon : MonsterBase
{
    private int random;
    private SpiderSceneManager sceneManager;
    new public void InitMonster(TableEntity_Monster monster, Vector3 pos, SpawnManager spawnManager)
    {
        if (GameObject.Find("SpiderSceneManager").TryGetComponent<SpiderSceneManager>(out sceneManager))
            Debug.Log("SpiderDemon - Init - SpiderSceneManager");
        base.InitMonster(monster,  pos,  spawnManager);
        ai.SetIdle();
    }
    new public void Spawn()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        ui.Spawn();

        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        yield return null;
        ai.Spawn();
        sceneManager.InitSpiderDemon(this);
    }

    new public void AttackRanged()
    {
        if (unit.currentHP < unit.maxHP / 2)
        {
            AimShoot();
        }
        else
        {
            ImmediatelyShoot();
        }
    }


    new public void AttackOnehand()
    {
        if(unit.currentHP < unit.maxHP / 2)
        {
            Swing(2);
        }
        else
        {
            Sting();
        }
    }

    private void OnDie()
    {
        anim.Die();
        unit.state = State.Die;
        gameObject.layer = LayerMask.NameToLayer("DieChar");
        sceneManager.StageClear();
        spawnManager.DropItem(transform.position + Vector3.up * 3);
        if (!usingRanged)
        {
            unit.onehand.InitCurrATK();
            unit.shield.InitCurrATK();
        }
        else
        {
            if (pInfo.projectile != null)
            {
                pInfo.projectile.TryTakePool();
            }
        }
        spawnManager.TakeMonsterPool(this);
    }
}
