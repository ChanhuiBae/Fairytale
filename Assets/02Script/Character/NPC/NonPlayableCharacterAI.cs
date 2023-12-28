using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NonPlayableCharacterAI : MonoBehaviour
{
    private CharacterAnimationController anim;
    private NavMeshAgent navAgent;
    private Vector3 movePos;

    private void Awake()
    {
        try
        {
            TryGetComponent<CharacterAnimationController>(out anim);
            anim.InitAinmController();

            if(transform.tag == "Famer")
            {
                TryGetComponent<NavMeshAgent>(out navAgent);
            }

            StartCoroutine(transform.tag);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private IEnumerator Shop()
    {
        yield return null;
        while (true)
        {
            anim.Talking();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopTalk();
            anim.Cast();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.Relax();
            yield return YieldInstructionCache.WaitForSeconds(60f);
            anim.StopRelax();
        }
    }

    private IEnumerator Blacksmith()
    {
        yield return null;
        while (true)
        {
            anim.Talking();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopTalk();
            anim.Relax();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopRelax();
            yield return YieldInstructionCache.WaitForSeconds(120f);
        }
    }

    private IEnumerator HBlacksmith()
    {
        yield return null;
        while (true)
        {
            anim.Hammering();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopHammering();
            yield return YieldInstructionCache.WaitForSeconds(120f);
        }
    }

    private IEnumerator Citizen()
    {
        yield return null;
        anim.Sit();
    }

    private IEnumerator Famer()
    {
        yield return null;
        while (true)
        {
            movePos.x = UnityEngine.Random.Range(12.5f, 16.5f);
            movePos.y = transform.position.y;
            movePos.z = UnityEngine.Random.Range(0f, 11f);
            navAgent.SetDestination(movePos);
            anim.Walk();
            while (transform.position == movePos)
            {
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
            anim.StopMove();
            anim.Digging();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopDig();
            movePos.x = UnityEngine.Random.Range(12.5f, 16.5f);
            movePos.y = transform.position.y;
            movePos.z = UnityEngine.Random.Range(0f, 11f);
            navAgent.SetDestination(movePos);
            anim.Walk();
            while (transform.position == movePos)
            {
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
            anim.StopMove();
            anim.Chop();
            yield return YieldInstructionCache.WaitForSeconds(120f);
            anim.StopChop();
            yield return YieldInstructionCache.WaitForSeconds(120f);
        }
    }

    private IEnumerator Hunter()
    {
        yield return null;
        while (true)
        {
            anim.Talking();
            yield return YieldInstructionCache.WaitForSeconds(30f);
            anim.StopTalk();
        }
    }
}
