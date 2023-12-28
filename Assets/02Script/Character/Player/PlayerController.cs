using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : CharacterBase, IDamageControl
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

    [SerializeField]
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
            charge.gameObject.SetActive(false);
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
        unit.attack = AttackType.None;
    }

    private void SetJumpIdel()
    {
        unit.state = State.Idle;
        unit.attack = AttackType.None;
    }

    private void OnDie()
    {
        isController = false;
        GameManager.Inst.PlayerDieReset();
        charge.gameObject.SetActive(false);
        rig.isKinematic = true;
        charge.gameObject.SetActive(false);
        unit.buff.SetBuff(BuffType.None);
        StopAllEffect();
        gameObject.layer = LayerMask.NameToLayer("DieChar");
        anim.Die();
    }

    public void Spawn()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        InitHP();
        stamina.SetMaxStamina();
        transform.position = GameManager.Inst.GetSpawnPos(SceneManager.GetActiveScene().buildIndex, transform.position);
        anim.Spawn();
        rig.isKinematic = false;
        isController = true;
    }

    public void TakeDamage(float damage)
    {
        if(unit.state != State.Dash)
        {
            isController = false;
            charge.value = 0;
            charge.gameObject.SetActive(false);
            anim.StopMove();
            anim.StopDefend();
            
            soundManager.PlaySFX(SFX_Type.SFX_Hit);
            if(unit.attack == AttackType.Aim)
            {
                anim.StopBowAim();
                f_cam.SetPlay();
                pInfo.projectile.TryTakePool();
                unit.state = State.Idle;
                unit.attack = AttackType.None;
            }
            if (unit.currentHP > 0)
            {
                ChangeHP(CalculateDamage(damage));
                if (unit.currentHP <= 0)
                {
                    StopAllCoroutines();
                    stamina.StopAllCoroutines();
                    StopTrail();
                    OnDie();
                }
                else
                {
                    if (unit.buff.Buff != BuffType.Rock && unit.buff.Buff != BuffType.Stun)
                    {
                        anim.Hit();
                        characterEffect.PlayEffect((int)EffectType.Hit);
                        StartCoroutine(IsIdel());
                    }
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
        StartCoroutine(Burnning(damage));
    }

    private IEnumerator Burnning(float damage)
    {
        for(int i = 0; i < 2;  i++)
        {
            TakeDamage(damage);
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        unit.buff.SetBuff(BuffType.None);
    }

    public void TakeStun()
    {
        isController = false; 
        unit.state = State.Idle;
        unit.attack = AttackType.None;
        unit.buff.SetBuff(BuffType.Stun);
        StartCoroutine(WaitStopStun());
    }

    private IEnumerator WaitStopStun()
    {
        yield return YieldInstructionCache.WaitForSeconds(2);
        unit.buff.SetBuff(BuffType.None);
        isController = true;
    }

    public void TakeRock(float damage)
    {
        isController = false;
        unit.state = State.Idle;
        unit.attack = AttackType.None;
        unit.buff.SetBuff(BuffType.Rock);
        StartCoroutine(Rocking(damage));
    }
    
    private IEnumerator Rocking(float damage)
    {
        for(int i = 0; i < 4; i++) 
        {
            TakeDamage(damage);
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
        }
        if (unit.currentHP > 0)
        {
            unit.buff.SetBuff(BuffType.None);
            isController = true;
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


    private void OnHpPotion()
    {
        if (GameManager.Inst.CheckItem(30107))
        {
            if (unit.state == State.Idle)
            {
                DrinkHpPotion();
            }
            else if (unit.state == State.Walk || unit.state == State.Run)
            {
                anim.StopMove();
                DrinkHpPotion();
            }
        }
    }

    private void DrinkHpPotion()
    {
        if (!anim.IsDrinking())
        {
            anim.Drink();
            soundManager.PlaySFX(SFX_Type.SFX_Drink);
            isController = false;
            if (unit.currentHP < unit.maxHP)
                ChangeHP(-unit.maxHP/4);
            GameManager.Inst.INVENTORY.DeleteItemAmount(30107, 1);
            unit.state = State.Idle;
            StartCoroutine(IsIdel());
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
            else if (unit.state == State.Walk || unit.state == State.Run)
            {
                anim.StopMove();
                DrinkStaminaPotion();
            }
        }
    }

    private void DrinkStaminaPotion()
    {
        if (!anim.IsDrinking())
        {
            anim.Drink();
            soundManager.PlaySFX(SFX_Type.SFX_Drink);
            isController = false;
            stamina.GetStamina(50);
            GameManager.Inst.INVENTORY.DeleteItemAmount(30108, 1);
            unit.state = State.Idle;
            StartCoroutine(IsIdel());
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
        if (unit.state == State.Idle || unit.state ==State.Walk)
        {
            unit.state= State.Run;
            anim.Run();
        }
    }

    private void DashDown()
    {
        if (unit.state == State.Walk && unit.attack != AttackType.Charge)
        {
            if (stamina.CheckDash())
            {
                unit.state = State.Dash;
                anim.Dash();
                StartCoroutine(Dash());
            }
            else
            {
                isDash = -1;
                anim.StopMove();
                unit.state = State.Idle;
            }
        }
    }

    private IEnumerator Dash()
    {
        isController = false;
        Vector3 pos = Vector3.zero;
        float speed = (float)State.Dash;
        trail.enabled = true;
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
        for (int i = 0; i < 6; i++)
        {
            transform.position += Time.deltaTime * pos;
            yield return null;
        }
        isDash = -1;
        trail.enabled = false;
        isController = true;
        anim.StopMove();
        unit.state = State.Idle;
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
                if(unit.state == State.Idle || unit.state == State.Walk)
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
                if (unit.state == State.Idle || unit.state == State.Walk)
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
                if (unit.state == State.Idle || unit.state == State.Walk)
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
                if (unit.state == State.Idle || unit.state == State.Walk)
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
            if(unit.state == State.Run)
            {
                anim.StopMove();
                anim.Walk();
                unit.state = State.Walk;
            }
            else
                RunDown();
        }

        if (unit.state == State.Idle && moveDir != Vector3.zero)
        {
            unit.state = State.Walk;
            anim.Walk();
        }
        else if (moveDir == Vector3.zero && (unit.state == State.Walk || unit.state == State.Run))
        {
            unit.state = State.Idle;
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
        if(unit.attack == AttackType.Aim)
        {
            return;
        }
        if (!usingRanged && CheckRanged())
        {
            usingRanged = true;
            charge.gameObject.SetActive(false);
            soundManager.PlaySFX(SFX_Type.SFX_ChangeWeapon);
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
            soundManager.PlaySFX(SFX_Type.SFX_ChangeWeapon);
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
            if(unit.state != State.Defense)
                ExchangeWeapon();
        }
    }

    #endregion

    #region Attack
    #region A_Input

    private void GetAttackInput() 
    {
        if (unit.attack == AttackType.ImmediatelyShoot && anim.IsAimming() && moveDir != Vector3.zero)
        {
            unit.state = State.Idle;
            unit.attack = AttackType.None;
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
                if (unit.attack != AttackType.Charge)
                {
                    charge.gameObject.LeanScale(new Vector3(4,4,1), 0f);
                    unit.attack = AttackType.Charge;
                    charge.gameObject.SetActive(true);
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
        if (unit.attack == AttackType.Charge)
        {
            charge.gameObject.SetActive(false);
            soundManager.PlaySFX(SFX_Type.SFX_OnehandAttack);
            if (charge.value < 1)
            {
                unit.attack = AttackType.Sting;
                unit.onehand.Sting();
                anim.Sting();
                StopAllCoroutines();    
                StartCoroutine(attackDelay());
            }
            else
            {
                unit.attack = AttackType.Swing;
                unit.onehand.Swing((int)(charge.value));
                anim.Swing();
                StopAllCoroutines();
                StartCoroutine(attackDelay());
            }

            charge.value = 0;
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
        unit.attack = AttackType.None;
    }

    public void DefendDown()
    {
        if (CheckShied())
        {
            if (unit.state == State.Idle)
            {
                unit.state = State.Defense;
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
            yield return null;
        }
        DefendUp();
    }
    public void DefendUp()
    {
        StopAllCoroutines();
        unit.state = State.Idle;
        unit.shield.StopDefense();
        anim.StopDefend();
    }

    private void GetProjectile()
    {
        if (unit.ranged.GetDurability() > 0)
        {
            pInfo.isUse = true;
            GameObject obj= pInfo.pool.SpawnProjectile(pInfo.attribute);
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
        unit.attack = AttackType.None;
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
                    unit.state = State.Attack;
                    unit.attack = AttackType.ImmediatelyShoot;
                    anim.BowAim();
                    if (anim.IsIdle())
                    {
                        GetProjectile();
                        anim.BowShoot();
                    soundManager.PlaySFX(SFX_Type.SFX_Ranged);
                        unit.ranged.WearOutDurability();
                        StartCoroutine(rangedAttackDelay());
                        if (pInfo.uid == 10501)
                            GameManager.Inst.INVENTORY.DeleteItemAmount(pInfo.uid, 1);
                    }
                    else
                    {
                    unit.state = State.Idle;
                        unit.attack = AttackType.None;
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
                aimPoint.LeanScale(Vector3.one, 0);
                unit.state = State.Attack;
                unit.attack = AttackType.Aim;
                f_cam.SetAim();
                anim.BowAim();
                if (anim.IsIdle())
                    GetProjectile();
            }
            else
            {
                unit.state = State.Idle;
                unit.attack = AttackType.None;
                anim.StopBowAim();
                // todo : warring
            }
        }
    }

    public void AimUp()
    {
        if (unit.attack == AttackType.Aim)
        {
            unit.attack = AttackType.Shoot;
            soundManager.PlaySFX(SFX_Type.SFX_Ranged);
            anim.BowShoot();
            StartCoroutine(rangedAttackDelay());
            pInfo.projectile.AimShot(unit.ranged.Shoot());
            unit.ranged.WearOutDurability();
            if (pInfo.uid == 10501)
                GameManager.Inst.INVENTORY.DeleteItemAmount(pInfo.uid, 1);
        }
        aimPoint.LeanScale(Vector3.zero, 0);
        f_cam.SetPlay();
        unit.state = State.Idle;
        unit.attack = AttackType.None;
    }

    public void JumpAttackDown()
    {
        if (CheckOneHand() && !anim.IsJumpAttack())
        {
            if (unit.state == State.Idle && stamina.CheckJumpAttack())
            {
                unit.state = State.Attack;
                unit.attack = AttackType.Jump;
                soundManager.PlaySFX(SFX_Type.SFX_OnehandAttack);
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
                if(unit.state == State.Idle)
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

    private void ApplyMoveState()
    {
        if (unit.attack == AttackType.Aim)
        {
            HorizontalRotationMoving();
            rangedMove.y = moveDir.x;
            rangedMove *= 0.1f;
            f_cam.VerticalRotationCam(rangedMove);
            pInfo.projectile.InitProjectile(pInfo);
        }
        else if (unit.state == State.Walk)
            Move();
        else if(unit.state == State.Run)
        {
            if (stamina.CheckRun())
                Move();
            else
            {
                unit.state = State.Idle;
                anim.StopMove();
            }
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
        else
        {
            unit.state = State.Idle;
            unit.attack = AttackType.None;
            if(rig != null)
            {
                rig.velocity = Vector3.zero;
            }
        }
    }
}