using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : CharacterBase
{
    private int isDash;
    private EventTrigger onehandET;
    private EventTrigger.Entry oh_down;
    private EventTrigger.Entry oh_up;
    private GameObject aimPoint; 

    private Button hpPotionBtn;
    private Button staminaPotionBtn;

    private Stamina stamina;
    private Slider charge;
    private Vector3 target;

    private FollowCamera f_cam;

    private bool usingRanged;

    private bool isController;
    public bool ISCONTROLLER
    {
        set => isController = value;
        get => isController;
    }

    private Vector3 rangedMove = Vector3.zero;

    public void InitPlayerController()
    {
        
        isDash = -1;
        isController = false;
        usingRanged = false;
        InitCharBase(GameManager.Inst.PlayerInfo.maxHP, GameManager.Inst.PlayerInfo.DEF);
        unit.currentHP = GameManager.Inst.PlayerInfo.curHP;

        if (!Camera.main.TryGetComponent<FollowCamera>(out f_cam))
            Debug.Log("PlayerController - Init - FollowCamera");
        if (!TryGetComponent<Rigidbody>(out rig))
            Debug.Log("PlayerController - Init - Rigidbody");
        else
        {
            rig.useGravity = true;
            rig.isKinematic = false;
        }

        if (!GameObject.Find("Stamina").TryGetComponent<Stamina>(out stamina))
            Debug.Log("PlayerController - Init - Stamina");

        if (!GameObject.Find("Attack").TryGetComponent<EventTrigger>(out onehandET))
            Debug.Log("PlayerController - Init - EventTrigger");
        else
        {
            oh_down = new EventTrigger.Entry();
            oh_down.eventID = EventTriggerType.PointerDown;
            oh_down.callback.AddListener((data) => { AttackDown((PointerEventData)data); });
            onehandET.triggers.Add(oh_down);
            oh_up = new EventTrigger.Entry();
            oh_up.eventID = EventTriggerType.PointerUp;
            oh_up.callback.AddListener((data) => { AttackUp((PointerEventData)data); });
            onehandET.triggers.Add(oh_up);
        }

        if (!(aimPoint = GameObject.Find("AimPoint")))
            Debug.Log("PlayerController - Init - GameObject");
        else
        {
            aimPoint.LeanScale(Vector3.zero, 0);
        }

  
        stamina.InitStamina(GameManager.Inst.PlayerInfo.maxStamina);

        InitOneHand(GameManager.Inst.PlayerInfo.onehand, GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_onehand));
        InitShield(GameManager.Inst.PlayerInfo.shield, GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_shield));
        InitRanged(GameManager.Inst.PlayerInfo.ranged, GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_ranged));
        InitProjectileInfo(GameManager.Inst.PlayerInfo.projectile);
        InitHelmet(GameManager.Inst.PlayerInfo.helmet);

        if (!GameObject.Find("AttackCharge").TryGetComponent<Slider>(out charge))
            Debug.Log("PlayerController - Init - Slider");
        else
        {
            charge.gameObject.LeanScale(Vector3.zero, 0f);
        }

        if (!GameObject.Find("HpPotionBtn").TryGetComponent<Button>(out hpPotionBtn))
            Debug.Log("PlayerController - Init - Button");
        else
        {
            hpPotionBtn.onClick.AddListener(OnHpPotion); 
        }
        if (!GameObject.Find("StaminaPotionBtn").TryGetComponent<Button>(out staminaPotionBtn))
            Debug.Log("PlayerController - Init - Button");
        else
        {
            staminaPotionBtn.onClick.AddListener(OnStaminaPotion);
        }


        charge.maxValue = 6;
        charge.value = 0;

        unit.ranged.transform.parent = unit.back;
        unit.ranged.transform.localPosition = Vector3.zero;
        unit.ranged.transform.localRotation = Quaternion.identity;
    }

    private IEnumerator IsIdel()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        while (anim.IsIdle())
        {
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        } 
        while (!anim.IsIdle())
        {
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        isController = true;
        unit.state = State.Idle;
        unit.moveType = MoveType.None;
        unit.jumpType = JumpType.None;
        unit.attackType = AttackType.None;
    }

    private void SetJumpIdel()
    {
        unit.state = State.Idle;
        unit.jumpType = JumpType.None;
        unit.attackType = AttackType.None;
    }

    public void Victory()
    {
        StartCoroutine(PlayVictory());
    }

    private IEnumerator PlayVictory()
    {
        isController = false;
        yield return YieldInstructionCache.WaitForSeconds(1f);
        while (!anim.IsIdle())
        {
            yield return null;
        }
        transform.LeanRotateAround(Vector3.up, 180, 1.4f);
        anim.Victory();
        yield return YieldInstructionCache.WaitForSeconds(1.4f);
        anim.StopVictory();
        isController = true;
    }

    private void OnDie()
    {
        isController = false;
        GameManager.Inst.PlayerDieReset();
        charge.transform.LeanScale(Vector3.zero, 0f);
        rig.isKinematic = true;
        charge.transform.LeanScale(Vector3.zero, 0f);
        anim.Die();
        gameObject.layer = LayerMask.NameToLayer("DieChar");
    }

    public void Spawn()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        material.color = Color.white;
        InitHP();
        stamina.SetMaxStamina();
        transform.position = GameManager.Inst.GetSpawnPos(SceneManager.GetActiveScene().buildIndex, transform.position);
        anim.Spawn();
        rig.isKinematic = false;
        isController = true;
    }

    public void TakeDamage(float damage)
    {
        if(unit.moveType != MoveType.Dash)
        {
            isController = false;
            charge.transform.LeanScale(Vector3.zero, 0f);
            anim.StopMove();
            if(unit.rangedAttack == RangedAttack.Aim)
            {
                anim.StopBowAim();
                f_cam.SetPlay();
                pInfo.projectile.TryTakePool();
                unit.state = State.Idle;
                unit.attackType = AttackType.None;
                unit.rangedAttack = RangedAttack.None;
            }
            if (unit.currentHP > 0)
            {
                ChangeHP(CalculateDamage(damage));
                if (unit.currentHP <= 0)
                {
                    StopAllCoroutines();
                    stamina.StopAllCoroutines();
                    OnDie();
                }
                else
                {
                    anim.Hit();
                    StartCoroutine(IsIdel());
                }
            }
        }
    }

    private void InitHP()
    {
        unit.currentHP = unit.maxHP;
        GameManager.Inst.UpdateHP(unit.currentHP);
    }

    private void ChangeHP(float value)
    {
        unit.currentHP -= value;
        if (unit.currentHP > unit.maxHP)
            unit.currentHP = unit.maxHP;
        GameManager.Inst.UpdateHP(unit.currentHP);
    }

    private void LieDown()
    {
        if (unit.state == State.Idle)
        {
            LieDown();
            StartCoroutine(ChargeHpStamina());
        }
        else if (unit.state == State.Lay)
        {
            StopAllCoroutines();
            StandUp();
        }
    }

    private IEnumerator ChargeHpStamina()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            ChangeHP(-5);
            stamina.GetStamina(2);           
        }
    }

    private void OnHpPotion()
    {
        if (GameManager.Inst.CheckItem(30107))
        {
            if (unit.state == State.Idle)
            {
                DrinkHpPotion();
            }
            else if (unit.state == State.Move)
            {
                anim.StopMove();
                DrinkHpPotion();
            }
            else if (unit.state == State.Lay)
            {
                StandUp();
                StopAllCoroutines();
                DrinkHpPotion();
            }
        }
    }

    private void DrinkHpPotion()
    {
        if (!anim.IsDrinking())
        {
            if (unit.state == State.Idle || (unit.state == State.Move) || (unit.state == State.Lay))
            {
                anim.Drink();
                isController = false;
                if (unit.currentHP < unit.maxHP)
                    ChangeHP(-50);
                GameManager.Inst.INVENTORY.DeleteOneItem(30107);
                unit.state = State.Idle;
                unit.moveType = MoveType.None;
                StartCoroutine(IsIdel());
            }
        }
    }

    private void OnStaminaPotion()
    {
        if (GameManager.Inst.CheckItem(30108))
        {
            if (unit.state == State.Idle)
            {
                DrinkStaminaPotion();
            }
            else if (unit.state == State.Move)
            {
                anim.StopMove();
                DrinkStaminaPotion();
            }
        }
        else if (unit.state == State.Lay)
        {
            StandUp();
            StopAllCoroutines();
            DrinkStaminaPotion();

        }
    }

    private void DrinkStaminaPotion()
    {
        if (!anim.IsDrinking())
        {
            if (unit.state == State.Idle || (unit.state == State.Move) || (unit.state == State.Lay))
            {
                anim.Drink();
                isController = false;
                stamina.GetStamina(50);
                GameManager.Inst.INVENTORY.DeleteOneItem(30108);
                unit.state = State.Idle;
                unit.moveType = MoveType.None;
                StartCoroutine(IsIdel());
            }
        }
    }

    #region Move
    #region M_Input
    public void StopMove()
    {
        if (!isController)
        {
            anim.StopMove();
        }
    }
    private void RunDown()
    {
        if (unit.state == State.Idle || unit.moveType == MoveType.Walk)
        {
            unit.state = State.Move;
            unit.moveType = MoveType.Run;
            anim.Run();
        }
    }

    private void DashDown()
    {
        if (unit.state == State.Idle || unit.state == State.Move)
        {
            if (stamina.CheckDash())
            {
                unit.state = State.Move;
                unit.moveType = MoveType.Dash;
                anim.Dash();
                StartCoroutine(Dash());
            }
            else
            {
                isDash = -1;
                anim.StopMove();
                unit.moveType = MoveType.None;
                unit.state = State.Idle;
            }
        }
    }

    private void RollBackDown()
    {
        if (unit.state == State.Idle)
        {
            if (stamina.CheckRollBack())
            {
                unit.state = State.Move;
                unit.moveType = MoveType.RollBack;
                anim.RollBack();
                StartCoroutine(RollBack());
            }
        }
    }

    private IEnumerator Dash()
    {
        isController = false;
        Vector3 pos = Vector3.zero;
        float speed = (float)unit.moveType;
        switch (isDash)
        {
            case 0:
                pos = speed * Vector3.left;
                break;
            case 1:
                pos = speed * Vector3.back;
                break;
            case 2:
                pos = speed * Vector3.forward;
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
        isDash = -1;
        isController = true;
        anim.StopMove();
        unit.state = State.Idle;
        unit.moveType = MoveType.None;
    }

    private IEnumerator RollBack()
    {
        yield return null;
        for (int i = 0; i < 30; i++)
        {
            yield return null;
            transform.position += ((float)unit.moveType * Time.deltaTime * (Vector3.forward - transform.position));
        }
        unit.state = State.Idle;
        unit.jumpType = JumpType.None;
        unit.moveType = MoveType.None;
        anim.StopRollBack();
    }

    private IEnumerator InitIsDash()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        isDash = -1;
    }
    private void GetMoveInput()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();

        if(Input.GetKeyUp(KeyCode.A))
        {
            if(isDash == 0)
            {
                if(unit.state == State.Idle || unit.moveType == MoveType.Walk)
                {
                    DashDown();
                }
            }
            else
            {
                isDash = 0;
                StartCoroutine(InitIsDash());
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            if (isDash == 1)
            {
                if (unit.state == State.Idle || unit.moveType == MoveType.Walk)
                {
                    DashDown();
                }
            }
            else
            {
                isDash = 1;
                StartCoroutine(InitIsDash());
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            if (isDash == 2)
            {
                if (unit.state == State.Idle || unit.moveType == MoveType.Walk)
                {
                    DashDown();
                }
            }
            else
            {
                isDash = 2;
                StartCoroutine(InitIsDash());
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            if (isDash == 3)
            {
                if (unit.state == State.Idle || unit.moveType == MoveType.Walk)
                {
                    DashDown();
                }
            }
            else
            {
                isDash = 3;
                StartCoroutine(InitIsDash());
            }
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if(unit.state == State.Move && unit.moveType == MoveType.Run)
            {
                anim.StopMove();
                anim.Walk();
                unit.moveType = MoveType.Walk;
            }
            else
                RunDown();
        }

        if (unit.state == State.Idle && moveDir != Vector3.zero)
        {
            unit.state = State.Move;
            unit.moveType = MoveType.Walk;
            anim.Walk();
        }
        else if (moveDir != Vector3.zero && unit.state == State.Lay)
        {
            StandUp();
            unit.state = State.Idle;
        }
        else if (moveDir == Vector3.zero && (unit.moveType == MoveType.Walk || unit.moveType == MoveType.Run))
        {
            unit.state = State.Idle;
            unit.moveType = MoveType.None;
            anim.StopMove();
        }
    }
    #endregion

    private int moveCount = 0;
    public void HorizontalRotationMoving() // It is a player rotation function when he is aimming.
    {
        float xDir = moveDir.x * 0.1f;
        if (xDir < 0 && moveCount > -150)
        {
            moveCount -= 1;
            transform.Rotate(0, xDir, 0);
        }
        else if (xDir > 0 && moveCount < 150)
        {

            moveCount += 1;
            transform.Rotate(0, xDir, 0);

        }
    }
    #endregion

    #region WeaponCheck

    private bool CheckRanged()
    {
        return unit.ranged != null && unit.ranged.GetDurability() > 0;
    }

    private bool CheckOneHand()
    {
        return unit.onehand != null && unit.onehand.GetDurability() > 0;
    }

    private bool CheckShied()
    {
        return unit.shield != null && unit.shield.GetDurability() > 0;
    }

    private void ExchangeWeapon()
    {
        if (!usingRanged && CheckRanged())
        {
            usingRanged = true;
            charge.transform.LeanScale(Vector3.zero,0f);
            if (CheckOneHand())
            {
                unit.onehand.transform.parent = unit.back;
                unit.onehand.transform.localPosition = Vector3.zero;
                unit.onehand.transform.localRotation = Quaternion.identity;
            }
            if (CheckShied())
            {
                unit.shield.transform.parent = unit.back;
                unit.shield.transform.localPosition = Vector3.zero;
                unit.shield.transform.localRotation = Quaternion.identity;
            }
            if (CheckRanged())
            {
                unit.ranged.transform.parent = unit.lefthand;
                unit.ranged.transform.localPosition = Vector3.zero;
                unit.ranged.transform.localRotation = Quaternion.identity;
            }
        }
        else if (usingRanged && CheckOneHand())
        {
            usingRanged = false;
            if (CheckOneHand())
            {
                unit.onehand.transform.parent = unit.righthand;
                unit.onehand.transform.localPosition = Vector3.zero;
                unit.onehand.transform.localRotation = Quaternion.identity;
            }
            if (CheckShied())
            {
                unit.shield.transform.parent = unit.lefthand;
                unit.shield.transform.localPosition = Vector3.zero;
                unit.shield.transform.localRotation = Quaternion.identity;
            }
            if (CheckRanged())
            {
                unit.ranged.transform.parent = unit.back;
                unit.ranged.transform.localPosition = Vector3.zero;
                unit.ranged.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void GetExchangeInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExchangeWeapon();
        }
    }

    #endregion

    #region Attack
    #region A_Input

    private void GetAttackInput() 
    {
        if (unit.rangedAttack == RangedAttack.ImmediatelyShoot && anim.IsAimming() && moveDir != Vector3.zero)
        {
            unit.rangedAttack = RangedAttack.None;
            unit.state = State.Idle;
            unit.attackType = AttackType.None;
            anim.StopBowAim();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!usingRanged)
                DefendDown();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!usingRanged)
                DefendUp();
        }
    }

    #endregion
    private IEnumerator Charging()
    {
        while (charge.value < 6)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.2f);
            charge.value += 1f;
        }
    }

    public void OneHandDown()
    {
        if (CheckOneHand())
        {
            if(unit.state == State.Idle)
            {
                unit.state = State.Attack;
                unit.attackType = AttackType.Onehand;
                if (unit.onehandAttack != OnehandAttack.Charge)
                {
                    charge.gameObject.LeanScale(Vector3.one, 0f);
                    unit.onehandAttack = OnehandAttack.Charge;
                    charge.value = 0;
                    StartCoroutine(Charging());
                    // todo : charging effect

                }
            }
        }
        else
        {
            // warring
        }

    }

    public void OneHandUp()
    {
        if (unit.state == State.Attack
            && unit.attackType == AttackType.Onehand
            && unit.onehandAttack == OnehandAttack.Charge)
        {
            charge.gameObject.LeanScale(Vector3.zero, 0f);
            if (charge.value < 1)
            {
                unit.onehandAttack = OnehandAttack.Sting;
                unit.onehand.Sting();
                anim.Sting();
                charge.value = 0;
                StopAllCoroutines();    
                StartCoroutine(attackDelay());
            }
            else
            {
                unit.onehandAttack = OnehandAttack.Swing;
                unit.onehand.Swing((int)(charge.value));
                anim.Swing();
                charge.value = 0;
                StopAllCoroutines();
                StartCoroutine(attackDelay());
            }
        }
    }

    private IEnumerator attackDelay()
    {
        while (!anim.IsIdle())
        {
            yield return null;
        }
        unit.onehand.InitCurrATK();
        unit.state = State.Idle;
        unit.attackType = AttackType.None;
        unit.onehandAttack = OnehandAttack.None;
    }

    public void DefendDown()
    {
        if (CheckShied())
        {
            if (unit.state == State.Idle)
            {
                unit.state = State.Attack;
                unit.attackType = AttackType.Defense;
                unit.shield.Defense();
                anim.Defend();
                StartCoroutine(haveShield());

            }
        }
    }

    private IEnumerator haveShield()
    {
        while (unit.shield.GetDurability() > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        DefendUp();
    }
    public void DefendUp()
    {
        if (unit.state == State.Attack
        && unit.attackType == AttackType.Defense)
        {
            unit.state = State.Idle;
            unit.attackType = AttackType.None;
            unit.shield.StopDefense();
            anim.StopDefend();
        }
    }

     private void GetProjectile()
    {
        if (unit.ranged.GetDurability() > 0)
        {
            pInfo.isUse = true;
            GameObject obj= pInfo.pool.SpawnProjectile(pInfo.poolIndex);
            if (!obj.TryGetComponent<Projectile>(out pInfo.projectile))
                Debug.Log("PlayerController - BowDown - Projectile");
            else
                pInfo.projectile.InitProjectile(pInfo);
        }
        else
        {
            // todo : warring
        }
    }
    private IEnumerator rangedAttackDelay()
    {
        while (anim.IsAimming())
        {
            yield return null;
        }
        while (anim.IsShooting())
        {
            yield return null;
        }
        pInfo.projectile.ImmediatelyShoot(unit.ranged.ImmediatelyShoot());
        anim.StopBowAim();
        unit.ranged.InitCurrATK();
        unit.state = State.Idle;
        unit.attackType = AttackType.None;
        unit.rangedAttack = RangedAttack.None;
    }

    public void ImmediatelyShootDown()
    {
            if(unit.state == State.Idle)
            {
                unit.state = State.Attack;
                bool checkProjectile = false;
                if (pInfo.uid > 10501)
                {
                    checkProjectile = true;
                }
                else if (pInfo.uid == 10501 && GameManager.Inst.CheckItem(pInfo.uid))
                {
                    checkProjectile = true;
                }

                if (checkProjectile && CheckRanged())
                {
                    unit.attackType = AttackType.Ranged;
                    unit.rangedAttack = RangedAttack.ImmediatelyShoot;
                    anim.BowAim();
                    if (anim.IsIdle())
                    {
                        GetProjectile();
                        anim.BowShoot();
                        unit.ranged.WearOutDurability();
                        StartCoroutine(rangedAttackDelay());
                        if (pInfo.uid == 10501)
                            GameManager.Inst.INVENTORY.DeleteOneItem(pInfo.uid);
                    }
                    else
                    {
                        unit.state = State.Idle;
                        unit.attackType = AttackType.None;
                        unit.rangedAttack = RangedAttack.None;
                        anim.StopBowAim();
                        // todo: warring
                    }
                }
            }
            else
            {
                unit.state = State.Idle;
                anim.StopBowAim();
                // todo: warring
            }
    }

    public void AimDown()
    {
        if (unit.state == State.Idle)
        {
            unit.state = State.Attack;
            aimPoint.LeanScale(Vector3.one, 0);
            bool checkProjectile = false;
            if (pInfo.uid == 10501 && GameManager.Inst.CheckItem(pInfo.uid))
            {
                checkProjectile = true;
            }
            else if (pInfo.uid > 10501)
            {
                checkProjectile = true;
            }

            if (checkProjectile && CheckRanged())
            {
                unit.attackType = AttackType.Ranged;
                unit.rangedAttack = RangedAttack.Aim;
                f_cam.SetAim();
                anim.BowAim();
                if (anim.IsIdle())
                    GetProjectile();
            }
            else
            {
                unit.state = State.Idle;
                anim.StopBowAim();
                // todo : warring
            }
        }
    }

    public void AimUp()
    {
        if (unit.rangedAttack == RangedAttack.Aim)
        {
            unit.rangedAttack = RangedAttack.Shoot;
            anim.BowShoot();
            StartCoroutine(rangedAttackDelay());
            pInfo.projectile.AimShot(unit.ranged.Shoot());
            unit.ranged.WearOutDurability();
            if (pInfo.uid == 10501)
                GameManager.Inst.INVENTORY.DeleteOneItem(pInfo.uid);
        }
        aimPoint.LeanScale(Vector3.zero, 0);
        f_cam.SetPlay();
        unit.state = State.Idle;
        unit.attackType = AttackType.None;
        unit.rangedAttack = RangedAttack.None;
    }

    public void JumpAttackDown()
    {
        if (CheckOneHand() && !anim.IsJumpAttack())
        {
            if (unit.state == State.Idle && stamina.CheckJumpAttack())
            {
                unit.state = State.Attack;
                unit.jumpType = JumpType.Attack;
                unit.attackType = AttackType.Jump;
                unit.onehand.JumpAttack();
                anim.JumpAttack();
                Jump();
                SetJumpIdel();
            }
        }
        
    }

    private void AttackDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (usingRanged)
            {
                target = GameManager.Inst.GetTarget();
                if(target != Vector3.zero)
                {
                    transform.LookAt(target);
                }
                ImmediatelyShootDown();
            }
            else
            {
                target = GameManager.Inst.GetTarget();
                if (target != Vector3.zero)
                {
                    transform.LookAt(target);
                }
                OneHandDown();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (usingRanged)
            {
                if(unit.state == State.Idle && unit.moveType == MoveType.None)
                AimDown();
            }
            else
            {
                target = GameManager.Inst.GetTarget();
                if (target != Vector3.zero)
                {
                    transform.LookAt(target);
                }
                JumpAttackDown();
            }
        }
    }

    private void AttackUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!usingRanged)
            {
                target = GameManager.Inst.GetTarget();
                if (target != Vector3.zero)
                {
                    transform.LookAt(target);
                }
                OneHandUp();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (usingRanged)
            {
                AimUp();
            }
        }
    }
    #endregion

    #region Jump

    public void JumpDown()
    {
        if (unit.state == State.Idle || unit.state == State.Move)
        {
            if (stamina.CheckJump())
            if (stamina.CheckJump())
            {
                unit.state = State.Jump;
                unit.jumpType = JumpType.Jump;
                anim.Jump();
                Jump();
                SetJumpIdel();
            }
        }
    }

    #endregion

    private void ApplyMoveState()
    {
        if (unit.moveType == MoveType.Walk)
            Move();
        else if(unit.moveType == MoveType.Run)
        {
            if (stamina.CheckRun())
                Move();
            else
            {
                unit.state = State.Idle;
                unit.moveType = MoveType.None;
                anim.StopMove();
            }
        }
        else if (unit.attackType == AttackType.Ranged && unit.rangedAttack == RangedAttack.Aim)
        {
            HorizontalRotationMoving();
            rangedMove.y = moveDir.x;
            rangedMove *= 0.1f;
            f_cam.VerticalRotationCam(rangedMove);
            pInfo.projectile.InitProjectile(pInfo);
        }
    }

    private void Update()
    {
        if (isController)
        {
            GetExchangeInput();
            GetAttackInput();
            GetMoveInput();
            ApplyMoveState();
        }
        else // 행동불가
        {
            moveDir = Vector3.zero;
            unit.state = State.Idle;
            unit.moveType = MoveType.None;
            unit.jumpType = JumpType.None;
            unit.attackType = AttackType.None;
            unit.onehandAttack = OnehandAttack.None;
            unit.rangedAttack = RangedAttack.None;
        }
    }
}