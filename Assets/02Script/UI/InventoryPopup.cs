using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryPopup : MonoBehaviour
{
    private Transform player;
    private Transform p_onehand;
    private Transform p_shield;
    private Transform p_ranged;
    private Transform p_helmet;

    private TextMeshProUGUI coin;
    private Transform i_onehand;
    private Transform i_shield;
    private Transform i_bow;
    private Transform i_wand;
    private Transform i_helmet;
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

    private void Awake()
    {
        transform.LeanScale(Vector3.zero, 0);
        if (!transform.Find("CharBack/Character/InventoryPlayer").TryGetComponent<Transform>(out player))
            Debug.Log("InventoryPopupManager -Awake - Transform");

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
        Transform pointer = transform.Find("InvenBack/InvenView/Viewport/Content");
        for(int i=0; i < 29; i++)
        {
            itemButtons.Add(pointer.GetChild(i).GetComponent<ItemButton>());
        }

        if (player != null)
        {
            if (!player.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigNeck/RigHead/Dummy Prop Head/Helmet").TryGetComponent<Transform>(out p_helmet))
                Debug.Log("InventoryPopupManager -Awake - GameObject");

            if (!player.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/Dummy Prop Weapon Back/RangedWeapon/Ranged").TryGetComponent<Transform>(out p_ranged))
                Debug.Log("InventoryPopupManager -Awake - GameObject");

            if (!player.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigLArm1/RigLArm2/RigLArmPalm/Dummy Prop Left/Shield/Shield").TryGetComponent<Transform>(out p_shield))
                Debug.Log("InventoryPopupManager -Awake - GameObject");

            if (!player.Find("RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigRArm1/RigRArm2/RigRArmPalm/Dummy Prop Right/OneHand/OneHand").TryGetComponent<Transform>(out p_onehand))
                Debug.Log("InventoryPopupManager -Awake - GameObject");
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

        if (!transform.Find("CharBack/OnehandItem/Onehand").TryGetComponent<Transform>(out i_onehand))
            Debug.Log("InventoryPopupManager -Awake - Transform");

        if (!transform.Find("CharBack/ShieldItem/Shield").TryGetComponent<Transform>(out i_shield))
            Debug.Log("InventoryPopupManager -Awake - Transform");

        if (!transform.Find("CharBack/RangedItem/Bow").TryGetComponent<Transform>(out i_bow))
            Debug.Log("InventoryPopupManager -Awake - Transform"); 
        if (!transform.Find("CharBack/RangedItem/Wand").TryGetComponent<Transform>(out i_wand))
            Debug.Log("InventoryPopupManager -Awake - Transform");

        if (!transform.Find("CharBack/HelmetItem/Helmet").TryGetComponent<Transform>(out i_helmet))
            Debug.Log("InventoryPopupManager -Awake - Transform");

        if (!transform.Find("CharBack/ProjectileItem/Amount").TryGetComponent<TextMeshProUGUI>(out i_projectile))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");

        if (!transform.Find("CharBack/HpPotionItem/Amount").TryGetComponent<TextMeshProUGUI>(out i_HpP))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");

        if (!transform.Find("CharBack/StaminaPotionItem/Amount").TryGetComponent<TextMeshProUGUI>(out i_StaminaP))
            Debug.Log("InventoryPopupManager -Awake - TextMeshProUGUI");
    }

    public void UpdatePlayer()
    {
        coin.text = GameManager.Inst.PlayerCoin.ToString();
        playerDEF.text = GameManager.Inst.PlayerInfo.DEF.ToString();

        GameObject obj;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        bool enchant;
        ParticleSystem ps;

        try
        {
            obj = Resources.Load<GameObject>(GameManager.Inst.PlayerInfo.onehand.resources);
            meshFilter = obj.GetComponent<MeshFilter>();
            meshRenderer = obj.GetComponent<MeshRenderer>();
            enchant = GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_onehand);
            p_onehand.gameObject.SetActive(true);
            p_onehand.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            p_onehand.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            i_onehand.gameObject.SetActive(true);
            i_onehand.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            i_onehand.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            if (enchant)
            {
                p_onehand.GetChild(0).gameObject.SetActive(true);
                ps = p_onehand.GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = meshFilter.sharedMesh;
                //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
                i_onehand.GetChild(0).gameObject.SetActive(true);
                ps = i_onehand.GetChild(0).GetComponent<ParticleSystem>();
                sh = ps.shape;
                sh.mesh = meshFilter.sharedMesh;
                //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
            }
            else
            {
                p_onehand.GetChild(0).gameObject.SetActive(false);
                i_onehand.GetChild(0).gameObject.SetActive(false);
            }
        }
        catch (NullReferenceException e)
        {
            p_onehand.gameObject.SetActive(false);
            i_onehand.gameObject.SetActive(false);
            i_onehand.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            i_onehand.parent.GetComponent<WearingItemButton>().InitButton();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            obj = Resources.Load<GameObject>(GameManager.Inst.PlayerInfo.shield.resources);
            meshFilter = obj.GetComponent<MeshFilter>();
            meshRenderer = obj.GetComponent<MeshRenderer>();
            enchant = GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_shield);
            p_shield.gameObject.SetActive(true);
            p_shield.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            p_shield.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            i_shield.gameObject.SetActive(true);
            i_shield.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            i_shield.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            if (enchant)
            {
                p_shield.GetChild(0).gameObject.SetActive(true);
                ps = p_shield.GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = meshFilter.sharedMesh;
                //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
                i_shield.GetChild(0).gameObject.SetActive(true);
                ps = i_shield.GetChild(0).GetComponent<ParticleSystem>();
                sh = ps.shape;
                sh.mesh = meshFilter.sharedMesh;
                //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
            }
            else
            {
                p_shield.GetChild(0).gameObject.SetActive(false);
                i_shield.GetChild(0).gameObject.SetActive(false);
            }
        }
        catch (NullReferenceException e)
        {
            p_shield.gameObject.SetActive(false);
            i_shield.gameObject.SetActive(false);
            i_shield.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            i_shield.parent.GetComponent<WearingItemButton>().InitButton();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            obj = Resources.Load<GameObject>(GameManager.Inst.PlayerInfo.ranged.resources);
            meshFilter = obj.GetComponent<MeshFilter>();
            meshRenderer = obj.GetComponent<MeshRenderer>();
            enchant = GameManager.Inst.INVENTORY.GetItemEnchant(GameManager.Inst.PlayerInfo.i_ranged);
            p_ranged.gameObject.SetActive(true);
            p_ranged.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            p_ranged.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            if (enchant)
            {
                p_ranged.GetChild(0).gameObject.SetActive(true);
                ps = p_ranged.GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = meshFilter.sharedMesh;
                //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
            }
            else
            {
                p_ranged.GetChild(0).gameObject.SetActive(false);
            }

            if (GameManager.Inst.PlayerInfo.ranged.type == 3)
            {
                i_bow.gameObject.SetActive(true);
                i_bow.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
                i_bow.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
                i_wand.gameObject.SetActive(false);
                if (enchant)
                {
                    i_bow.GetChild(0).gameObject.SetActive(true);
                    ps = i_bow.GetChild(0).GetComponent<ParticleSystem>();
                    var sh = ps.shape;
                    sh.mesh = meshFilter.sharedMesh;
                    //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
                }
                else
                {
                    i_bow.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                i_wand.gameObject.SetActive(true);
                i_wand.gameObject.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
                i_wand.gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
                i_bow.gameObject.SetActive(false);
                if (enchant)
                {
                    i_wand.GetChild(0).gameObject.SetActive(true);
                    ps = i_wand.GetChild(0).GetComponent<ParticleSystem>();
                    var sh = ps.shape;
                    sh.mesh = meshFilter.sharedMesh;
                    //sh.texture = meshRenderer.sharedMaterial.GetTexture(0) as Texture2D;
                }
                else
                {
                    i_wand.GetChild(0).gameObject.SetActive(false);
                }
            }

        }
        catch (NullReferenceException e)
        {
            p_ranged.gameObject.SetActive(false);
            i_bow.gameObject.SetActive(false);
            i_wand.gameObject.SetActive(false);
            i_bow.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            i_bow.parent.GetComponent<WearingItemButton>().InitButton();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            obj = Resources.Load<GameObject>(GameManager.Inst.PlayerInfo.helmet.resources);
            p_helmet.gameObject.SetActive(true);
            p_helmet.gameObject.GetComponent<MeshFilter>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            p_helmet.gameObject.GetComponent<MeshRenderer>().sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
            i_helmet.gameObject.SetActive(true);
            i_helmet.gameObject.GetComponent<MeshFilter>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            i_helmet.gameObject.GetComponent<MeshRenderer>().sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
            i_helmet.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text
                = GameManager.Inst.PlayerInfo.helmet.DEF.ToString();

        }
        catch (NullReferenceException e)
        {
            p_helmet.gameObject.SetActive(false);
            i_helmet.gameObject.SetActive(false);
            i_helmet.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            i_helmet.parent.GetComponent<WearingItemButton>().InitButton();
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
        TableEntity_Weapon weapondata;
        TableEntity_Item Itemdata;
        GameObject itemMeshdata;

        int popupIndex = 0;
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
                
                if (inven[i].uid == GameManager.Inst.PlayerInfo.i_onehand)
                {
                    i_onehand.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = inven[i].durability.ToString();
                    i_onehand.parent.GetComponent<WearingItemButton>().UpdateItemButton(inven[i]);
                }
                else if (inven[i].uid == GameManager.Inst.PlayerInfo.i_shield)
                {
                    i_shield.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = inven[i].durability.ToString();
                    i_shield.parent.GetComponent<WearingItemButton>().UpdateItemButton(inven[i]);
                }
                else if (inven[i].uid == GameManager.Inst.PlayerInfo.i_ranged)
                {
                    i_bow.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = inven[i].durability.ToString();
                    i_bow.parent.GetComponent<WearingItemButton>().UpdateItemButton(inven[i]);
                }
            }
            else // item
            {
                itemButtons[popupIndex].UpdateItemButton(inven[i]);

                if (inven[i].uid == GameManager.Inst.PlayerInfo.i_helmet)
                {
                    i_helmet.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Inst.PlayerInfo.helmet.DEF.ToString();
                    i_helmet.parent.GetComponent<WearingItemButton>().UpdateItemButton(inven[i]);
                }
            }
        }
        for (int i = popupIndex; i < 30; i++)
        {
            itemButtons[popupIndex].InitButton();
        }
        if(SceneManager.GetActiveScene().buildIndex != 3)
        {
            Time.timeScale = 0;
        }
    }

    public void ShowNamePopup(string name)
    {
        namePopup.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
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
        weaponPopup.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = weapon.ATK.ToString();
        weaponPopup.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = data.durability.ToString();
        weaponPopup.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = "/ " + weapon.durability.ToString();
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
        weaponPopup.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = weapon.ATK.ToString();
        weaponPopup.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = data.durability.ToString();
        weaponPopup.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = "/ " + weapon.durability.ToString();
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
