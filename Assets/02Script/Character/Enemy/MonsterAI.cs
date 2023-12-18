using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AI_State
{
    Idle,
    Lay,
    Roaming,    // 스폰 위피를 기준으로 반경 일정 거리를 배회하는 상태
    ReturnHome,
    Chase,
    Attack,
    Defense, 
    Evasion,
    Die
}

public enum AttackDistance
{
    Ranged = 20,
    Onhand = 3,
    Both = 9,
}

public class MonsterAI : MonoBehaviour
{
    protected NavMeshAgent navAgent;
    protected AI_State currentState;
    protected MonsterBase IBase;

    protected GameObject attackTarget;
    protected CharacterBase targetBase;
    protected AttackType targetState;

    protected Vector3 homePos;
    protected Vector3 movePos;

    protected bool haveOneHand;
    protected bool haveShield;
    protected bool haveRanged;
    protected AttackDistance attackDistance;

    protected bool isInit;

    // MosterBase 초기화 시 호출
    public void InitAI(bool onehand, bool shield, bool ranged)
    {
        if (!TryGetComponent<NavMeshAgent>(out navAgent))
            Debug.Log("MonsterAI - Init - NavMeshAgent");
        if (!TryGetComponent<MonsterBase>(out IBase))
            Debug.Log("MonsterAI - Init - MonsterBase");
        isInit = true;
        attackTarget = null;
        homePos = transform.position;
        haveOneHand = onehand;
        haveShield = shield;
        haveRanged = ranged;
        if (haveRanged)
            attackDistance = AttackDistance.Ranged;
        else
            attackDistance = AttackDistance.Onhand;
    }
    protected void ChangeAIState(AI_State newState)
    {
        if (isInit)
        {
            StopCoroutine(currentState.ToString());
            currentState = newState;
            StartCoroutine(currentState.ToString());
        }
    }

    protected void SetMoveTarget(Vector3 targetPos)
    {
        // debug message 남길 수 있도록 이동 로직을 하나의 평선으로 통일
        navAgent.SetDestination(targetPos);
    }

    protected float GetDistanceToTarget()
    {
        if (attackTarget)
            return (attackTarget.transform.position - transform.position).sqrMagnitude;
        return -1;
    }


    public void SetTarget(GameObject newTarget)
    {
        if (currentState == AI_State.Idle || currentState == AI_State.Roaming)
        {
            attackTarget = newTarget;
            try
            {
                attackTarget.TryGetComponent<CharacterBase>(out targetBase);
                AttackType targetState = targetBase.GetTargetAttack();
            }
            catch (NullReferenceException e)
            {
                Debug.Log("A traget don't have CharacterBase.");
            }
            ChangeAIState(AI_State.Chase);
        }
    }


    public void Spawn()
    {
        ChangeAIState(AI_State.Roaming); // first state
    }

    protected IEnumerator Roaming()
    {
        navAgent.speed = (float)MoveType.Walk;
        yield return null;
        while (true)
        {
            movePos.x = UnityEngine.Random.Range(-6f, 6f);
            movePos.y = transform.position.y;
            movePos.z = UnityEngine.Random.Range(-6f, 6f);
            Vector3 target = homePos + movePos;
            SetMoveTarget(target);
            IBase.Walk();
            while((target - transform.position).sqrMagnitude > 0.3)
            {
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
            IBase.StopMove();
            yield return YieldInstructionCache.WaitForSeconds(UnityEngine.Random.Range(7f, 10f));
        }
    }

    protected IEnumerator Chase()
    {
        navAgent.speed = (float)MoveType.Run;
        IBase.Run();
        yield return null;
        while (attackTarget != null)
        {
            if (GetDistanceToTarget() <= (float)attackDistance)
            {
                navAgent.SetDestination(transform.position);
                IBase.StopMove();
                ChangeAIState(AI_State.Attack);
            }
            else if (GetDistanceToTarget() >70f)
            {
                attackTarget = null;
            }
            else
            {
                SetMoveTarget(attackTarget.transform.position); // 이동 목표점을 갱신
                targetState = targetBase.GetTargetAttack();
                if (targetState == AttackType.Ranged)
                {
                    if (haveShield)
                    {
                        IBase.StopMove();
                        ChangeAIState(AI_State.Defense);
                    }
                    else
                    {
                        IBase.StopMove();
                        ChangeAIState(AI_State.Evasion);
                    }
                }
            }
            yield return YieldInstructionCache.WaitForSeconds(0.2f);
        }
        ChangeAIState(AI_State.ReturnHome);
    }

    protected IEnumerator ReturnHome()
    {
        yield return null;
        SetMoveTarget(homePos);
        IBase.Run();

        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            if (navAgent.remainingDistance < 1f) // 목표까지 남은 거리
                ChangeAIState(AI_State.Roaming);
        }
    }

