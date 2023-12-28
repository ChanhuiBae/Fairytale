using System.Collections;
using UnityEngine;

public enum State
{
    Idle = 0,
    Walk = 5,
    Run = 8,
    Dash = 40,
    Attack = 5,
    Defense = 5,
    Die = 0,
}

public enum AttackType
{
    None,
    Sting,
    Charge,
    Swing,
    Jump,
    ImmediatelyShoot,
    Aim,
    Shoot,
}

public class CharacterUnit
{
    public float currentHP;
    public float maxHP;
    public float DEF;
    public State state;
    public AttackType attack;
    public BuffState buff;
    public Transform back;
    public Transform lefthand;
    public Transform righthand;
    public Transform helmet;
    public OneHandWeapon onehand;
    public Shield shield;
    public RangedWeapon ranged;
}

public class CharacterBase : MonoBehaviour
{
    protected CharacterAnimationController anim;
    protected Rigidbody rig;
    protected CharacterUnit unit = new CharacterUnit();
    protected ProjectileInfo pInfo = new ProjectileInfo();
    protected SoundManager soundManager;
    protected Vector3 moveDir = Vector3.zero;

    protected TrailRenderer trail;
    protected CharacterEffect characterEffect;

    public void InitCharBase(float maxhp, float DEF)
    {
        if(!GameObject.Find("SoundManager").TryGetComponent<SoundManager>(out soundManager))
            Debug.Log("CharacterBase - Init - SoundManager");
        if (!TryGetComponent<CharacterAnimationController>(out anim))
            Debug.Log("CharacterBase - Init - CharacterAnimationController");

        if (!TryGetComponent<Rigidbody>(out rig))
            Debug.Log("CharacterBase - Init - Rigidbody");

        if (!transform.Find("Trail").TryGetComponent<TrailRenderer>(out trail))
            Debug.Log("CharacterBase - Init - TrailRender");
        else
        {
            trail.enabled = false;
        }
        if (!TryGetComponent<CharacterEffect>(out characterEffect))
            Debug.Log("CharacterBase - Init - CharacterEffect");

        anim.InitAinmController();

        unit.state = State.Idle;
        unit.attack = AttackType.None;

        if(!transform.Find("Buff").TryGetComponent<BuffState>(out unit.buff))
            Debug.Log("CharacterBase -Init - BuffState");
        else
        {
            unit.buff.SetBuff(BuffType.None);
        }

        if (!transform.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigNeck/RigHead/Dummy Prop Head/Helmet").TryGetComponent<Transform>(out unit.helmet))
            Debug.Log("CharacterBase -Init - Transform");

        if (!transform.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/Dummy Prop Weapon Back").TryGetComponent<Transform>(out unit.back))
            Debug.Log("CharacterBase -Init - Transform");

        if (!transform.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigLArm1/RigLArm2/RigLArmPalm/Dummy Prop Left").TryGetComponent<Transform>(out unit.lefthand))
            Debug.Log("CharacterBase -Init - Transform");

        if (!transform.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigRArm1/RigRArm2/RigRArmPalm/Dummy Prop Right").TryGetComponent<Transform>(out unit.righthand))
            Debug.Log("CharacterBase -Init - Transform");

        Transform weaponPos = unit.righthand.transform.Find("OneHand");
        if (weaponPos == null)
            weaponPos = unit.back.transform.Find("OneHand");
        if (!weaponPos.TryGetComponent<OneHandWeapon>(out unit.onehand))
            Debug.Log("CharacterBaser - Init - OneHand");

        if (!unit.lefthand.transform.Find("Shield").TryGetComponent<Shield>(out unit.shield))
            Debug.Log("CharacterBase - Init - Shield");

        weaponPos = unit.lefthand.transform.Find("RangedWeapon");
        if (weaponPos == null)
            weaponPos = unit.back.transform.Find("RangedWeapon");
        if (!weaponPos.TryGetComponent<RangedWeapon>(out unit.ranged))
            Debug.Log("CharacterBase - Init - RangedWeapon");

        if (!GameObject.Find("ProjectileManager").TryGetComponent<ProjectilePool>(out pInfo.pool))
            Debug.Log("CharacterBase - Init - PoolManager");

        pInfo.projectileQuat = new Vector3(0f, 180f, 0f);
        pInfo.projectilePos = new Vector3(0, 0.6f, 0);

        unit.maxHP = maxhp;
        unit.currentHP = unit.maxHP;
        unit.DEF = DEF;
    }

    public void InitOneHand(TableEntity_Weapon onehand, bool enchant)
    {
        unit.onehand.InitOneHandWeapon(onehand.ATK, onehand.durability, enchant, onehand.attribute);
        if (onehand.ATK == 0)
        {
            characterEffect.PlayEffect((int)EffectType.BreakWeapon);
            unit.onehand.gameObject.SetActive(false);
        }
        else
        {
            GameObject item = Resources.Load<GameObject>(onehand.resources);
            unit.onehand.gameObject.SetActive(true);
            unit.onehand.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
            unit.onehand.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            unit.onehand.transform.GetChild(0).GetComponent<BoxCollider>().center = item.GetComponent<BoxCollider>().center;
            unit.onehand.transform.GetChild(0).GetComponent<BoxCollider>().size = item.GetComponent<BoxCollider>().size;
            if (enchant)
            {
                unit.onehand.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                ParticleSystem ps = unit.onehand.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = item.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                unit.onehand.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }

        }
    }

    public void InitShield(TableEntity_Weapon shield, bool enchant)
    {
        unit.shield.InitShield(shield.ATK, shield.durability, enchant);
        if (shield.ATK == 0)
        {
            characterEffect.PlayEffect((int)EffectType.BreakWeapon);
            unit.shield.gameObject.SetActive(false);
        }
        else
        {
            GameObject item = Resources.Load<GameObject>(shield.resources);
            unit.shield.gameObject.SetActive(true);
            unit.shield.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
            unit.shield.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            unit.shield.transform.GetChild(0).GetComponent<BoxCollider>().center = item.GetComponent<BoxCollider>().center;
            unit.shield.transform.GetChild(0).GetComponent<BoxCollider>().size = item.GetComponent<BoxCollider>().size;
            if (enchant)
            {
                unit.shield.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                ParticleSystem ps = unit.shield.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = item.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                unit.shield.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void InitRanged(TableEntity_Weapon ranged, bool enchant)
    {
        unit.ranged.InitRangedWeapon(ranged.ATK, ranged.durability, enchant);
        if (ranged.ATK == 0)
        {
            characterEffect.PlayEffect((int)EffectType.BreakWeapon);
            unit.ranged.gameObject.SetActive(false);
            unit.back.parent.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            GameObject item = Resources.Load<GameObject>(ranged.resources);
            unit.ranged.gameObject.SetActive(true);
            if (gameObject.tag == "Player")
            {
                if (ranged.type == 3)
                    unit.back.parent.GetChild(0).gameObject.SetActive(true);
                else
                    unit.back.parent.GetChild(0).gameObject.SetActive(false);
            }
            unit.ranged.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
            unit.ranged.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            if (enchant)
            {
                unit.ranged.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                ParticleSystem ps = unit.ranged.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = item.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                unit.ranged.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void InitProjectileInfo(TableEntity_Weapon projectile)
    {
        if (projectile.ATK == 0)
        {
            pInfo.attribute = -1;
            pInfo.uid = -1;
            pInfo.name = "";
            pInfo.ATK = 0;
        }
        else
        {
            switch (projectile.uid)
            {
                case 10501:
                    pInfo.attribute = 0;
                    break;
                case 10502:
                    pInfo.attribute = 1;
                    break;
                case 10503:
                    pInfo.attribute = 2;
                    break;
            }
            pInfo.uid = projectile.uid;
            pInfo.name = projectile.name;
            pInfo.ATK = projectile.ATK;
            pInfo.owner = transform;
        }
    }

    public void InitHelmet(TableEntity_Item helmet)
    {
        if (helmet.uid == -1 || helmet.resources == "")
        {
            unit.helmet.gameObject.SetActive(false);
        }
        else
        {
            unit.helmet.gameObject.SetActive(true);
            GameObject item = Resources.Load<GameObject>(helmet.resources);
            if (item != null)
            {
                unit.helmet.gameObject.GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
                unit.helmet.gameObject.GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
    }

    public void StartTrail()
    {
        unit.onehand.StartTrail();
    }
    public void StopTrail()
    {
        unit.onehand.StopTrail();
    }

    public void SpawnEffect()
    {
        characterEffect.PlayEffect((int)EffectType.Spawn);
    }

    public void PotionEffect()
    {
        characterEffect.PlayEffect((int)EffectType.Potion);
    }

    protected float CalculateDamage(float takeDamage)
    {
        float result = takeDamage - unit.DEF;
        return result > 0 ? result : 0;
    }

    protected void Move()
    {
        if (unit.buff.Buff == BuffType.Frozen)
        {
            rig.MovePosition(transform.position + moveDir * Time.deltaTime * 3f);
        }
        else
        {
            rig.MovePosition(transform.position + moveDir * Time.deltaTime * (float)unit.state);
        }
        transform.LookAt(transform.position + moveDir);
    }

    protected void Jump()
    {
        rig.velocity = Vector3.up * 3f;
    }

    protected void StopAllEffect()
    {
        characterEffect.StopAllEffect();
    }


    public State GetTargetState()
    {
        return unit.state;
    }
    public AttackType GetTargetAttack()
    {
        return unit.attack;
    }
    public BuffType GetTargetBuff()
    {
        return unit.buff.Buff;
    }
}
