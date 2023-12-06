using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    private Image hp;
    private float max;

    public void InitHP(float max)
    {
        if (!transform.GetChild(0).TryGetComponent<Image>(out hp))
            Debug.Log("HP - Init - Image");

        this.max = max;
        hp.fillAmount = 1;
    }

    public void SetCurrentHP(float hpAmount)
    {
        hp.fillAmount = hpAmount/max;
    }
}
