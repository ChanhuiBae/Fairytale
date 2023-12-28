using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterBase : CharacterBase, IDamageControl
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
        ui.Spawn();
        ai.Spawn();
    }

    public void StopMove()
    {
        unit.state = State.Idle;
        anim.StopMove();
    }

    public void Walk()
    {
        unit.state = State.Walk;
        anim.Walk();
    }

    public void Run()
    {
        unit.state = State.Run;
        anim.Run();
    }

    protected void GetProjectile()
    {
        if (unit.ranged.GetDurability() > 0)
        {
            pInfo.isUse = true;
            GameObject obj = pInfo.pool.SpawnProjectile(pInfo.attribute);
            if (!obj.TryGetComponent<Projectile>(out pInfo.projectile))
                Debug.Log("PlayerController - BowDown - Projectile");
            else
            {
                pInfo.projectile.InitProjectile(pInfo);
            }
        }
    }

    protected IEnumerator rangedAttackDelay(float power)
    {
        while (!anim.IsShooting())
        {
            yield return null;
        }
        pInfo.projectile.TargetShoot(power, transform.forward);
        anim.StopBowAim();
        unit.ranged.InitCurrATK();
        unit.state = State.Idle;
        unit.attack = AttackType.None;
    }
    protected void ImmediatelyShoot()
    {
        unit.state = State.Attack;
        unit.attack = AttackType.ImmediatelyShoot;
        anim.BowAim();
        GetProjectile();
        anim.BowShoot();
        soundManager.PlaySFX(SFX_Type.SFX_Ranged);
        StartCoroutine(rangedAttackDelay(unit.ranged.ImmediatelyShoot()));
    }

    protected void AimShoot()
    {
        unit.state = State.Attack;
        unit.attack = AttackType.Aim;
        anim.BowAim();
        GetProjectile();
        anim.BowShoot();
        soundManager.PlaySFX(SFX_Type.SFX_Ranged);
        StartCoroutine(rangedAttackDelay(unit.ranged.Shoot()));
    }

    public void AttackRanged()
    {
        ImmediatelyShoot();
    }

    public void SkillRanged()
    {
        AimShoot();
    }

    protected void Sting()
    {
        unit.attack = AttackType.Sting;
        anim.Sting();
        unit.onehand.Sting();
    }

    protected void Swing(int charge)
    {
        unit.attack = AttackType.Swing;
        anim.Swing();
        unit.onehand.Swing(charge);
    }

    protected void JumpAttack()
    {
        if (!anim.IsJumpAttack())
        {
            anim.JumpAttack();
            Jump();
        }
    }

    public void AttackOnehand()
    {
        Sting();
        soundManager.PlaySFX(SFX_Type.SFX_OnehandAttack);
    }
    public void SkillOnehand()
    {
        JumpAttack();
        soundManager.PlaySFX(SFX_Type.SFX_OnehandAttack);
    }

    public void StopAttack()
    {
        unit.ranged.InitCurrATK();
        unit.onehand.InitCurrATK();
        unit.state = State.Idle;
        unit.attack= AttackType.None;
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
        unit.attack = AttackType.None;
    }

    public void StartDash()
    {
        unit.state = State.Dash;
        int direction = UnityEngine.Random.Range(0, 10);
        if(direction < 5)
            StartCoroutine(Dash(0));
        else
            StartCoroutine(Dash(3));
    }
    protected IEnumerator Dash(int isDash)
    {
        Vector3 pos = Vector3.zero;
        float speed = (float)State.Dash;
        switch (isDash)
        {
            case 0:
                pos = speed * Vector3.left;
                break;
            case 3:
                pos = speed * Vector3.right;
                break;
        }
        for (int i = 0; i < 6; i++)
        {
            transform.position += Time.deltaTime * pos;
            yield return null;
        }
        anim.StopMove();
        unit.state = State.Idle;
    }

    protected void OnDie()
    {
        unit.state = State.Die;
        gameObject.layer = LayerMask.NameToLayer("DieChar");
        StopAllEffect();
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
        StartCoroutine(ReturnMonster());
    }

    private IEnumerator ReturnMonster()
    {
        unit.buff.SetBuff(BuffType.None);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        anim.Die();
        yield return YieldInstructionCache.WaitForSeconds(2f);
        spawnManager.DropItem(transform.position + Vector3.up);
        spawnManager.TakeMonsterPool(this);
    }

    public void TakeDamage(float damage)
    {
        if (unit.currentHP > 0)
        {
            soundManager.PlaySFX(SFX_Type.SFX_Hit);
            StopTrail();
            if(unit.attack == AttackType.Aim)
            {
                anim.StopBowAim();
                pInfo.projectile.TryTakePool();
                unit.state = State.Idle;
                unit.attack = AttackType.None;
            }
            ChangeHP(CalculateDamage(damage));
            if (unit.currentHP < 1)
            {
                ui.Die();
                ai.Die();
                unit.buff.SetBuff(BuffType.None);
                OnDie();
            }
            else
            {
                if(unit.buff.Buff != BuffType.Rock && unit.buff.Buff != BuffType.Stun)
                {
                    anim.Hit();
                    characterEffect.PlayEffect((int)EffectType.Hit);
                }
            }
        }
    }

    public void TakeFrozen()
    {
        unit.buff.SetBuff(BuffType.Frozen);
        StartCoroutine(StopFrozen());
    }

    private IEnumerator StopFrozen()
    {
        yield return YieldInstructionCache.WaitForSeconds(5);
        unit.buff.SetBuff(BuffType.None);
    }

    public void TakeBurn(float damage)
    {
        unit.buff.SetBuff(BuffType.Burn);
        if (unit.currentHP > 0)
            StartCoroutine(Burnning(damage));
    }

    private IEnumerator Burnning(float damage)
    {
        for (int i = 0; i < 2; i++)
        {
            TakeDamage(damage);
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        unit.buff.SetBuff(BuffType.None);
    }

    public void TakeStun()
    {
        unit.buff.SetBuff(BuffType.Stun);
        ai.TakeDebuff(2);
        if(unit.currentHP > 0)
            StartCoroutine(WaitStopStun());
    }

    private IEnumerator WaitStopStun()
    {
        for(int i = 0; i < 4; i++)
        {
            if (unit.currentHP > 0)
            {
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
            }
            else
                break;
        }
        unit.buff.SetBuff(BuffType.None);
    }

    public void TakeRock(float damage)
    {
        unit.buff.SetBuff(BuffType.Rock);
        ai.TakeDebuff(2);
        if (unit.currentHP > 0)
            StartCoroutine(Rocking(damage));
    }

    private IEnumerator Rocking(float damage)
    {
        for (int i = 0; i < 4; i++)
        {
            if (unit.currentHP > 0)
            {
                TakeDamage(damage);
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
            }
            else
                break;
        }
        unit.buff.SetBuff(BuffType.None);
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
