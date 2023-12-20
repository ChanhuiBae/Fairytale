using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : MonoBehaviour
{
    private TextMeshProUGUI coin;
    private Image i_onehand;
    private Image i_onehand_enchant;
    private TextMeshProUGUI i_onehand_info;
    private WearingItemButton i_onehand_btn;
    private Image i_shield;
    private Image i_shield_enchant;
    private TextMeshProUGUI i_shield_info;
    private WearingItemButton i_shield_btn;
    private Image i_ranged;
    private Image i_ranged_enchant;
    private TextMeshProUGUI i_ranged_info;
    private WearingItemButton i_ranged_btn;
    private Image i_helmet;
    private WearingItemButton i_helmet_btn;
    private TextMeshProUGUI i_helmet_info;
    private TextMeshProUGUI i_projectile;
    private TextMeshProUGUI i_HpP;
    private TextMeshProUGUI i_StaminaP;
    private TextMeshProUGUI playerDEF;

    private TextMeshProUGUI nickname;
    private List<ItemButton> itemButtons;

    private Transform namePopup;
    private Transform helmetPopup;
    private Transform weaponPopup;
    private Button deleteButton;
    private InventoryItemData selectedItem;

    private bool sort;

    public RectTransform scrollContent;


    private void Awake()
    {
        transform.LeanScale(Vector3.zero, 0);
        sort = false;

        if (!transform.Find("NamePopup").TryGetComponent<Transform>(out namePopup))
            Debug.Log("InventoryPopupManager -Awake - Transform");
        else
        {
            namePopup.gameObject.SetActive(false);
        }
        if (!transform.Find("HelmetPopup").TryGetComponent<Transform>(out helmetPopup))
            Debug.Log("InventoryPopupManager -Awake - Transform");
        else
        {
            helmetPopup.gameObject.SetActive(false);
        }
        if (!transform.Find("WeaponPopup").TryGetComponent<Transform>(out weaponPopup))
            Debug.Log("InventoryPopupManager -Awake - Transform");
        else
        {
            if (!weaponPopup.Find("DeleteButton").TryGetComponent<Button>(out deleteButton))
                Debug.Log("InventoryPopup - Awake - Button");
            else
            {
                deleteButton.onClick.AddListener(DeleteItem);
            }
            weaponPopup.gameObject.SetActive(false);
        }

        itemButtons = new List<ItemButton>();
        scrollContent = transform.Find("InvenBack/InvenView/Viewport/Content").GetComponent<RectTransform>();
        if (scrollContent == null)
            Debug.Log("InventoryPopup - Awake - RectTransform");
        Transform pointer = transform.Find("InvenBack/InvenView/Viewport/Content");
        for (int i=0; i < 29; i++)
        {
            itemButtons.Add(pointer.GetChild(i).GetComponent<ItemButton>());
        }


        if (!transform.Find("CharBack/Nickname").TryGetComponent<TextMeshProUGUI>(out nickname))
            Debug.Log("InventoryPopupManager - Init - TextMeshProUGUI");
        else
        {
            nickname.text = GameManager.Inst.PlayerName;
        }

        if (!transform.Find("CharBack/CoinInfo/Coin").TryGetComponent<TextMeshProUGUI>(out coin))
            Debug.Log("InventoryPopupManager - Awake - TextMeshProUGUI");

        if (!transform.Find("CharBack/DEF/DEFValue").TryGetComponent<TextMeshProUGUI>(out playerDEF))
            Debug.Log("InventoryPopupManager - Awake -TextMeshProUGUI");

        if (!transform.Find("CharBack/OnehandItem/Back/Item").TryGetComponent<Image>(out i_onehand))
            Debug.Log("InventoryPopupManager -Awake - Image");
        if (!transform.Find("CharBack/OnehandItem/Enchant").TryGetComponent<Image>(out i_onehand_enchant))
            Debug.Log("InventoryPopupManager -Awake - Image");
        if (!transform.Find("CharBack/OnehandItem/Info").TryGetComponent<TextMeshProUGUI>(out i_onehand_info))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
        if (!i_onehand_info.transform.parent.TryGetComponent<WearingItemButton>(out i_onehand_btn))
            Debug.Log("InventoryPopupManager -Awake - WearingItemButton");

        if (!transform.Find("CharBack/ShieldItem/Back/Item").TryGetComponent<Image>(out i_shield))
            Debug.Log("InventoryPopupManager -Awake - Image"); 
        if (!transform.Find("CharBack/ShieldItem/Enchant").TryGetComponent<Image>(out i_shield_enchant))
            Debug.Log("InventoryPopupManager -Awake - Image");
        if (!transform.Find("CharBack/ShieldItem/Info").TryGetComponent<TextMeshProUGUI>(out i_shield_info))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
        if (!i_shield_info.transform.parent.TryGetComponent<WearingItemButton>(out i_shield_btn))
            Debug.Log("InventoryPopupManager -Awake - WearingItemButton");

        if (!transform.Find("CharBack/RangedItem/Back/Item").TryGetComponent<Image>(out i_ranged))
            Debug.Log("InventoryPopupManager -Awake - Image");
        if (!transform.Find("CharBack/RangedItem/Enchant").TryGetComponent<Image>(out i_ranged_enchant))
            Debug.Log("InventoryPopupManager -Awake - Image");
        if (!transform.Find("CharBack/RangedItem/Info").TryGetComponent<TextMeshProUGUI>(out i_ranged_info))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
        if (!i_ranged_info.transform.parent.TryGetComponent<WearingItemButton>(out i_ranged_btn))
            Debug.Log("InventoryPopupManager -Awake - WearingItemButton");

        if (!transform.Find("CharBack/HelmetItem/Back/Item").TryGetComponent<Image>(out i_helmet))
            Debug.Log("InventoryPopupManager -Awake - Image"); 
        if (!transform.Find("CharBack/HelmetItem/Info").TryGetComponent<TextMeshProUGUI>(out i_helmet_info))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
        if (!i_helmet_info.transform.parent.TryGetComponent<WearingItemButton>(out i_helmet_btn))
            Debug.Log("InventoryPopupManager -Awake - WearingItemButton");

        if (!transform.Find("CharBack/Arrow/Info").TryGetComponent<TextMeshProUGUI>(out i_projectile))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");

        if (!transform.Find("CharBack/HpPotion/Info").TryGetComponent<TextMeshProUGUI>(out i_HpP))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");

        if (!transform.Find("CharBack/StaminaPotion/Info").TryGetComponent<TextMeshProUGUI>(out i_StaminaP))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
    }

    public void SetRectPosition()
    {
        float x = scrollContent.anchoredPosition.x;
        scrollContent.anchoredPosition = new Vector3(x, 0, 0);
    }
    public void UpdatePlayer()
    {
        coin.text = GameManager.Inst.PlayerCoin.ToString();
        playerDEF.text = GameManager.Inst.PlayerInfo.DEF.ToString();
        InventoryItemData data;
        if (GameManager.Inst.PlayerInfo.i_onehand != -1)
        {
            i_onehand.transform.parent.gameObject.SetActive(true);
            i_onehand.enabled = true;
            i_onehand.sprite = Resources.Load<Sprite>(GameManager.Inst.PlayerInfo.onehand.iconResources);
            data = GameManager.Inst.INVENTORY.GetItem(GameManager.Inst.PlayerInfo.i_onehand);
            if (data.enchant)
            {
                i_onehand_enchant.enabled = true;
            }
            else
            {
                i_onehand_enchant.enabled = false;
            }
            i_onehand_info.text = data.durability.ToString();
            i_onehand_btn.UpdateItemButton(data);
        }
        else
        {
            i_onehand.transform.parent.gameObject.SetActive(false);
            i_onehand.enabled = false;
            i_onehand_enchant.enabled = false; 
            i_onehand_info.text = "";
            i_onehand_btn.InitButton();
        }

        if (GameManager.Inst.PlayerInfo.i_shield != -1)
        {
            i_shield.transform.parent.gameObject.SetActive(true);
            i_shield.enabled = true;
            i_shield.sprite = Resources.Load<Sprite>(GameManager.Inst.PlayerInfo.shield.iconResources);
            data = GameManager.Inst.INVENTORY.GetItem(GameManager.Inst.PlayerInfo.i_shield);
            if(data != null)
            {
                if (data.enchant)
                {
                    i_shield_enchant.enabled = true;
                }
                else
                {
                    i_shield_enchant.enabled = false;
                }
                i_shield_info.text = data.durability.ToString();
                i_shield_btn.UpdateItemButton(data);
            }
        }
        else
        {
            i_shield.transform.parent.gameObject.SetActive(false);
            i_shield.enabled = false;
            i_shield_enchant.enabled = false;
            i_shield_info.text = "";
            i_shield_btn.InitButton();
        }

        if (GameManager.Inst.PlayerInfo.i_ranged != -1)
        {
            i_ranged.transform.parent.gameObject.SetActive(true);
            i_ranged.enabled = true;
            i_ranged.sprite = Resources.Load<Sprite>(GameManager.Inst.PlayerInfo.ranged.iconResources);
            data = GameManager.Inst.INVENTORY.GetItem(GameManager.Inst.PlayerInfo.i_ranged);
            if (data.enchant)
            {
                i_ranged_enchant.enabled = true;
            }
            else
            {
                i_ranged_enchant.enabled = false;
            }
            i_ranged_info.text = data.durability.ToString();
            i_ranged_btn.UpdateItemButton(data);
        }
        else
        {
            i_ranged.transform.parent.gameObject.SetActive(false);
            i_ranged_enchant.enabled = false;
            i_ranged_info.text = "";
            i_ranged_btn.InitButton();
        }

        if (GameManager.Inst.PlayerInfo.i_helmet != -1)
        {
            i_helmet.transform.parent.gameObject.SetActive(true);
            i_helmet.enabled = true;
            i_helmet.sprite = Resources.Load<Sprite>(GameManager.Inst.PlayerInfo.helmet.iconResources);
            data = GameManager.Inst.INVENTORY.GetItem(GameManager.Inst.PlayerInfo.i_helmet);
            i_helmet_info.text = GameManager.Inst.PlayerInfo.helmet.DEF.ToString();
            i_helmet_btn.UpdateItemButton(data);
        }
        else
        {
            i_helmet.transform.parent.gameObject.SetActive(false);
            i_helmet.enabled = false;
            i_helmet_info.text = "";
            i_helmet_btn.InitButton();
        }

        int amount = GameManager.Inst.INVENTORY.GetItemAmount(10501);
        if (amount > -1)
            i_projectile.text = amount.ToString();
        else
            i_projectile.text = "0";
        amount = GameManager.Inst.INVENTORY.GetItemAmount(30107);
        if (amount > -1)
            i_HpP.text = amount.ToString();
        else
            i_HpP.text = "0";
        amount = GameManager.Inst.INVENTORY.GetItemAmount(30108);
        if (amount > -1)
            i_StaminaP.text = amount.ToString();
        else
            i_StaminaP.text = "0";

    }
    public void UpdateInventory()
    {
        List<InventoryItemData> inven = GameManager.Inst.INVENTORY.GetItemList();
        if (sort)
        {
            inven = inven.OrderBy(x => x.itemID).ToList();
        }
        TableEntity_Weapon weapondata;
        TableEntity_Item Itemdata;
        GameObject itemMeshdata;

        int popupIndex = 0;
        SetRectPosition();
        for (int i = 0; i < inven.Count; i++, popupIndex++)
        {
            if (inven[i].itemID == 10501) // arrow
            {
                popupIndex--;
            }
            else if (inven[i].itemID == 30107) // hp potion
            {
                popupIndex--;
            }
            else if (inven[i].itemID == 30108) // stamina potion
            {
                popupIndex--;
            }
            else if (inven[i].type < 5) // weapon
            {
                itemButtons[popupIndex].UpdateItemButton(inven[i]);
            }
            else // item
            {
                itemButtons[popupIndex].UpdateItemButton(inven[i]);
            }
        }
        for (int i = popupIndex; i < itemButtons.Count; i++)
        {
            itemButtons[i].InitButton();
        }
    }

    public void ShowNamePopup(string name, string amount)
    {
        namePopup.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        namePopup.GetChild(1).GetComponent<TextMeshProUGUI>().text = amount;
        namePopup.gameObject.SetActive(true);
        helmetPopup.gameObject.SetActive(false);
        weaponPopup.gameObject.SetActive(false);
    }

    public void CloseNamePopup()
    {
        namePopup.gameObject.SetActive(false);
    }

    public void ShowHelmetPopup(string name, int DEF)
    {
        helmetPopup.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        helmetPopup.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DEF.ToString();
        helmetPopup.gameObject.SetActive(true);
        namePopup.gameObject.SetActive(false);
        weaponPopup.gameObject.SetActive(false);
    }

    public void CloseHelmetPopup()
    {
        helmetPopup.gameObject.SetActive(false);
    }

    public void ShowWeaponPopup(InventoryItemData data)
    {
        selectedItem = new InventoryItemData();
        selectedItem.uid = data.uid;
        selectedItem.itemID = data.itemID;
        selectedItem.amount = data.amount;
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(data.itemID, out weapon);
        weaponPopup.GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.name;
        if(data.enchant)
            weaponPopup.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (weapon.ATK * 1.2f).ToString();
        else
            weaponPopup.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.ATK.ToString();
        weaponPopup.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = data.durability.ToString() + " / " + weapon.durability.ToString();
        weaponPopup.gameObject.SetActive(true);
        namePopup.gameObject.SetActive(false);
        helmetPopup.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
    }

    public void ShowDeleteWeaponPopup(InventoryItemData data)
    {
        selectedItem = new InventoryItemData();
        selectedItem.uid = data.uid;
        selectedItem.itemID = data.itemID;
        selectedItem.amount = data.amount;
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(data.itemID, out weapon);
        weaponPopup.GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.name;
        if (data.enchant)
            weaponPopup.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (weapon.ATK * 1.2f).ToString();
        else
            weaponPopup.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.ATK.ToString();
        weaponPopup.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = data.durability.ToString() + " / " + weapon.durability.ToString();
        weaponPopup.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        namePopup.gameObject.SetActive(false);
        helmetPopup.gameObject.SetActive(false);
    }

    public void CloseWeaponPopup()
    {
        weaponPopup.gameObject.SetActive(false);
    }

    public void DeleteItem()
    {
        GameManager.Inst.INVENTORY.DeleteItem(selectedItem);
        CloseWeaponPopup();
        UpdateInventory();
    }
}
