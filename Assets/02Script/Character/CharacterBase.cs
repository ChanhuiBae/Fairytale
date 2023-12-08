using System.Collections;
using UnityEngine;

public enum MoveType
{
    None = 0,
    Walk = 5,
    Run = 7,
    Dash = 40,
    RollBack = 13
}

public enum JumpType
{
    None,
    Jump,
    Attack
}

public enum AttackType
{
    None,
    Onehand,
    Defense,
    Ranged,
    Jump
}

public enum OnehandAttack
{
    None,
    Charge,
    Sting,
    Swing
}

public enum RangedAttack
{
    None,
    ImmediatelyShoot,
    Aim,
    Shoot,
    Cast
}

public class CharacterUnit
{
    public float currentHP;
    public float maxHP;
    public float DEF;
    public State state;
    public MoveType moveType; // It is used move speed.
    public AttackType attackType;
    public JumpType jumpType;
    public OnehandAttack onehandAttack;
    public RangedAttack rangedAttack;
    public float j_Power;
    public Transform back;
    public Transform lefthand;
    public Transform righthand;
    public Transform helmet;
    public bool useRanged;
    public OneHandWeapon onehand;
    public Shield shield;
    public RangedWeapon ranged;
}

public class CharacterBase : MonoBehaviour
{
    protected CharacterAnimationController anim;
    protected Rigidbody rig;
    protected Material material;

    protected CharacterUnit unit = new CharacterUnit();
    protected ProjectileInfo pInfo = new ProjectileInfo();
    protected Vector3 moveDir = Vector3.zero;

    public void InitCharBase(float maxhp, float DEF)
    {
        material = GetComponentInChildren<SkinnedMeshRenderer>().material;

        if (!TryGetComponent<CharacterAnimationController>(out anim))
            Debug.Log("CharacterBase - Init - CharacterAnimationController");

        if (!TryGetComponent<Rigidbody>(out rig))
            Debug.Log("CharacterBase - Init - Rigidbody");

        anim.InitAinmController();

        unit.state = State.Idle;
        unit.moveType = MoveType.None;
        unit.attackType = AttackType.None;
        unit.jumpType = JumpType.None;
        unit.onehandAttack = OnehandAttack.None;
        unit.j_Power = 3f;

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
        unit.onehand.InitOneHandWeapon(onehand.ATK, onehand.durability, enchant);
        if(onehand.ATK == 0)
        {
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
            if(enchant)
            {
                unit.onehand.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                ParticleSystem ps = unit.onehand.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = item.GetComponent<MeshFilter>().sharedMesh;
                //sh.texture = item.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
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
                //sh.texture = item.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
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
        if(ranged.ATK == 0)
        {
            unit.ranged.gameObject.SetActive(false);
            unit.back.parent.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            GameObject item = Resources.Load<GameObject>(ranged.resources);
            unit.ranged.gameObject.SetActive(true);
            if(gameObject.tag == "Player")
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
                //sh.texture = item.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
            }
            else
            {
                unit.ranged.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void InitProjectileInfo(TableEntity_Weapon projectile)
    {
        if(projectile.ATK == 0)
        {
            pInfo.poolIndex = -1;
            pInfo.uid = -1;
            pInfo.name = "";
            pInfo.ATK = 0;
        }
        else
        {
            switch (projectile.uid)
            {
                case 10501:
                    pInfo.poolIndex = 0;
                    break;
                case 10502:
                    pInfo.poolIndex = 1;
                    break;
                case 10503:
                    pInfo.poolIndex = 2;
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
        if(helmet.uid == -1 || helmet.resources == "")
        {
            unit.helmet.gameObject.SetActive(false);
        }
        else
        {
            unit.helmet.gameObject.SetActive(true);
            GameObject item = Resources.Load<GameObject>(helmet.resources);
            if(item != null)
            {
                unit.helmet.gameObject.GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
                unit.helmet.gameObject.GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
    }

    protected float CalculateDamage(float takeDamage)
    {
        float result = takeDamage - unit.DEF;
        return result > 0 ? result : 0;
    }

    protected void Move()
    {
        transform.position += ((float)unit.moveType * Time.deltaTime * moveDir);
        transform.LookAt(transform.position + moveDir);
    }

    protected void Jump()
    {
        rig.velocity = Vector3.up * unit.j_Power;
    }

    protected void LieDown()
    {
        unit.state = State.Lay;
        anim.LieDown();
    }

    protected void StandUp()
    {
        unit.state = State.Idle;
        anim.StandUp();
    }
    virtual protected IEnumerator RollBack() { yield return null; }

    #region Hit&Die
    virtual protected IEnumerator OnDie() { yield return null; }
    virtual public void TakeDamge(float damage) { }
    virtual protected void ChangeHP(float value) { }
    virtual protected IEnumerator OnStun() { yield return null; }
    virtual public void TakeStun(int time) { }
    #endregion

    public State GetTargetState()
    {
        return unit.state;
    }

    public AttackType GetTargetAttack()
    {
        return unit.attackType;
    }
}
