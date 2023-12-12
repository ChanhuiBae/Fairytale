using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponBoxType
{
    Buy,
    Sell,
    Enchant,
    Fix
}
public class WeaponBox : MonoBehaviour
{
    private SmithyPopup smithyPopup;
    private Button button;
    private Image weapon;
    private Image use;
    private Image enchant;
    private Image chose;
    private TextMeshProUGUI ATKValue;
    private TextMeshProUGUI durabilityValue;
    private TextMeshProUGUI priceValue;

    private InventoryItemData item;
    private int price;

    private WeaponBoxType type;

    private void Awake()
    {
        if (!GameObject.Find("SmithyPopup").TryGetComponent<SmithyPopup>(out smithyPopup))
            Debug.Log("WeaponBox - Awake - Image");
        if (!transform.Find("Button").TryGetComponent<Button>(out button))
            Debug.Log("WeaponBox - Awake - Button");
        {
            button.onClick.AddListener(OnClick);
        }
        if (!transform.Find("Background/Weapon").TryGetComponent<Image>(out weapon))
            Debug.Log("WeaponBox - Awake - Image"); 
        if (!transform.Find("Use").TryGetComponent<Image>(out use))
            Debug.Log("WeaponBox - Awake - Image"); 
        if (!transform.Find("Enchant").TryGetComponent<Image>(out enchant))
            Debug.Log("WeaponBox - Awake - Image");
        if (!transform.Find("Chose").TryGetComponent<Image>(out chose))
            Debug.Log("WeaponBox - Awake - Image");
        else
        {
            chose.enabled = false;
        }
        if (!transform.Find("ATK").TryGetComponent<TextMeshProUGUI>(out ATKValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("Durability").TryGetComponent<TextMeshProUGUI>(out durabilityValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("Price").TryGetComponent<TextMeshProUGUI>(out priceValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");

        item = new InventoryItemData();
        item.uid = -1;
        item.enchant = false;
    }

    public void SetWeaponBox(InventoryItemData item, WeaponBoxType type)
    {
        TableEntity_Weapon weaponInfo;
        GameManager.Inst.GetWeaponData(item.itemID, out weaponInfo); 
        GameObject obj = Resources.Load<GameObject>(weaponInfo.resources);
        this.type = type;
        chose.enabled = false;
        switch (type)
        {
            case WeaponBoxType.Buy:
                priceValue.text = weaponInfo.buyCoin.ToString();
                price = weaponInfo.buyCoin;
                ATKValue.text = weaponInfo.ATK.ToString();
                durabilityValue.text = item.durability.ToString();
                use.enabled = false;
                enchant.enabled = false;
                break;
            case WeaponBoxType.Sell:
                priceValue.text = weaponInfo.sellCoin.ToString();
                price = weaponInfo.sellCoin;
                use.enabled = false;
                if (item.enchant)
                {
                    ATKValue.text = (weaponInfo.ATK * 1.2f).ToString();
                    enchant.enabled = true;
                }
                else
                {
                    ATKValue.text = weaponInfo.ATK.ToString();
                    enchant.enabled = false;
                }
                durabilityValue.text = item.durability.ToString();
                break;
            case WeaponBoxType.Enchant:
                TableEntity_Enchant enchantInfo;
                GameManager.Inst.GetEnchantData(item.itemID, out enchantInfo);
                ATKValue.text = weaponInfo.ATK.ToString();
                priceValue.text = enchantInfo.coin.ToString();
                price = enchantInfo.coin;
                durabilityValue.text = item.durability.ToString();
                if(item.uid == GameManager.Inst.PlayerInfo.i_onehand 
                    || item.uid == GameManager.Inst.PlayerInfo.i_shield
                    || item.uid == GameManager.Inst.PlayerInfo.i_ranged)
                {
                    use.enabled = true;
                }
                else
                {
                    use.enabled= false;
                }
                    break;
            case WeaponBoxType.Fix:
                priceValue.text = weaponInfo.fixCoin.ToString();
                
                price = weaponInfo.fixCoin;
                if (item.enchant)
                {
                    ATKValue.text = (weaponInfo.ATK * 1.2f).ToString();
                }
                else
                {
                    ATKValue.text = weaponInfo.ATK.ToString();
                }
                if (item.uid == GameManager.Inst.PlayerInfo.i_onehand
                    || item.uid == GameManager.Inst.PlayerInfo.i_shield
                    || item.uid == GameManager.Inst.PlayerInfo.i_ranged)
                {
                    use.enabled = true;
                }
                else
                {
                    use.enabled = false;
                }
                durabilityValue.text = item.durability.ToString();
                break;
        }

        if (this.item.uid == -1 || this.item.uid != item.uid)
        {
            this.item.uid = item.uid;
            this.item.itemID = item.itemID;
            this.item.type = item.type;
            this.item.durability = item.durability;
            this.item.enchant = item.enchant;
            this.item.amount = item.amount;

            TableEntity_Weapon data; 
            GameManager.Inst.GetWeaponData(item.itemID, out data);
            weapon.sprite = Resources.Load<Sprite>(data.iconResources);
        }
        else if(this.item.uid == item.uid)
        {
            if(this.item.enchant != item.enchant)
            {
                this.item.enchant = item.enchant;
            }
        }
    }

    private void OnClick()
    {
        if (chose.enabled)
        {
            chose.enabled = false;
            switch (type)
            {
                case WeaponBoxType.Buy:
                    smithyPopup.RemoveBuy(price, item);
                    break;
                case WeaponBoxType.Sell:
                    smithyPopup.RemoveSell(price, item);
                    break;
                case WeaponBoxType.Enchant:
                    smithyPopup.ResetEnchat();
                    break;
                case WeaponBoxType.Fix:
                    smithyPopup.ResetFix();
                    break;
            }
        }
        else
        {
            chose.enabled = true;
            switch (type)
            {
                case WeaponBoxType.Buy:
                    smithyPopup.AddBuy(price, item);
                    break;
                case WeaponBoxType.Sell:
                    smithyPopup.AddSell(price, item);
                    break;
                case WeaponBoxType.Enchant:
                    if (this.item.enchant)
                    {
                        smithyPopup.ResetEnchat();
                        smithyPopup.UnchoseOthers(gameObject.name[name.Length - 1] - 48);
                    }
                    else
                    {
                        smithyPopup.ChangeEnchant(price, item);
                        smithyPopup.UnchoseOthers(gameObject.name[name.Length - 1] - 48);
                    }
                    break;
                case WeaponBoxType.Fix:
                    smithyPopup.ChangeFix(price, item);
                    smithyPopup.UnchoseOthers(gameObject.name[name.Length - 1] -48);
                    break;
            }
        }
    }

    public void Unchose()
    {
        chose.enabled = false;
    }
}
