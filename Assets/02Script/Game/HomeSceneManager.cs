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
        if(!GameObject.Find("SmithyPopup").TryGetComponent<SmithyPopup>(out smithy))
            Debug.Log("HomeSceneManager - Awake - SmithyPopup");
    }

    public void ShowPotionShopPopup()
    {
        potionshop.InitPotionShopPopup();
    }

    public void ClosePotionPopup()
    {
        potionshop.ClosePotionShopPopup();
    }

    public void ShowSmithyPopup()
    {
        smithy.InitSmithyPopup();
    }

    public void CloseSmithyPopup()
    {
        smithy.CloseSmithyPopup();
    }
}
