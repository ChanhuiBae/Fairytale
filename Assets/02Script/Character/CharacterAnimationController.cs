using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator anim;

    private int H_Walk = Animator.StringToHash("Walk");
    private int H_Run = Animator.StringToHash("Run");
    private int H_Dash = Animator.StringToHash("Dash");

    private int H_TakeDamgae = Animator.StringToHash("Take Damage");
    private int H_Stunned = Animator.StringToHash("Stunned");
    private int H_Die = Animator.StringToHash("Die");
    private int H_Spawn = Animator.StringToHash("Spawn Ground");

    private int H_A_R_Swing = Animator.StringToHash("Melee Right Attack 02");
    private int H_A_R_Sting = Animator.StringToHash("Melee Right Attack 01");
    private int H_A_R_Jump = Animator.StringToHash("Jump Right Attack 01");

    private int H_A_B_Aim = Animator.StringToHash("Longbow Aim 01");
    private int H_A_B_Shoot = Animator.StringToHash("Longbow Shoot Attack 01");
    private int H_A_W_Aim = Animator.StringToHash("Crossbow Aim");
    private int H_A_W_Cast = Animator.StringToHash("Cast Spell 02");

    private int H_Defend = Animator.StringToHash("Defend");

    private int H_Relax = Animator.StringToHash("Relax");
    private int H_Sitting = Animator.StringToHash("Sitting");
    private int H_Drink = Animator.StringToHash("Drink Potion");
    private int H_Talking = Animator.StringToHash("Talking");
    private int H_Digging = Animator.StringToHash("Digging");
    private int H_ChopTree = Animator.StringToHash("Chop Tree");
    private int H_Hammering = Animator.StringToHash("Hammering On Anvil");



    public void InitAinmController()
    {
        if (!TryGetComponent<Animator>(out anim))
            Debug.Log("CharacterAnimationController - Init - Animator");
    }

    public void StopMove()
    {
        anim.SetBool(H_Walk, false);
        anim.SetBool(H_Run, false);
        anim.SetBool(H_Dash, false);
    }

    public void Walk()
    {
        anim.SetBool(H_Walk, true);
        anim.SetBool(H_Run, false);
        anim.SetBool(H_Dash, false);
    }

    public void Run()
    {
        anim.SetBool(H_Run, true);
        anim.SetBool(H_Dash, false);
    }

    public void Dash()
    {
        anim.SetBool(H_Dash, true);
    }
    

    public void JumpAttack()
    {
        anim.SetTrigger(H_A_R_Jump);
    }

    public void Sting()
    {
        anim.SetTrigger(H_A_R_Sting);
    }
    
    public void Swing()
    {
        anim.SetTrigger (H_A_R_Swing);
    }

    public void Defend()
    {
        anim.SetBool(H_Defend, true);
    }

    public void StopDefend()
    {
        anim.SetBool(H_Defend, false);
    }

    public void BowAim()
    {
        anim.SetBool(H_A_B_Aim, true);
    }

    public void BowShoot()
    {
        anim.SetTrigger(H_A_B_Shoot);
    }
    public void StopBowAim()
    {
        anim.SetBool(H_A_B_Aim, false);
    }
    public void WandAim()
    {
        anim.SetBool(H_A_W_Aim, true);
    }

    public void WandShoot()
    {
        anim.SetTrigger(H_A_W_Aim);
    }
    public void StopWandAim()
    {
        anim.SetBool(H_A_W_Aim, false);
    }

    public void Hit()
    {
        anim.SetTrigger(H_TakeDamgae);
    }

    public void Stun()
    {
        anim.SetBool(H_Stunned, true);
    }

    public void StopStun()
    {
        anim.SetBool(H_Stunned, false);
    }

    public void Die()
    {
        anim.SetTrigger(H_Die);
    }

    public void Spawn()
    {
        anim.SetTrigger(H_Spawn);
    }

    public void Relax()
    {
        anim.SetBool(H_Relax, true);
    }

    public void StopRelax()
    {
        anim.SetBool(H_Relax, false);
    }

    public void Hammering()
    {
        anim.SetBool(H_Hammering, true);
    }

    public void StopHammering()
    {
        anim.SetBool(H_Hammering, false);
    }

    public void Talking()
    {
        anim.SetBool (H_Talking, true);
    }

    public void StopTalk()
    {
        anim.SetBool(H_Talking,false);
    }

    public void Chop()
    {
        anim.SetBool(H_ChopTree, true);
    }

    public void StopChop()
    {
        anim.SetBool(H_ChopTree, false);
    }

    public void Digging()
    {
        anim.SetBool(H_Digging, true);
    }

    public void StopDig()
    {
        anim.SetBool(H_Digging, false);
    }

    public void Sit()
    {
        anim.SetBool(H_Sitting, true);
    }

    public void Drink()
    {
        anim.SetTrigger(H_Drink);
    }

    public void Cast()
    {
        anim.SetTrigger(H_A_W_Cast);
    }

    public bool CheckPlaying()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            return false;
        }
        return true;
    }
    
    public bool IsIdle()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }

    public bool IsJumping()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
    }
    public bool IsJumpAttack()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump Right Attack 01"));
    }

    public bool IsDrinking()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Drink Potion"));
    }

    public bool IsLying()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("On the Ground Loop"));
    }

    public bool IsAimming()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Longbow Aim 01"));
    }

    public bool IsShooting()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Longbow Shoot Attack 01"));
    }
    public bool isCasting()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Cast Spell 02"));
    }
}
