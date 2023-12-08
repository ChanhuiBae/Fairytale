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
    private Image background;
    private Image chose;
    private TextMeshProUGUI ATKValue;
    private TextMeshProUGUI durabilityValue;
    private TextMeshProUGUI priceValue;

    private Transform weapon;
    private List<MeshFilter> mesh;
    private List<MeshRenderer> material;
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
        if (!button.transform.TryGetComponent<Image>(out background))
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
        mesh = new List<MeshFilter>();
        material = new List<MeshRenderer>();
        weapon = transform.Find("Weapon");
        if (weapon != null)
        {
            mesh.Add(weapon.GetChild(0).GetComponent<MeshFilter>());
            material.Add(weapon.GetChild(0).GetComponent<MeshRenderer>());
            mesh.Add(weapon.GetChild(1).GetComponent<MeshFilter>());
            material.Add(weapon.GetChild(1).GetComponent<MeshRenderer>());
            mesh.Add(weapon.GetChild(2).GetComponent<MeshFilter>());
            material.Add(weapon.GetChild(2).GetComponent<MeshRenderer>());
            mesh.Add(weapon.GetChild(3).GetComponent<MeshFilter>());
            material.Add(weapon.GetChild(3).GetComponent<MeshRenderer>());
        }
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
        background.color = Color.white;
        chose.enabled = false;
        switch (type)
        {
            case WeaponBoxType.Buy:
                priceValue.text = weaponInfo.buyCoin.ToString();
                price = weaponInfo.buyCoin;
                ATKValue.text = weaponInfo.ATK.ToString();
                durabilityValue.text = item.durability.ToString();
                break;
            case WeaponBoxType.Sell:
                priceValue.text = weaponInfo.sellCoin.ToString();
                price = weaponInfo.sellCoin;
                if (item.enchant)
                {
                    ATKValue.text = (weaponInfo.ATK * 1.2f).ToString();
                }
                else
                {
                    ATKValue.text = weaponInfo.ATK.ToString();
                }
                durabilityValue.text = item.durability.ToString();
                break;
            case WeaponBoxType.Enchant:
                if (item.enchant)
                {
                    ATKValue.text = (weaponInfo.ATK * 1.2f).ToString();
                    priceValue.text = "0";
                    price = 0;
                    background.color = Color.gray;
                }
                else
                {
                    TableEntity_Enchant enchantInfo;
                    GameManager.Inst.GetEnchantData(item.itemID, out enchantInfo);
                    ATKValue.text = weaponInfo.ATK.ToString();
                    priceValue.text = enchantInfo.coin.ToString();
                    price = enchantInfo.coin;
                }
                durabilityValue.text = item.durability.ToString();
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

            for (int i = 0; i < 4; i++)
            {
                if (weaponInfo.type == i + 1)
                {
                    mesh[i].gameObject.SetActive(true);
                    mesh[i].sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
                    material[i].sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
                    if (item.enchant)
                    {
                        mesh[i].transform.GetChild(0).gameObject.SetActive(true);
                        ParticleSystem ps = mesh[i].transform.GetChild(0).GetComponent<ParticleSystem>();
                        var sh = ps.shape;
                        sh.mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                        //sh.texture = obj.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
                    }
                    else
                    {
                        mesh[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                else
                {
                    mesh[i].gameObject.SetActive(false);
                }
            }
        }
        else if(this.item.uid == item.uid)
        {
            if(this.item.enchant != item.enchant)
            {
                this.item.enchant = item.enchant;
                mesh[weaponInfo.type -1].transform.GetChild(0).gameObject.SetActive(true);
                ParticleSystem ps = mesh[weaponInfo.type - 1].transform.GetChild(0).GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                //sh.texture = obj.GetComponent<MeshRenderer>().sharedMaterial.GetTexture(0) as Texture2D;
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
