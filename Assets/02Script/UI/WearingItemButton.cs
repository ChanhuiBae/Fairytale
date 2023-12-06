using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WearingItemButton : MonoBehaviour
{
    private InventoryItemData item;
    private EventTrigger ET;
    private EventTrigger.Entry entry_click;
    private EventTrigger.Entry entry_enter;
    private EventTrigger.Entry entry_exit;

    private bool clicked;
    private bool entered;
    private InventoryPopup inventoryPopup;

    void OnClick(PointerEventData data)
    {
        if (item != null)
        {
            if (clicked)
            {
                clicked = false;
                if (item.type == 5)
                {
                    inventoryPopup.CloseHelmetPopup();
                }
                else
                {
                    inventoryPopup.CloseWeaponPopup();
                }
                GameManager.Inst.SetWeaponNullAtInven(item.type);
            }
            else
            {
                StartCoroutine(DublieClick());
            }
        }
    }

    private IEnumerator DublieClick()
    {
        clicked = true;
        yield return YieldInstructionCache.WaitForSeconds(5f);
        clicked = false;
    }

    void OnEnter(PointerEventData data)
    {
        if (item != null)
        {
            if(item.type == 5)
            {
                TableEntity_Item helmet;
                GameManager.Inst.GetItemData(item.itemID, out helmet);
                inventoryPopup.ShowHelmetPopup(helmet.name, helmet.DEF);
            }
            else if(item.type <= 2)
            {
                inventoryPopup.ShowWeaponPopup(item);
            }
            else
            {
                inventoryPopup.ShowWeaponPopup(item);
            }
        }
    }
    void OnExit(PointerEventData data)
    {
        if (item != null)
        {
            if (item.type == 5)
            {
                inventoryPopup.CloseHelmetPopup();
            }
        }
    }

    private void Awake()
    {
        entry_click = new EventTrigger.Entry();
        entry_enter = new EventTrigger.Entry();
        entry_exit = new EventTrigger.Entry();

        if (!TryGetComponent<EventTrigger>(out ET))
            Debug.Log("ItemButton - Awake - EventTrigger");
        else
        {
            entry_click.eventID = EventTriggerType.PointerClick;
            entry_click.callback.AddListener((data) => { OnClick((PointerEventData)data); });
            ET.triggers.Add(entry_click);
            entry_enter.eventID = EventTriggerType.PointerEnter;
            entry_enter.callback.AddListener((data) => { OnEnter((PointerEventData)data); });
            ET.triggers.Add(entry_enter);
            entry_exit.eventID = EventTriggerType.PointerExit;
            entry_exit.callback.AddListener((data) => { OnExit((PointerEventData)data); });
            ET.triggers.Add(entry_exit);
        }

        if (!GameObject.Find("InventoryPopup").TryGetComponent<InventoryPopup>(out inventoryPopup))
            Debug.Log("WearingItemButton - Awake - InventoryPopup");
    }
    public void UpdateItemButton(InventoryItemData data)
    {
        clicked = false;
        entered = false;
        item = data;
    }

    public void InitButton()
    {
        item = null;
    }
}
