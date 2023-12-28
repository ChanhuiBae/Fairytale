using System.Collections;
using UnityEngine;
using UnityEngine.UI;

enum UseStamina
{
    Run = 4,
    Dash = 50,
    Jump = 6,
    JumpAttack = 10,
}

public class Stamina : MonoBehaviour
{
    private Image stamina;
    private float available;
    private float chargingValue;
    private float max;
    private bool isUse;
    
    public void InitStamina(float staminValue)
    {
        if (!transform.GetChild(0).TryGetComponent<Image>(out stamina))
            Debug.Log("Stamina - Init - Image");
        available = staminValue;
        max = staminValue;
        stamina.fillAmount = 1;
        available = max;
        chargingValue = 5f;
        isUse = false;
        StartCoroutine(UpdateStamina());
    }

    public void SetMaxStamina()
    {
        StopCoroutine(UpdateStamina());
        StopCoroutine(Charge());
        available = max;
        stamina.fillAmount = 1;
        isUse = false;
        StartCoroutine(UpdateStamina());
    }



    private IEnumerator UpdateStamina()
    {
        while (true)
        {
            if (available > max)
            {
                available = max;
            }
            else if (available < max)
            {
                if (!isUse)
                {
                    StartCoroutine(Charge());
                    yield return YieldInstructionCache.WaitForSeconds(0.5f);
                }
                else
                {
                    StopCoroutine(Charge());
                }
            }
            yield return null;
        }
    }


    private IEnumerator Charge()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        available += chargingValue;
        stamina.fillAmount = available/max;
    }

    public void GetStamina(float amout)
    {
        if (available < max)
            available += amout;
    }


    public bool CheckRun()
    {
        if (available - (int)UseStamina.Run >= 0)
        {
            if(!isUse) 
            {
                isUse = true;
                StartCoroutine(Run());
            }
            return true;
        }
        else
        {
            StopCoroutine(Run());
            isUse = false;
            return false;
        }
    }

    private IEnumerator Run()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        if (available - (int)UseStamina.Run >= 0)
        {
            available -= (int)UseStamina.Run;
            stamina.fillAmount = available/max;
            isUse = false;
        }
    }

    public bool CheckDash()
    {
        if (available - (int)UseStamina.Dash >= 0)
        {
            available -= (int)UseStamina.Dash;
            stamina.fillAmount = available / max;
            return true;
        }
        return false;
    }

    public bool CheckJumpAttack()
    {
        if (available - (int)UseStamina.JumpAttack >= 0)
        {
            available -= (int)UseStamina.JumpAttack;
            stamina.fillAmount = available / max;
            return true;
        }
        return false;
    }
}
