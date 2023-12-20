using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class SmithyPopup : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform scrollContent;

    private Button closeButton;
    private Transform boxPointer;
    private List<WeaponBox> boxes;
    private Transform buyPopup;
    private Transform inventoryWarning;
    private Transform coinWarning;
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

    private TextMeshProUGUI buyCoin;
    private TextMeshProUGUI sellCoin;
    private TextMeshProUGUI enchantCoin;
    private TextMeshProUGUI fixCoin;
    private TextMeshProUGUI enchantATK;
    private TextMeshProUGUI enchantDurability;
    private TextMeshProUGUI enchantItemValue;
    private TextMeshProUGUI enchantPercent;
    private TextMeshProUGUI fixDurability;

    private Image itemImage;

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
        if (!transform.parent.TryGetComponent<Canvas>(out canvas))
            Debug.Log("SmithyPopup - Init - Canvas");

        if (!GameObject.Find("Contents").TryGetComponent<RectTransform>(out scrollContent))
            Debug.Log("SmithyPopup - Init - RectTransform");

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
            if (!buyPopup.Find("InventoryWarning").TryGetComponent<Transform>(out inventoryWarning))
                Debug.Log("SmithyPopup - Awake - Transform");
            else
                inventoryWarning.gameObject.SetActive(false);
            if (!buyPopup.Find("CoinWarning").TryGetComponent<Transform>(out coinWarning))
                Debug.Log("SmithyPopup - Awake - Transform");
            else
                coinWarning.gameObject.SetActive(false);    
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
            if (!enchantPopup.Find("ItemBackground/Item").TryGetComponent<Image>(out itemImage))
                Debug.Log("SmithyPopup - Awake - Image");
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

    public void SetRectPosition()
    {
        float x = scrollContent.anchoredPosition.x;
        scrollContent.anchoredPosition = new Vector3(x, 0, 0);
    }
    public void InitSmithyPopup()
    {
        transform.LeanScale(new Vector3(0.7f, 0.7f, 1), 0f);
        SetRectPosition();
        InitFirstBuyPopup();
        chose = -1;
    }

    private void InitFirstBuyPopup()
    {
        if (buyList == null || buyList.Count == 0)
        {
            buyList = new List<InventoryItemData>();
            for (int i = 0; i < 8; i++)
            {
                InventoryItemData item = new InventoryItemData();
                TableEntity_Weapon sellWeapon = GameManager.Inst.GetCanBuyWeapon();
                item.itemID = sellWeapon.uid;
                item.enchant = false;
                item.amount = 1;
                item.durability = sellWeapon.durability;
                item.type = sellWeapon.type;
                buyList.Add(item);
            }
        }
        InitBuyPopup();
    }

    private void InitBuyPopup()
    {
        buySword.LeanMoveLocalX(-430f, 0.5f);
        sellSword.LeanMoveLocalX(-445f, 0.5f);
        enchantSword.LeanMoveLocalX(-445f, 0.5f);
        fixSword.LeanMoveLocalX(-445f, 0.5f);
        sellPopup.gameObject.SetActive(false);
        enchantPopup.gameObject.SetActive(false);
        fixPopup.gameObject.SetActive(false);
        if (GameManager.Inst.INVENTORY.IsFull || (choseList.Count + 1) > GameManager.Inst.INVENTORY.DeltaSlotCount)
        {
            inventoryWarning.gameObject.SetActive(true);
        }
        else
        {
            inventoryWarning.gameObject.SetActive(false);
        }
        if (price > GameManager.Inst.PlayerCoin)
        {
            coinWarning.gameObject.SetActive(true);
        }
        else
        {
            coinWarning.gameObject.SetActive(false);
        }
        price = 0;
        buyCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Clear();

        for (int i = 0; i < buyList.Count; i++)
        {
            boxes[i].gameObject.SetActive(true);
            buyList[i].uid = -i -1;
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
        buySword.LeanMoveLocalX(-445f, 0.5f);
        sellSword.LeanMoveLocalX(-430f, 0.5f);
        enchantSword.LeanMoveLocalX(-445f, 0.5f);
        fixSword.LeanMoveLocalX(-445f, 0.5f);
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
                && inventory[i].uid != GameManager.Inst.PlayerInfo.i_onehand
                && inventory[i].uid != GameManager.Inst.PlayerInfo.i_shield
                && inventory[i].uid != GameManager.Inst.PlayerInfo.i_ranged)
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
        buySword.LeanMoveLocalX(-445f, 0.5f);
        sellSword.LeanMoveLocalX(-445f, 0.5f);
        enchantSword.LeanMoveLocalX(-430f, 0.5f);
        fixSword.LeanMoveLocalX(-445f, 0.5f);
        enchantPopup.gameObject.SetActive(true);
        fixPopup.gameObject.SetActive(false);
        chose = -1;
        choseList.Clear();
        enchantCoin.text = "0 / " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = " -> ";
        enchantDurability.text =" -> ";
        enchantPercent.text = "";
        enchantItemValue.text = "";
        itemImage.enabled = false;

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
        buySword.LeanMoveLocalX(-445f, 0.5f);
        sellSword.LeanMoveLocalX(-445f, 0.5f);
        enchantSword.LeanMoveLocalX(-445f, 0.5f);
        fixSword.LeanMoveLocalX(-430f, 0.5f);
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
        if(GameManager.Inst.INVENTORY.IsFull || (choseList.Count + 1) > GameManager.Inst.INVENTORY.DeltaSlotCount)
        {
            inventoryWarning.gameObject.SetActive(true);
        }
        if(price > GameManager.Inst.PlayerCoin)
        {
            coinWarning.gameObject.SetActive(true);
        }
        price += value;
        buyCoin.text = price.ToString() + " / " + GameManager.Inst.PlayerCoin.ToString();
        choseList.Add(item);
    }

    public void RemoveBuy(int value, InventoryItemData item)
    {
        if (!GameManager.Inst.INVENTORY.IsFull && (choseList.Count - 1) <= GameManager.Inst.INVENTORY.DeltaSlotCount)
        {
            inventoryWarning.gameObject.SetActive(false);
        }
        if (price <= GameManager.Inst.PlayerCoin)
        {
            coinWarning.gameObject.SetActive(false);
        }
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
        choseList.Add(item);
        GameManager.Inst.GetEnchantData(item.itemID, out enchantInfo);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(item.itemID, out weapon);
        TableEntity_Item needItem;
        GameManager.Inst.GetItemData(enchantInfo.itemID, out needItem);
        GameObject obj = Resources.Load<GameObject>(needItem.resources);

        enchantCoin.text = price.ToString() + "\n/ " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = weapon.ATK.ToString() + " -> " + (weapon.ATK *1.2f).ToString();
        enchantDurability.text = item.durability.ToString() + " - > " + weapon.durability.ToString();
        enchantPercent.text = enchantInfo.rate.ToString() + "%";
        int amount = GameManager.Inst.INVENTORY.GetItemAmount(needItem.uid);
        enchantItemValue.text = enchantInfo.amount.ToString() + "\n/ " + amount;
        itemImage.enabled = true;
        itemImage.sprite = Resources.Load<UnityEngine.Sprite>(needItem.iconResources);
        if (price <= GameManager.Inst.PlayerCoin && amount >= enchantInfo.amount)
            enchantButton.enabled = true;
        else 
            enchantButton.enabled = false;
    }

    public void ResetEnchat()
    {
        choseList.Clear();
        enchantCoin.text = "0\n/ " + GameManager.Inst.PlayerCoin.ToString();
        enchantATK.text = " -> ";
        enchantDurability.text = " -> ";
        enchantPercent.text = "";
        enchantItemValue.text = "";
        itemImage.enabled = false;
    }

    public void ChangeFix(int price, InventoryItemData item)
    {
        choseList.Add(item);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(item.itemID, out weapon);
        
        fixCoin.text = price.ToString() + "\n/ " + GameManager.Inst.PlayerCoin.ToString();
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
        List<InventoryItemData> deleteData = choseList.OrderBy(x => x.uid).ToList();
        for(int i = 0; i < choseList.Count; i++)
        {
            buyList.RemoveAt(-deleteData[i].uid - 1);
            GameManager.Inst.INVENTORY.BuyWeapon(choseList[i].itemID);
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
