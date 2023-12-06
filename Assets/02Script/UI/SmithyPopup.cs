using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class SmithyPopup : MonoBehaviour
{
    private Button closeButton;
    private Transform boxPointer;
    private List<WeaponBox> boxes;
    private Transform buyPopup;
    private Transform sellPopup;
    private Transform enchantPopup;
    private Transform fixPopup;
    private Button buyButton;
    private Button sellButton;
    private Button enchantButton;
    private Button fixButton;
    private Button buyListButton;
    private Button sellListButton;
    private Button enchantListButton;
    private Button fixListButton;
    private RectTransform buySword;
    private RectTransform sellSword;
    private RectTransform enchantSword;
    private RectTransform fixSword;

    private MeshFilter enchantWeaponMesh; 
    private MeshRenderer enchantWeaponMaterial;
    private MeshFilter enchantItemMesh;
    private MeshRenderer enchantItemMaterial;
    private MeshFilter fixWeaponMesh; 
    private MeshRenderer fixWeaponMaterial;
   

    private TextMeshProUGUI buyCoin;
    private TextMeshProUGUI sellCoin;
    private TextMeshProUGUI enchantCoin;
    private TextMeshProUGUI fixCoin;
    private TextMeshProUGUI enchantItemValue;

    private List<InventoryItemData> inventory;
    private List<TableEntity_Weapon> tableList;

    private void Awake()
    {
        transform.LeanScale(Vector3.zero, 0f);
        if (!transform.Find("CloseButton").TryGetComponent<Button>(out closeButton))
            Debug.Log("SmityPopup - Awake - Button");
        else
        {
            closeButton.onClick.AddListener(CloseSmithyPopup);
        }
        if (!GameObject.Find("WeaponBoxes").TryGetComponent<Transform>(out boxPointer))
            Debug.Log("SmityPopup - Awake - Transform");
        else
        {
            boxes = new List<WeaponBox>();
            for (int i = 0; i < 30; i++)
            {
                boxes.Add(boxPointer.GetChild(i).GetComponent<WeaponBox>());
            }
        }

        if (!transform.Find("BuyPopup").TryGetComponent<Transform>(out buyPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            buyPopup.LeanScale(Vector3.zero, 0f);
            if (!buyPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out buyCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!buyPopup.GetChild(0).TryGetComponent<Button>(out buyButton))
                Debug.Log("SmithyPopup - Awake - Button");
        }
        if (!transform.Find("SellPopup").TryGetComponent<Transform>(out sellPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            sellPopup.LeanScale(Vector3.zero, 0f);
            if (!sellPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out sellCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!sellPopup.GetChild(0).TryGetComponent<Button>(out sellButton))
                Debug.Log("SmithyPopup - Awake - Button");
        }
        if (!transform.Find("EnchantPopup").TryGetComponent<Transform>(out enchantPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            enchantPopup.LeanScale(Vector3.zero, 0f);
            if (!enchantPopup.Find("Weapon").TryGetComponent<MeshFilter>(out enchantWeaponMesh))
                Debug.Log("WeaponBox - Awake - MeshFilter");
            if (!enchantPopup.Find("Weapon").TryGetComponent<MeshRenderer>(out enchantWeaponMaterial))
                Debug.Log("WeaponBox - Awake - MeshRenderer");
            if (!enchantPopup.Find("Item").TryGetComponent<MeshFilter>(out enchantItemMesh))
                Debug.Log("WeaponBox - Awake - MeshFilter"); 
            if (!enchantPopup.Find("Item").TryGetComponent<MeshRenderer>(out enchantItemMaterial))
                Debug.Log("WeaponBox - Awake - MeshRenderer");
            if (!enchantPopup.Find("ItemValue").TryGetComponent<TextMeshProUGUI>(out enchantItemValue))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out enchantCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.GetChild(0).TryGetComponent<Button>(out enchantButton))
                Debug.Log("SmithyPopup - Awake - Button");
        }
        if (!transform.Find("FixPopup").TryGetComponent<Transform>(out fixPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            fixPopup.LeanScale(Vector3.zero, 0f);
            if (!fixPopup.Find("Weapon").TryGetComponent<MeshFilter>(out fixWeaponMesh))
                Debug.Log("WeaponBox - Awake - MeshFilter");
            if (!fixPopup.Find("Weapon").TryGetComponent<MeshRenderer>(out fixWeaponMaterial))
                Debug.Log("WeaponBox - Awake - MeshRenderer");
            if (!fixPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out fixCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!fixPopup.GetChild(0).TryGetComponent<Button>(out fixButton))
                Debug.Log("SmithyPopup - Awake - Button");
        }
        if (!transform.Find("BuyListButton").TryGetComponent<Button>(out buyListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        if (!transform.Find("SellListButton").TryGetComponent<Button>(out sellListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        if (!transform.Find("EnchantListButton").TryGetComponent<Button>(out enchantListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        if (!transform.Find("FixListButton").TryGetComponent<Button>(out fixListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        if (!transform.Find("BuySword").TryGetComponent<RectTransform>(out buySword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("SellSword").TryGetComponent<RectTransform>(out sellSword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("EnchantSword").TryGetComponent<RectTransform>(out enchantSword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("FixSword").TryGetComponent<RectTransform>(out fixSword))
            Debug.Log("SmithyPopup - Awake - Transform");
    }

    public void InitSmithyPopup()
    {
        transform.LeanScale(Vector3.one, 0f);
        InitBuyPopup();
    }

    public void InitBuyPopup()
    {
        sellPopup.gameObject.SetActive(false);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(false);
        for (int i = 6; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }


    public void InitSellPopup()
    {
        inventory = GameManager.Inst.INVENTORY.GetItemList();
    }


    public void InitEnchantPopup()
    {
        inventory = GameManager.Inst.INVENTORY.GetItemList();
    }

    
    public void InitFixPopup()
    {
        inventory = GameManager.Inst.INVENTORY.GetItemList();
    }


    
    public void CloseSmithyPopup()
    {
        transform.LeanScale(Vector3.zero, 0f);
        GameManager.Inst.PlayerIsController(true);
    }
}
