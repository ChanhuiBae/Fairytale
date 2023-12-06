using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    private InventoryItemData item;
    private EventTrigger ET;
    private EventTrigger.Entry entry_click;
    private EventTrigger.Entry entry_down;
    private EventTrigger.Entry entry_up;
    private Image background;
    private Color color;
    private List<MeshFilter> mesh; 
    private List<MeshRenderer> material;

    private TableEntity_Item itemdata;
    private TableEntity_Weapon weapondata;

    private TextMeshProUGUI info;
    private bool clicked;
    private bool entered;

    private InventoryPopup inventoryPopup;

    void OnClick(PointerEventData data)
    {
        if(item != null)
        {
            if (clicked)
            {
                clicked = false;
                if (item.type < 6)
                {
                    background.color = Color.gray;
                    inventoryPopup.CloseWeaponPopup();
                    switch (item.type)
                    {
                        case 1:
                            if(item.uid == GameManager.Inst.PlayerInfo.i_onehand)
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
                            if(item.uid == GameManager.Inst.PlayerInfo.i_helmet)
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

    void OnDown(PointerEventData data)
    {
        if(item != null)
        {
            if(item.type < 5)
            {
                if(background.color == Color.gray)
                    inventoryPopup.ShowWeaponPopup(item);
                else
                    inventoryPopup.ShowDeleteWeaponPopup(item);
            }
            else if(item.type == 5)
            {
                inventoryPopup.ShowHelmetPopup(itemdata.name, itemdata.DEF);
            }
            else
            {
                inventoryPopup.ShowNamePopup(itemdata.name);
            }
        }
    }
    void OnUp(PointerEventData data)
    {
        if (item != null)
        {
            if (item.type == 5)
            {
                inventoryPopup.CloseHelmetPopup();
            }
            else
            {
                inventoryPopup.CloseNamePopup();
            }
        }
    }

    private void Awake()
    {
        entry_click = new EventTrigger.Entry();
        entry_down = new EventTrigger.Entry();
        entry_up = new EventTrigger.Entry();

        if (!TryGetComponent<EventTrigger>(out ET))
            Debug.Log("ItemButton - Awake - EventTrigger");
        else
        {
            entry_click.eventID = EventTriggerType.PointerClick;
            entry_click.callback.AddListener((data) => { OnClick((PointerEventData)data); });
            ET.triggers.Add(entry_click);
            entry_down.eventID = EventTriggerType.PointerEnter;
            entry_down.callback.AddListener((data) => { OnDown((PointerEventData)data); });
            ET.triggers.Add(entry_down);
            entry_up.eventID = EventTriggerType.PointerExit;
            entry_up.callback.AddListener((data) => { OnUp((PointerEventData)data); });
            ET.triggers.Add(entry_up);
        }
        if (!TryGetComponent<Image>(out background))
            Debug.Log("ItemButton - Awake - MeshRenderer");
        else
        {
            color = background.color;
        }

        mesh = new List<MeshFilter>();
        for(int i = 1; i < transform.childCount - 1; i++)
        {
            mesh.Add(transform.GetChild(i).GetComponent<MeshFilter>());
        }
        material = new List<MeshRenderer>();
        for (int i = 1; i < transform.childCount - 1; i++)
        {
            material.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }

        if (!transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out info))
            Debug.Log("ItemButton - Awake - TextMeshProUGUI");
        else
        {
            info.text = "";
        }

        for (int i = 1; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
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
            GameManager.Inst.GetWeaponData(item.itemID, out weapondata);
            obj = Resources.Load<GameObject>(weapondata.resources);
            info.text = item.durability.ToString();
            if (item.uid == GameManager.Inst.PlayerInfo.i_onehand)
            {
                background.color = Color.gray;
            }
            else if (item.uid == GameManager.Inst.PlayerInfo.i_shield)
            {
                background.color = Color.gray;
            }
            else if (item.uid == GameManager.Inst.PlayerInfo.i_ranged)
            {
                background.color = Color.gray;
            }
            else
            {
                background.color = color;
            }
        }
        else // item
        {
            GameManager.Inst.GetItemData(item.itemID, out itemdata);
            obj = Resources.Load<GameObject>(itemdata.resources);

            info.text = item.amount.ToString();

            if (item.uid == GameManager.Inst.PlayerInfo.i_helmet)
            {
                background.color = Color.gray;
            }
            else
            {
                background.color = color;
            }
        }
        for (int i = 1; i < transform.childCount-1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(item.type).gameObject.SetActive(true);
        mesh[item.type-1].sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        material[item.type - 1].sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
        bool enchant = GameManager.Inst.INVENTORY.GetItemEnchant(item.uid);
        if (enchant)
        {
            transform.GetChild(item.type).GetChild(0).gameObject.SetActive(true);
            ParticleSystem ps = transform.GetChild(item.type).GetChild(0).GetComponent<ParticleSystem>();
            var sh = ps.shape;
            sh.mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            //sh.texture = obj.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
        }
        else
        {
            if(transform.GetChild(item.type).childCount != 0)
            {
                transform.GetChild(item.type).GetChild(0).gameObject.SetActive(false);
            }
        }

    }

    public void InitButton()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        background.color = color;
        item = null;
        itemdata = null;
        weapondata = null;
    }
}