    protected IEnumerator Attack()
    {

        while (attackTarget.layer == LayerMask.NameToLayer("Player"))
        {
            float distance = GetDistanceToTarget();
            if ( distance > (float)attackDistance)
            {
                if (haveRanged && haveOneHand && distance < (float)AttackDistance.Ranged)
                {
                    attackDistance = AttackDistance.Ranged;
                }
                else
                {
                    IBase.StopAttack();
                    ChangeAIState(AI_State.Chase);
                }
            }
            if(haveRanged && haveOneHand &&  distance < (float)AttackDistance.Both && distance > (float)AttackDistance.Onhand)
            {
                attackDistance = AttackDistance.Onhand;
                IBase.StopAttack();
                ChangeAIState(AI_State.Chase);
            }
            yield return YieldInstructionCache.WaitForSeconds(1f);
            transform.LookAt(attackTarget.transform);
            if(targetBase.GetTargetState() == State.Attack)
            {
                IBase.StopAttack();
                if(haveShield)
                {
                    ChangeAIState(AI_State.Defense);
                }
                else
                {
                    ChangeAIState(AI_State.Evasion);
                }
            }
            else
            {
                if (targetBase.GetTargetBuff() != Buff.None)
                {
                    if (attackDistance == AttackDistance.Ranged)
                    {
                        IBase.SkillRanged();
                    }
                    else
                    {
                        IBase.SkillOnehand();
                    }
                }
                else
                {
                    if (attackDistance == AttackDistance.Ranged)
                    {
                        IBase.AttackRanged();
                    }
                    else
                    {
                        IBase.AttackOnhand();
                    }
                }
            }
            yield return YieldInstructionCache.WaitForSeconds(1f);
            IBase.StopAttack();
            

        }
        IBase.StopAttack();
        attackTarget = null;
        ChangeAIState(AI_State.ReturnHome);
    }

    protected IEnumerator Defense()
    {
        navAgent.SetDestination(transform.position);
        IBase.Defense();
        targetState = targetBase.GetTargetAttack();
        while (targetState != AttackType.None)
        {
            transform.LookAt(attackTarget.transform);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
            targetState = targetBase.GetTargetAttack();
        }
        IBase.StopDefense();
        ChangeAIState(AI_State.Chase);
    }

    protected IEnumerator Evasion()
    {
        if(targetState == AttackType.Ranged)
        {
            yield return null;
            IBase.StartDash();
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        else
        {
            yield return null;
            IBase.StartRollBack();
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        yield return null;
        ChangeAIState(AI_State.Attack);
    }

    protected IEnumerable Idle()
    {
        yield return null;
        navAgent.speed = 0f;
    }

    public void Die()
    {
        StopAllCoroutines();
        currentState = AI_State.Die;
        attackTarget = null;
        StartCoroutine(AI_State.Idle.ToString());
    }

    public void SetIdle()
    {
        currentState = AI_State.Idle;
        attackTarget = null;
        StartCoroutine(AI_State.Idle.ToString());
    }

    public void TakeDebuff(float time)
    {
        if(currentState != AI_State.Die)
        {
            StopAllCoroutines();
            StartCoroutine(StopAction(time));
        }
    }

    protected IEnumerator StopAction(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        StartCoroutine(currentState.ToString());
    }
}