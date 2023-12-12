using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    private InventoryItemData item;
    private EventTrigger ET;
    private EventTrigger.Entry entry_click;
    private Image itemImage;
    private Image enchant;
    private Image use;

    private TableEntity_Item itemdata;
    private TableEntity_Weapon weapondata;

    private TextMeshProUGUI info;
    private bool clicked;
    private bool entered;

    private InventoryPopup inventoryPopup;

    void OnClick(PointerEventData data)
    {
        if (item != null)
        {
            if (item.type < 5)
            {
                if (use.enabled)
                    inventoryPopup.ShowWeaponPopup(item);
                else
                    inventoryPopup.ShowDeleteWeaponPopup(item);
            }
            else if (item.type == 5)
            {
                inventoryPopup.ShowHelmetPopup(itemdata.name, itemdata.DEF);
            }
            else
            {
                InventoryItemData itemAmount;
                inventoryPopup.ShowNamePopup(itemdata.name, info.text);
            }
            if (clicked)
            {
                clicked = false;
                if (item.type < 6)
                {
                    use.enabled = true;
                    inventoryPopup.CloseWeaponPopup();
                    switch (item.type)
                    {
                        case 1:
                            if (item.uid == GameManager.Inst.PlayerInfo.i_onehand)
                            {
                                GameManager.Inst.SetWeaponNullAtInven(item.type);
                            }
                            else
                            {
                                GameManager.Inst.SetWeapon(item.uid, item.itemID);
                            }
                            break;
                        case 2:
                            if (item.uid == GameManager.Inst.PlayerInfo.i_shield)
                            {
                                GameManager.Inst.SetWeaponNullAtInven(item.type);
                            }
                            else
                            {
                                GameManager.Inst.SetWeapon(item.uid, item.itemID);
                            }
                            break;
                        case 5:
                            if (item.uid == GameManager.Inst.PlayerInfo.i_helmet)
                            {
                                GameManager.Inst.SetWeaponNullAtInven(item.type);
                            }
                            else
                            {
                                GameManager.Inst.SetHelmet(item.uid, item.itemID);
                            }
                            break;
                        default:
                            if (item.uid == GameManager.Inst.PlayerInfo.i_ranged)
                            {
                                GameManager.Inst.SetWeaponNullAtInven(item.type);
                            }
                            else
                            {
                                GameManager.Inst.SetWeapon(item.uid, item.itemID);
                            }
                            break;
                    }
                }
            }
            else
            {
                clicked = true;
            }
        }
    }

    private IEnumerator DublieClick()
    {
        clicked = true;
        yield return YieldInstructionCache.WaitForSeconds(1f);
        clicked = false;
    }

    private void Awake()
    {
        entry_click = new EventTrigger.Entry();

        if (!TryGetComponent<EventTrigger>(out ET))
            Debug.Log("ItemButton - Awake - EventTrigger");
        else
        {
            entry_click.eventID = EventTriggerType.PointerClick;
            entry_click.callback.AddListener((data) => { OnClick((PointerEventData)data); });
            ET.triggers.Add(entry_click);
        }
        itemImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (itemImage == null)
            Debug.Log("ItemButton - Awake - Image");
        use = transform.GetChild(1).GetComponent<Image>();
        if (use == null)
            Debug.Log("ItemButton - Awake - Image");
        enchant = transform.GetChild(2).GetComponent<Image>();
        if (enchant == null)
            Debug.Log("ItemButton - Awake - Image");
        info = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        if (info == null)
            Debug.Log("ItemButton - Awake - TextMeshProUGUI");
        else
        {
            info.text = "";
        }
        if (!GameObject.Find("InventoryPopup").TryGetComponent<InventoryPopup>(out inventoryPopup))
            Debug.Log("WearingItemButton - Awake - InventoryPopup");
    }
    public void UpdateItemButton(InventoryItemData data)
    {
        clicked = false;
        entered = false;
        item = data;
        GameObject obj;
        if (item.type < 5) // weapon
        {
            itemImage.transform.parent.gameObject.SetActive(true);
            GameManager.Inst.GetWeaponData(item.itemID, out weapondata);
            obj = Resources.Load<GameObject>(weapondata.resources);
            info.text = item.durability.ToString();
            itemImage.sprite = Resources.Load<Sprite>(weapondata.iconResources);
            if (item.uid == GameManager.Inst.PlayerInfo.i_onehand)
            {
                use.enabled = true;
            }
            else if (item.uid == GameManager.Inst.PlayerInfo.i_shield)
            {
                use.enabled = true;
            }
            else if (item.uid == GameManager.Inst.PlayerInfo.i_ranged)
            {
                use.enabled = true;
            }
            else
            {
                use.enabled = false;
            }

            bool enchantValue = GameManager.Inst.INVENTORY.GetItemEnchant(item.uid);
            if (enchantValue)
            {
                enchant.enabled = true;
            }
            else
            {
                enchant.enabled = false;
            }
        }
        else // item
        {
            GameManager.Inst.GetItemData(item.itemID, out itemdata);
            obj = Resources.Load<GameObject>(itemdata.resources);
            info.text = item.amount.ToString();
            itemImage.sprite = Resources.Load<Sprite>(itemdata.iconResources);
            enchant.enabled = false;
            if (item.uid == GameManager.Inst.PlayerInfo.i_helmet)
            {
                use.enabled = true;
            }
            else
            {
                use.enabled = false;
            }
        }
    }

    public void InitButton()
    {
        info.text = "";
        itemImage.transform.parent.gameObject.SetActive(false);
        use.enabled = false;
        enchant.enabled = false;
        weapondata = null;
        item = null;
        itemdata = null;
    }
}
