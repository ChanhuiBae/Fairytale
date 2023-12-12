using System.Collections;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    private PotionShopPopup potionshop;
    private SmithyPopup smithy;

    private void Awake()
    {
        if (!GameObject.Find("PotionShopPopup").TryGetComponent<PotionShopPopup>(out potionshop))
            Debug.Log("HomeSceneManager - Awake - PotionShopPopup");
        if (!GameObject.Find("SmithyPopup").TryGetComponent<SmithyPopup>(out smithy))
            Debug.Log("HomeSceneManager - Awake - SmithypPopup");
    }

    public void ShowPotionShopPopup()
    {
        potionshop.InitPotionShopPopup();
        GameManager.Inst.PlayerIsController(false);
    }

    public void ShowSmithyPopup()
    {
        smithy.InitSmithyPopup();
        GameManager.Inst.PlayerIsController(false);
    }



}
