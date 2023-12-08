using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SmithyPopup : MonoBehaviour
{
    private Button closeButton;
    private Transform boxPointer;
    private List<WeaponBox> boxes;
    private Transform buyPopup;
    private Transform sellPopup;
    private Transform enchantPopup;
    private Transform fixPopup; 
    private Transform tryPopup;
    private Transform successPopup;
    private Transform failPopup;
    private Button buyButton;
    private Button sellButton;
    private Button enchantButton;
    private Button fixButton;
    private Button buyListButton;
    private Button sellListButton;
    private Button enchantListButton;
    private Button fixListButton;
    private Transform buySword;
    private Transform sellSword;
    private Transform enchantSword;
    private Transform fixSword;
    
    private List<MeshFilter> enchantItemMesh;
    private List<MeshRenderer> enchantItemMaterial;

    private TextMeshProUGUI buyCoin;
    private TextMeshProUGUI sellCoin;
    private TextMeshProUGUI enchantCoin;
    private TextMeshProUGUI fixCoin;
    private TextMeshProUGUI enchantATK;
    private TextMeshProUGUI enchantDurability;
    private TextMeshProUGUI enchantItemValue;
    private TextMeshProUGUI enchantPercent;
    private TextMeshProUGUI fixDurability;

    private List<InventoryItemData> inventory;
    private List<InventoryItemData> buyList;

    private Image tryFill;

    private int price;
    private int weaponCount;
    private List<InventoryItemData> choseList;
    TableEntity_Enchant enchantInfo;
    private int chose;

    private void Awake()
    {
        transform.LeanScale(Vector3.zero, 0f);
        buyList = new List<InventoryItemData>();
        choseList = new List<InventoryItemData>();
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
            if (!buyPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out buyCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!buyPopup.GetChild(0).TryGetComponent<Button>(out buyButton))
                Debug.Log("SmithyPopup - Awake - Button");
            else
            {
                buyButton.onClick.AddListener(Buy);
            }
        }
        if (!transform.Find("SellPopup").TryGetComponent<Transform>(out sellPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            if (!sellPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out sellCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!sellPopup.GetChild(0).TryGetComponent<Button>(out sellButton))
                Debug.Log("SmithyPopup - Awake - Button");
            else
            {
                sellButton.onClick.AddListener(Sell);
            }
        }
        if (!transform.Find("EnchantPopup").TryGetComponent<Transform>(out enchantPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            enchantItemMesh = new List<MeshFilter>();
            enchantItemMaterial = new List<MeshRenderer>();
            for(int i = 0; i < 2 ; i++)
            {
                enchantItemMesh.Add(enchantPopup.Find("Item").GetChild(i).GetComponent<MeshFilter>());
                enchantItemMaterial.Add(enchantPopup.Find("Item").GetChild(i).GetComponent<MeshRenderer>());
            }
            if (!enchantPopup.Find("ItemValue").TryGetComponent<TextMeshProUGUI>(out enchantItemValue))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out enchantCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.Find("ATK").TryGetComponent<TextMeshProUGUI>(out enchantATK))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.Find("Durability").TryGetComponent<TextMeshProUGUI>(out enchantDurability))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.Find("Percent").TryGetComponent<TextMeshProUGUI>(out enchantPercent))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!enchantPopup.GetChild(0).TryGetComponent<Button>(out enchantButton))
                Debug.Log("SmithyPopup - Awake - Button");
            else
            {
                enchantButton.onClick.AddListener(TryEnchant);
            }
        }
        if (!transform.Find("FixPopup").TryGetComponent<Transform>(out fixPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            if (!fixPopup.Find("Coin").TryGetComponent<TextMeshProUGUI>(out fixCoin))
                Debug.Log("SmithyPopup - Awake - TextMeshProUGUI");
            if (!fixPopup.Find("Durability").TryGetComponent<TextMeshProUGUI>(out fixDurability))
                Debug.Log("SmithyPopup - Awake -  TextMeshProUGUI");
            if (!fixPopup.GetChild(0).TryGetComponent<Button>(out fixButton))
                Debug.Log("SmithyPopup - Awake - Button");
            else
            {
                fixButton.onClick.AddListener(Fix);
            }
        }
        if (!transform.Find("TryPopup").TryGetComponent<Transform>(out tryPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            if (!tryPopup.Find("TryFill").TryGetComponent<Image>(out tryFill))
                Debug.Log("SmithyPopup - Awake - Image");
            tryPopup.gameObject.SetActive(false);
        }
        if (!transform.Find("SuccessPopup").TryGetComponent<Transform>(out successPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            successPopup.gameObject.SetActive(false);
        }
        if (!transform.Find("FailPopup").TryGetComponent<Transform>(out failPopup))
            Debug.Log("SmithyPopup - Awake - Transform");
        else
        {
            failPopup.gameObject.SetActive(false);
        }

        if (!transform.Find("BuyListButton").TryGetComponent<Button>(out buyListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        else
        {
            buyListButton.onClick.AddListener(InitBuyPopup);
        }
        if (!transform.Find("SellListButton").TryGetComponent<Button>(out sellListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        else
        {
            sellListButton.onClick.AddListener(InitSellPopup);
        }
        if (!transform.Find("EnchantListButton").TryGetComponent<Button>(out enchantListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        else
        {
            enchantListButton.onClick.AddListener(InitEnchantPopup);
        }
        if (!transform.Find("FixListButton").TryGetComponent<Button>(out fixListButton))
            Debug.Log("SmithyPopup - Awake - Button");
        else
        {
            fixListButton.onClick.AddListener(InitFixPopup);
        }
        if (!transform.Find("BuySword").TryGetComponent<Transform>(out buySword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("SellSword").TryGetComponent<Transform>(out sellSword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("EnchantSword").TryGetComponent<Transform>(out enchantSword))
            Debug.Log("SmithyPopup - Awake - Transform");
        if (!transform.Find("FixSword").TryGetComponent<Transform>(out fixSword))
            Debug.Log("SmithyPopup - Awake - Transform");
    }

    public void InitSmithyPopup()
    {
        transform.LeanScale(Vector3.one, 0f);
        InitFirstBuyPopup();
        chose = -1;
    }

    private void InitFirstBuyPopup()
    {
        buySword.LeanMoveLocalX(-682f, 1);
        sellPopup.gameObject.SetActive(false);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(false);
        price = 0;
        buyCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Clear();

        for(int i = 0; i < 6; i++)
        {
            InventoryItemData  item = new InventoryItemData();
            item.uid = -7 + i;
            TableEntity_Weapon sellWeapon = GameManager.Inst.GetCanBuyWeapon();
            item.itemID = sellWeapon.uid;
            item.enchant = false;
            item.amount = 1;
            item.durability = sellWeapon.durability;
            item.type = sellWeapon.type;
            buyList.Add(item);
            boxes[i].SetWeaponBox(item, WeaponBoxType.Buy);
        }
        for (int i = 6; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }

    private void InitBuyPopup()
    {
        buySword.LeanMoveLocalX(-682f, 1);
        sellSword.LeanMoveLocalX(-710f, 1);
        enchantSword.LeanMoveLocalX(-710f, 1);
        fixSword.LeanMoveLocalX(-710f, 1);
        sellPopup.gameObject.SetActive(false);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(false);
        price = 0;
        buyCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Clear();

        for (int i = 0; i < buyList.Count; i++)
        {
            boxes[i].gameObject.SetActive(true);
            boxes[i].SetWeaponBox(buyList[i], WeaponBoxType.Buy);
        }
        for (int i = buyList.Count; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }


    private void InitSellPopup()
    {
        inventory = GameManager.Inst.INVENTORY.GetItemList();
        buySword.LeanMoveLocalX(-710f, 1);
        sellSword.LeanMoveLocalX(-682f, 1);
        enchantSword.LeanMoveLocalX(-710f, 1);
        fixSword.LeanMoveLocalX(-710f, 1);
        sellPopup.gameObject.SetActive(true);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(false);
        price = 0;
        sellCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Clear();

        weaponCount = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemID < 20000
                && inventory[i].itemID != 10501
                && inventory[i].itemID != GameManager.Inst.PlayerInfo.onehand.uid
                && inventory[i].itemID != GameManager.Inst.PlayerInfo.shield.uid
                && inventory[i].itemID != GameManager.Inst.PlayerInfo.ranged.uid)
            {
                boxes[weaponCount].gameObject.SetActive(true);
                boxes[weaponCount].SetWeaponBox(inventory[i], WeaponBoxType.Sell);
                weaponCount++;
            }
        }
        for (int i = weaponCount; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }


    private void InitEnchantPopup()
    {
        enchantButton.enabled = false;
        inventory = GameManager.Inst.INVENTORY.GetItemList(); 
        enchantItemMesh[0].transform.parent.gameObject.SetActive(false);
        buySword.LeanMoveLocalX(-710f, 1);
        sellSword.LeanMoveLocalX(-710f, 1);
        enchantSword.LeanMoveLocalX(-682f, 1);
        fixSword.LeanMoveLocalX(-710f, 1);
        enchantPopup.gameObject.SetActive(true);
        fixPopup.gameObject.SetActive(false);
        chose = -1;
        choseList.Clear();
        enchantCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = " -> ";
        enchantDurability.text =" -> ";
        enchantPercent.text = "";
        enchantItemValue.text = "";

        weaponCount = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemID < 20000 && inventory[i].itemID != 10501 && !inventory[i].enchant)
            {
                boxes[weaponCount].gameObject.SetActive(true);
                boxes[weaponCount].SetWeaponBox(inventory[i], WeaponBoxType.Enchant);
                weaponCount++;
            }
        }
        for (int i = weaponCount; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }

    
    private void InitFixPopup()
    {
        inventory = GameManager.Inst.INVENTORY.GetItemList();
        buySword.LeanMoveLocalX(-710f, 1);
        sellSword.LeanMoveLocalX(-710f, 1);
        enchantSword.LeanMoveLocalX(-710f, 1);
        fixSword.LeanMoveLocalX(-682f, 1);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(true);
        price = 0;
        choseList.Clear();
        chose = -1;
        fixCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        fixDurability.text = " -> ";

        weaponCount = 0;
        TableEntity_Weapon weapon;
        for (int i = 0; i < inventory.Count; i++)
        {
            GameManager.Inst.GetWeaponData(inventory[i].itemID, out weapon);
            if (inventory[i].itemID < 20000 && inventory[i].itemID != 10501
                && inventory[i].durability != weapon.durability)
            {
                boxes[weaponCount].gameObject.SetActive(true);
                boxes[weaponCount].SetWeaponBox(inventory[i], WeaponBoxType.Fix);
                weaponCount++;
            }
        }
        for (int i = weaponCount; i < 30; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
    }

    public void AddBuy(int value, InventoryItemData item)
    {
        price += value;
        buyCoin.text = price.ToString() + " / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Add(item);
        if (price <= GameManager.Inst.PlayerCoin)
            buyButton.enabled = true;
        else
            buyButton.enabled = false;
    }

    public void RemoveBuy(int value, InventoryItemData item)
    {
        price -= value;
        buyCoin.text = price.ToString() + " / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Remove(item);
    }
    public void AddSell(int value, InventoryItemData item)
    {
        price += value;
        sellCoin.text = price.ToString();
        choseList.Add(item);
    }
    public void RemoveSell(int value, InventoryItemData item)
    {
        price -= value;
        sellCoin.text = price.ToString();
        choseList.Remove(item);
    }

    public void ChangeEnchant(int price, InventoryItemData item)
    {
        enchantItemMesh[0].transform.parent.gameObject.SetActive(true);
        choseList.Add(item);
        GameManager.Inst.GetEnchantData(item.itemID, out enchantInfo);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(item.itemID, out weapon);
        TableEntity_Item needItem;
        GameManager.Inst.GetItemData(enchantInfo.itemID, out needItem);
        GameObject obj = Resources.Load<GameObject>(needItem.resources);
        if(needItem.type == 6)
        {
            enchantItemMesh[0].gameObject.SetActive(true);
            enchantItemMesh[1].gameObject.SetActive(false);
            enchantItemMesh[0].sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            enchantItemMaterial[0].sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
        }
        else if(needItem.type == 7)
        {
            enchantItemMesh[0].gameObject.SetActive(false);
            enchantItemMesh[1].gameObject.SetActive(true);
            enchantItemMesh[1].sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            enchantItemMaterial[1].sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
        }

        enchantCoin.text = price.ToString() + " / " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = weapon.ATK.ToString() + " -> " + (weapon.ATK *1.2f).ToString();
        enchantDurability.text = item.durability.ToString() + " - > " + weapon.durability.ToString();
        enchantPercent.text = enchantInfo.rate.ToString() + "%";
        int amount = GameManager.Inst.INVENTORY.GetItemAmount(needItem.uid);
        enchantItemValue.text = enchantInfo.amount.ToString() + " / " + amount;
        if (price <= GameManager.Inst.PlayerCoin && amount >= enchantInfo.amount)
            enchantButton.enabled = true;
        else 
            enchantButton.enabled = false;
    }

    public void ResetEnchat()
    {
        choseList.Clear();
        enchantItemMesh[0].transform.parent.gameObject.SetActive(false);
        enchantCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = " -> ";
        enchantDurability.text = " -> ";
        enchantPercent.text = "";
        enchantItemValue.text = "";
    }

    public void ChangeFix(int price, InventoryItemData item)
    {
        choseList.Add(item);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(item.itemID, out weapon);
        
        fixCoin.text = price.ToString() + " / " + GameManager.Inst.PlayerCoin.ToString();
        fixDurability.text = item.durability.ToString() + " -> " + weapon.durability.ToString();
        if (price <= GameManager.Inst.PlayerCoin)
            fixButton.enabled = true;
        else
            fixButton.enabled = false;
    }

    public void ResetFix()
    {
        choseList.Clear();
        fixCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        fixDurability.text = " -> ";
    }

    public void UnchoseOthers(int index)
    {
        if(chose != -1 && chose != index)
        {
            boxes[chose].Unchose();
        }
        chose = index;
    }

    private void Buy()
    {
        GameManager.Inst.PlayerCoin -= price;
        for(int i = choseList.Count - 1; i > -1; i--)
        {
            buyList.Remove(choseList[i]);
            GameManager.Inst.INVENTORY.AddWeapon(choseList[i]);
        }
        InitBuyPopup();
    }

    private void Sell()
    {
        GameManager.Inst.PlayerCoin += price;
        for (int i = choseList.Count - 1; i > -1; i--)
        {
            GameManager.Inst.INVENTORY.DeleteItem(choseList[i]);
        }
        InitSellPopup();
    }

    private void TryEnchant()
    {
        enchantPopup.gameObject.SetActive(false);
        tryPopup.gameObject.SetActive(true);
        GameManager.Inst.PlayerCoin -= price;
        GameManager.Inst.INVENTORY.DeleteItemAmount(enchantInfo.itemID, enchantInfo.amount);
        int i = Random.Range(0,99);
        if (i < enchantInfo.rate)
        {
            GameManager.Inst.INVENTORY.EnchantItem(choseList[0].uid);
            StartCoroutine(ShowEnchantResult(i, true));
        }
        else
        {
            StartCoroutine(ShowEnchantResult(i, false));
        }
    }

    private IEnumerator ShowEnchantResult(float value, bool result)
    {
        float i = 0;
        if (result)
        {
            while (i <= 1)
            {
                yield return YieldInstructionCache.WaitForSeconds(0.1f);
                i += 0.01f;
                tryFill.fillAmount = i;
            }
            successPopup.gameObject.SetActive(true);
            tryPopup.gameObject.SetActive(false);
            yield return YieldInstructionCache.WaitForSeconds(1);
            successPopup.gameObject.SetActive(false);
        }
        else
        {
            value = 1 - value;
            value /= 100;
            while (i <= value)
            {
                yield return YieldInstructionCache.WaitForSeconds(0.1f);
                i += 0.01f;
                tryFill.fillAmount = i;
            }
            failPopup.gameObject.SetActive(true);
            tryPopup.gameObject.SetActive(false);
            yield return YieldInstructionCache.WaitForSeconds(1);
            failPopup.gameObject.SetActive(false);
        }
        InitEnchantPopup();
    }

    private void Fix()
    {
        GameManager.Inst.INVENTORY.FixItem(choseList[0].uid);
        InitFixPopup();
    }

    public void CloseSmithyPopup()
    {
        transform.LeanScale(Vector3.zero, 0f);
        GameManager.Inst.PlayerIsController(true);
    }
}
