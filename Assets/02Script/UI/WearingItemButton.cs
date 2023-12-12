using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WearingItemButton : MonoBehaviour
{
    private InventoryItemData item;
    private EventTrigger ET;
    private EventTrigger.Entry entry_click;

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
        }
    }

    private IEnumerator DublieClick()
    {
        clicked = true;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
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
