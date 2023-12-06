using System;
using System.Collections;
using UnityEngine;

public class MonsterBase : CharacterBase
{
    protected EnemyUI ui;
    protected MonsterAI ai;
    protected SpawnManager spawnManager;
    protected bool isInit;
    protected bool usingRanged;

    public void InitMonster(TableEntity_Monster monster, Vector3 pos, SpawnManager spawnManager)
    {
        if (!gameObject.TryGetComponent<EnemyUI>(out ui))
            Debug.Log("MonsterBase - Awake - EnemyUI");
        if (!gameObject.TryGetComponent<MonsterAI>(out ai))
            Debug.Log("MonsterBase - Awake - MonsterAI");

        this.InitCharBase(monster.maxHP, monster.DEF);
        ui.InitEnemyUI(unit.maxHP);

        transform.position = pos;
        this.spawnManager = spawnManager;
        switch (monster.monsterType)
        {
            case 1:
                try
                {
                    usingRanged = false;
                    TableEntity_Weapon weapon;
                    GameManager.Inst.GetWeaponData(monster.onehand, out weapon);
                    InitOneHand(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.shield, out weapon);
                    InitShield(weapon, false);
                    unit.lefthand.transform.GetChild(1).gameObject.SetActive(false);
                    ai.InitAI( true, true, false);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("MonsterBase - Init - use onehand shield");
                }
                break;
            case 2:
                try
                {
                    usingRanged = true;
                    TableEntity_Weapon weapon;
                    GameManager.Inst.GetWeaponData(monster.ranged, out weapon);
                    InitRanged(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.projectile, out weapon);
                    InitProjectileInfo(weapon);
                    unit.righthand.transform.GetChild(0).gameObject.SetActive(false);
                    unit.lefthand.transform.GetChild(0).gameObject.SetActive(false);

                    ai.InitAI(false, false, true);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("MonsterBase - Init - use ranged");
                }
                break;
            case 3:
                try
                {
                    usingRanged = true;
                    TableEntity_Weapon weapon;
                    GameManager.Inst.GetWeaponData(monster.onehand, out weapon);
                    InitOneHand(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.ranged, out weapon);
                    InitRanged(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.projectile, out weapon);
                    InitProjectileInfo(weapon);
                    unit.lefthand.transform.GetChild(0).gameObject.SetActive(false);

                    ai.InitAI(true, false, true);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("MonsterBase - Init - use Onehand ranged");
                }
                break;
            case 4:
                try
                {
                    usingRanged = true;
                    TableEntity_Weapon weapon;
                    GameManager.Inst.GetWeaponData(monster.onehand, out weapon);
                    InitOneHand(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.shield, out weapon);
                    InitShield(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.ranged, out weapon);
                    InitRanged(weapon, false);
                    GameManager.Inst.GetWeaponData(monster.projectile, out weapon);
                    InitProjectileInfo(weapon);

                    ai.InitAI(true, false, true);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("MonsterBase - Init - use 3 Weapon");
                }
                break;
        }
    }

    public void Spawn()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        material.color = Color.white;
        ui.Spawn();
        ai.Spawn();
    }

    public void StopMove()
    {
        unit.state = State.Idle;
        unit.moveType = MoveType.None;
        anim.StopMove();
    }

    public void Walk()
    {
        unit.state = State.Move;
        unit.moveType = MoveType.Walk;
        anim.Walk();
    }

    public void Run()
    {
        unit.state = State.Move;
        unit.moveType = MoveType.Run;
        anim.Run();
    }

    protected void GetProjectile()
    {
        if (unit.ranged.GetDurability() > 0)
        {
            pInfo.isUse = true;
            GameObject obj = pInfo.pool.SpawnProjectile(pInfo.poolIndex);
            if (!obj.TryGetComponent<Projectile>(out pInfo.projectile))
                Debug.Log("PlayerController - BowDown - Projectile");
            else
            {
                pInfo.projectile.InitProjectile(pInfo);
            }
        }
    }

    protected IEnumerator rangedAttackDelay()
    {
        while (!anim.IsShooting())
        {
            yield return null;
        }
        pInfo.projectile.TargetShoot(unit.ranged.ImmediatelyShoot(), transform.forward);
        anim.StopBowAim();
        unit.ranged.InitCurrATK();
        unit.state = State.Idle;
        unit.attackType = AttackType.None;
        unit.rangedAttack = RangedAttack.None;
    }

    public void AttackRanged()
    {
        unit.state = State.Attack;
        unit.attackType = AttackType.Ranged;
        unit.rangedAttack = RangedAttack.ImmediatelyShoot;
        anim.BowAim();
        GetProjectile();
        anim.BowShoot();
        StartCoroutine(rangedAttackDelay());
    }
    public void AttackOnehand()
    {
        unit.onehandAttack = OnehandAttack.Sting;
        anim.Sting();
        unit.onehand.Sting();
    }

    public void StopAttack()
    {
        unit.ranged.InitCurrATK();
        unit.onehand.InitCurrATK();
        unit.state = State.Idle;
        unit.attackType = AttackType.None;
        unit.rangedAttack = RangedAttack.None;
        unit.onehandAttack = OnehandAttack.None;
    }

    public void Defense()
    {
        unit.shield.Defense();
        anim.Defend();
    }

    public void StopDefense()
    {
        anim.StopDefend();
        unit.shield.StopDefense();
        unit.attackType = AttackType.None;
    }

    public void StartRollBack()
    {
        unit.state = State.Move;
        unit.moveType= MoveType.RollBack;
        StartCoroutine(RollBack());
    }
    protected IEnumerator RollBack()
    {
        yield return null;
        for (int i = 0; i < 10; i++)
        {
            yield return null;
            transform.position += ((float)unit.moveType * Time.deltaTime * (transform.GetChild(0).position - transform.position));
        }
        unit.state = State.Idle;
        unit.jumpType = JumpType.None;
        unit.moveType = MoveType.None;
        anim.StopRollBack();
    }
    public void StartDash()
    {
        unit.state = State.Move;
        unit.moveType = MoveType.Dash;
        int direction = UnityEngine.Random.Range(0, 10);
        if(direction < 5)
            StartCoroutine(Dash(0));
        else
            StartCoroutine(Dash(3));
    }
    protected IEnumerator Dash(int isDash)
    {
        Vector3 pos = Vector3.zero;
        float speed = (float)unit.moveType;
        switch (isDash)
        {
            case 0:
                pos = speed * Vector3.left;
                break;
            case 3:
                pos = speed * Vector3.right;
                break;
        }
        for (int i = 0; i < 10; i++)
        {
            transform.position += Time.deltaTime * pos;
            yield return null;
        }
        anim.StopMove();
        unit.state = State.Idle;
        unit.moveType = MoveType.None;
    }

    protected IEnumerator OnDie()
    {
        anim.Die();
        unit.state = State.Die;
        gameObject.layer = LayerMask.NameToLayer("DieChar");
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
        spawnManager.DropItem(transform.position + Vector3.up * 3);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        spawnManager.TakeMonsterPool(this);
    }

    public void TakeDamage(float damage)
    {
        if (unit.currentHP > 0)
        {
            ChangeHP(CalculateDamage(damage));
            if (unit.currentHP < 1)
            {
                StopAllCoroutines();
                ui.Die();
                ai.Die();
                StartCoroutine(OnDie());
            }
            else
            {
                anim.Hit();
            }
        }
    }

    protected void ChangeHP(float value)
    {
        unit.currentHP -= value;
        ui.ChangeHP(value);
    }

    public void ResetHP()
    {
        unit.currentHP = unit.maxHP;
        ui.ResetHP();
    }
}
