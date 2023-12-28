using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionShopPopup : MonoBehaviour
{
    private Button closePotionShop;
    private EventTrigger agreeCheck;
    private EventTrigger buyEventTrigger;
    private EventTrigger.Entry entry_enter;
    private EventTrigger.Entry entry_exit;
    private Button buyButton;
    private Button hpPlus;
    private Button hpMinus;
    private Button staminaPlus;
    private Button staminaMinus;

    private Image hpPotionCount;
    private Image staminaPotionCount;
    private int hpCurMax;
    private int staminaCurMax;
    private int hpCount;
    private int staminaCount;

    private Image hpPotion;
    private Image staminaPotion;

    private TextMeshProUGUI nickname;
    private TextMeshProUGUI coin;
    private TextMeshProUGUI priceText;

    private int hpPotionPrice;
    private int staminaPotionPrice;
    private int price;

    private void OnEnter(PointerEventData data)
    {
        buyButton.gameObject.LeanScale(Vector3.one, 0f);
    }
    private void OnExit(PointerEventData data)
    {
        buyButton.gameObject.LeanScale(Vector3.zero, 0f);
    }

    private void Awake()
    {
        transform.LeanScale(Vector3.zero, 0f);

        if (!transform.Find("BuyButton").TryGetComponent<Button>(out buyButton))
            Debug.Log("PotionShopPopup - Awake - Button");
        else
        {
            buyButton.onClick.AddListener(Buy);
        }
        if(!transform.Find("BuyButton").TryGetComponent<EventTrigger>(out buyEventTrigger))
            Debug.Log("PotionShopPopup - Awake - EventTrigger");
        else
        {
            entry_exit = new EventTrigger.Entry();
            entry_exit.eventID = EventTriggerType.PointerExit;
            entry_exit.callback.AddListener((data) => { OnExit((PointerEventData)data); });
            buyEventTrigger.triggers.Add(entry_exit);
        }

        if (!transform.Find("ClosePotionShop").TryGetComponent<Button>(out closePotionShop))
            Debug.Log("PotionShopPopup - Awake - Button");
        else
        {
            closePotionShop.onClick.AddListener(ClosePotionShopPopup);
        }
        if (!transform.Find("AgreeCheck").TryGetComponent<EventTrigger>(out agreeCheck))
            Debug.Log("PotionShopPopup - Awake - EventTrigger");
        else
        {
            entry_enter = new EventTrigger.Entry();
            entry_enter.eventID = EventTriggerType.PointerEnter;
            entry_enter.callback.AddListener((data) => { OnEnter((PointerEventData)data); });
            agreeCheck.triggers.Add(entry_enter);

        }
        if (!transform.Find("HpPlus").TryGetComponent<Button>(out hpPlus))
            Debug.Log("PotionShopPopup - Awake - Button");
        else 
        {
            hpPlus.onClick.AddListener(PlusHpPotion);
        }
        if (!transform.Find("HpMinus").TryGetComponent<Button>(out hpMinus))
            Debug.Log("PotionShopPopup - Awake - Button");
        else
        {
            hpMinus.onClick.AddListener(MinusHpPotion);
        }
        if (!transform.Find("StaminaPlus").TryGetComponent<Button>(out staminaPlus))
            Debug.Log("PotionShopPopup - Awake - Button");
        else
        {
            staminaPlus.onClick.AddListener(PlusStaminaPotion);
        }
        if (!transform.Find("StaminaMinus").TryGetComponent<Button>(out staminaMinus))
            Debug.Log("PotionShopPopup - Awake - Button");
        else
        {
            staminaMinus.onClick.AddListener(MinusStaminaPotion);
        }

        if (!transform.Find("HpPotionFectory/Potion").TryGetComponent<Image>(out hpPotionCount))
            Debug.Log("PotionShopPopup - Awake - Image");
        if(!transform.Find("StaminaPotionFectory/Potion").TryGetComponent<Image>(out staminaPotionCount))
            Debug.Log("PotionShopPopup - Awake - Image");
        if (!transform.Find("HpPotion").TryGetComponent<Image>(out hpPotion))
            Debug.Log("PotionShopPopup - Awake - Image");

        if (!transform.Find("StaminaPotion").TryGetComponent<Image>(out staminaPotion))
            Debug.Log("PotionShopPopup - Awake - Image");

        if(!transform.Find("Contract/Nickname").TryGetComponent<TextMeshProUGUI>(out nickname))
            Debug.Log("PotionShopPopup - Awake - TextMeshProUGUI");
        if (!transform.Find("Contract/Coin").TryGetComponent<TextMeshProUGUI>(out coin))
            Debug.Log("PotionShopPopup - Awake - TextMeshProUGUI");
        if (!transform.Find("Contract/Price").TryGetComponent<TextMeshProUGUI>(out priceText))
            Debug.Log("PotionShopPopup - Awake - TextMeshProUGUI");


        TableEntity_Item item;
        GameManager.Inst.GetItemData(30107, out item);
        hpPotionPrice = item.sellGold;
        GameManager.Inst.GetItemData(30108, out item);
        staminaPotionPrice = item.sellGold;

        nickname.text = GameManager.Inst.PlayerName;
    }

    public void InitPotionShopPopup()
    {
        DateTime userTime;
        DateTime.TryParse(GameManager.Inst.UsedTime, out userTime);
        hpCurMax = GameManager.Inst.ShopHpPotion;
        staminaCurMax = GameManager.Inst.ShopStaminaPotion;

        DateTime time = DateTime.Now;
        if (hpCurMax != 10)
        {
            int deltaHour = time.Hour - userTime.Hour;
            int deltaMinute;
            if (deltaHour == 0)
            {
                deltaMinute = time.Minute - userTime.Minute;
                hpCurMax += deltaMinute;
                if (hpCurMax > 10)
                    hpCurMax = 10;
            }
            else if (deltaHour == 1)
            {
                deltaMinute = 60 - userTime.Minute + time.Minute;
                hpCurMax += deltaMinute;
                if (hpCurMax > 10)
                    hpCurMax = 10;
            }
            else
            {
                hpCurMax = 10;
            }
        }
            
        if(staminaCurMax != 10)
        {
            int deltaHour = time.Hour - userTime.Hour;
            int deltaMinute;
            if (deltaHour == 0)
            {
                deltaMinute = time.Minute - userTime.Minute;
                staminaCurMax += deltaMinute;
                if (staminaCurMax > 10)
                    staminaCurMax = 10;
            }
            else if (deltaHour == 1)
            {
                deltaMinute = 60 - userTime.Minute + time.Minute;
                staminaCurMax += deltaMinute;
                if (staminaCurMax > 10)
                    staminaCurMax = 10;
            }
            else
            {
                hpCurMax = 10;
            }
        }


        GameManager.Inst.SetUsedTime();

        hpCount = 0;
        staminaCount = 0;

        hpPotionCount.fillAmount = (float)hpCurMax / 10;
        staminaPotionCount.fillAmount = (float)staminaCurMax / 10;

        coin.text = GameManager.Inst.PlayerCoin.ToString();
        priceText.text = "0";
        price = 0;

        hpPotion.gameObject.LeanScale(Vector3.zero, 0f);
        staminaPotion.gameObject.LeanScale(Vector3.zero, 0f);
        buyButton.gameObject.LeanScale(Vector3.zero, 0f);
        if(hpCurMax == 0)
        {
            hpPlus.gameObject.LeanScale(Vector3.zero, 0f);
            hpMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        else
        {
            hpPlus.gameObject.LeanScale(Vector3.one, 0f);
            hpMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        if(staminaCurMax == 0)
        {
            staminaPlus.gameObject.LeanScale(Vector3.zero, 0f);
            staminaMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        else
        {
            staminaPlus.gameObject.LeanScale(Vector3.one, 0f);
            staminaMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        transform.LeanScale(Vector3.one, 0.2f);
    }

    public void ClosePotionShopPopup()
    {
        GameManager.Inst.ShopHpPotion = hpCurMax;
        GameManager.Inst.ShopStaminaPotion = staminaCurMax;
        transform.LeanScale(Vector3.zero, 0.2f);
        GameManager.Inst.PlayerIsController(true);
    }

    public void PlusHpPotion()
    {
        hpCount++;
        price += hpPotionPrice;
        priceText.text = price.ToString();
        hpPotionCount.fillAmount = (float)(hpCurMax - hpCount) / 10;
        if (hpCount == 1) 
        {
            hpPotion.gameObject.LeanScale(Vector3.one, 0f);
            hpMinus.gameObject.LeanScale(Vector3.one, 0f);
        }
        else if(hpCount == hpCurMax)
        {
            hpPlus.gameObject.LeanScale(Vector3.zero, 0f);
        }
    }

    public void MinusHpPotion()
    {
        hpCount--;
        price -= hpPotionPrice;
        priceText.text = price.ToString();
        hpPotionCount.fillAmount = (float)(hpCurMax - hpCount) / 10;
        if (hpCount == 0)
        {
            hpPotion.gameObject.LeanScale(Vector3.zero, 0f);
            hpMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        else if (hpCount == hpCurMax - 1)
        {
            hpPlus.gameObject.LeanScale(Vector3.one, 0f);
        }
    }

    public void PlusStaminaPotion()
    {
        staminaCount++;
        price += staminaPotionPrice;
        priceText.text = price.ToString();
        staminaPotionCount.fillAmount = (float)(staminaCurMax - staminaCount) / 10;
        if (staminaCount == 1)
        {
            staminaPotion.gameObject.LeanScale(Vector3.one, 0f);
            staminaMinus.gameObject.LeanScale(Vector3.one, 0f);
        }
        else if (staminaCount == staminaCurMax)
        {
            staminaPlus.gameObject.LeanScale(Vector3.zero, 0f);
        }
    }

    public void MinusStaminaPotion()
    {
        staminaCount--;
        price -= staminaPotionPrice;
        priceText.text = price.ToString();
        staminaPotionCount.fillAmount = (float)(staminaCurMax - staminaCount) / 10;
        if (staminaCount == 0)
        {
            staminaPotion.gameObject.LeanScale(Vector3.zero, 0f);
            staminaMinus.gameObject.LeanScale(Vector3.zero, 0f);
        }
        else if(staminaCount == staminaCurMax - 1)
        {
            staminaPlus.gameObject.LeanScale(Vector3.one, 0f);
        }
    }
    
    public void Buy()
    {
        if(hpCount > 0 || staminaCount > 0)
        {
            GameManager.Inst.PlayerCoin -= price;
            coin.text = GameManager.Inst.PlayerCoin.ToString();
            buyButton.gameObject.LeanScale(Vector3.zero, 0f);

            if (hpCount > 0) 
            { 
                InventoryItemData item = new InventoryItemData();
                item.uid = -1;
                item.itemID = 30107;
                item.amount = hpCount;
                item.durability = 1;
                GameManager.Inst.INVENTORY.AddItem(item);
            }
            if(staminaCount > 0)
            {
                InventoryItemData item = new InventoryItemData();
                item.uid = -1;
                item.itemID = 30108;
                item.amount = staminaCount;
                item.durability = 1;
                GameManager.Inst.INVENTORY.AddItem(item);
            }

            priceText.text = "0";
            price = 0;

            hpCurMax -= hpCount;
            hpPotionCount.fillAmount = (float)hpCurMax / 10;
            hpPotion.gameObject.LeanScale(Vector3.zero, 0f);
            if (hpCurMax == 0)
                hpPlus.gameObject.LeanScale(Vector3.zero, 0f);
            hpMinus.gameObject.LeanScale(Vector3.zero, 0f);

            staminaCurMax -= staminaCount;
            staminaPotionCount.fillAmount = (float)staminaCurMax / 10;
            staminaPotion.gameObject.LeanScale(Vector3.zero, 0f);
            if (staminaCurMax == 0)
                staminaPlus.gameObject.LeanScale(Vector3.zero, 0f);
            staminaMinus.gameObject.LeanScale(Vector3.zero, 0f);

            hpCount = 0;
            staminaCount = 0;
        }
    }

}
