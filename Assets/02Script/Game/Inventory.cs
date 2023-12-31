using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public int uid;
    public int itemID;
    public int type; // 0: onehand, 1: shield, 2:ranged, 3:helmet, 4:items, projectile
    public int amount;
    public float durability;
    public bool enchant;
}

[System.Serializable]
public class Inventory
{
    private int maxSlotCount = 32;
    public int MAXSlotCount { get => maxSlotCount; }

    private int curSlotCount = 0;
    public int CURSlotCount
    {
        get => curSlotCount;
        set => curSlotCount = value;
    }
    public bool IsFull
    {
        get { return curSlotCount >= maxSlotCount; }
    }
    public int DeltaSlotCount
    {
        get { return maxSlotCount - curSlotCount; }
    }

    [SerializeField]
    private List<InventoryItemData> items = new List<InventoryItemData>();

    public int FindIndexByUid(int uid)
    {
        curSlotCount = items.Count;
        for (int i = 0; i < curSlotCount; i++)
        {
            if (items[i].uid == uid)
            {
                return i;
            }
        }
        return -1;
    }

    public int FindIndexByItemID(int itemID)
    {
        curSlotCount = items.Count;
        for (int i = 0; i < curSlotCount; i++)
        {
            if (items[i].itemID == itemID)
            {
                return i;
            }
        }
        return -1;
    }

   

    public void AddItem(InventoryItemData newItem)
    {
        int index = FindIndexByItemID(newItem.itemID);
        if(index <= -1)
        {
            if (GameManager.Inst.GetItemData(newItem.itemID, out TableEntity_Item item))
            {
                if (item.equip) // 장착여부
                {
                    newItem.uid = GameManager.Inst.PlayerUIDMaker;
                    newItem.amount = 1;
                    items.Add(newItem);
                    curSlotCount++;
                }
                else // first item
                {
                    newItem.uid = GameManager.Inst.PlayerUIDMaker;
                    items.Add(newItem);
                    curSlotCount++;
                }
            }
        }
        else if(index > -1)
        {
            items[index].amount += newItem.amount;
            if(items[index].amount > 99)
            {
                items[index].amount = 99;
            }
        }
    }

    public void AddWeapon(InventoryItemData newItem)
    {
        if (GameManager.Inst.GetWeaponData(newItem.itemID, out TableEntity_Weapon item))
        {
            if (item.equip) // 장착여부
            {
                newItem.uid = GameManager.Inst.PlayerUIDMaker;
                items.Add(newItem);
                curSlotCount++;
            }
            else
            {
                int index = FindIndexByItemID(newItem.itemID);
                if (index == -1) 
                {
                    newItem.uid = GameManager.Inst.PlayerUIDMaker;
                    items.Add(newItem);
                    curSlotCount++;
                }
                else
                {
                    items[index].amount += newItem.amount;
                    if (items[index].amount > 99)
                    {
                        items[index].amount = 99;
                    }
                }
            }
        }
    }

    public void BuyWeapon(int itemID)
    {
        if (GameManager.Inst.GetWeaponData(itemID, out TableEntity_Weapon item))
        {
            InventoryItemData newItem = new InventoryItemData();
            newItem.uid = GameManager.Inst.PlayerUIDMaker;
            newItem.amount = 1;
            newItem.enchant = false;
            newItem.itemID = itemID;
            newItem.durability = item.durability;
            newItem.type = item.type;
            items.Add(newItem);
            curSlotCount++;
        }
    }

    public List<InventoryItemData> GetItemList()
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].durability == 0)
            {
                items.RemoveAt(i);
            }
        }
        curSlotCount = items.Count;
        return items;
    }

    public int GetItemAmount(int itemID)
    {
        int index = FindIndexByItemID(itemID);
        if (index == -1)
            return 0;
        return items[index].amount;
    }

    public InventoryItemData GetItem(int uid)
    {
        int index = FindIndexByUid(uid);
        if(index != -1)
        {
            return items[index];
        }
        return null;
    }

    public bool GetItemEnchant(int uid)
    {
        int index = FindIndexByUid(uid);
        if (index != -1)
        {
            return items[index].enchant;
        }
        return false;
    }

    public void DeleteItemAmount(int itemID, int amount)
    {
        int index = FindIndexByItemID(itemID);
        if (-1 < index)
        {
            items[index].amount -= amount;
            if (items[index].amount < 1)
            {
                items.RemoveAt(index);
                curSlotCount--;
            }
        }
    }

    public void DeleteItem(InventoryItemData deleteItem)
    {
        int index = FindIndexByUid(deleteItem.uid);
        if (-1 < index)
        {
            items[index].amount -= deleteItem.amount;
            if (items[index].amount < 1)
            {
                items.RemoveAt(index);
                curSlotCount--;
            }
        }
    }

    public void WarnItem(int uid, float damage)
    {
        int index = FindIndexByUid(uid);
        items[index].durability -= damage;
        if (items[index].durability <= 0)
        {
            TableEntity_Weapon weapon;
            GameManager.Inst.GetWeaponData(items[index].itemID, out weapon);
            GameManager.Inst.SetWeaponNull(weapon.type);
            items.RemoveAt(index);
            curSlotCount--;
        }
    }

    public void EnchantItem(int uid)
    {
        int index = FindIndexByUid(uid);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(items[index].itemID, out weapon);
        items[index].durability = weapon.durability;
        items[index].enchant = true;
    }

    public void FixItem(int uid)
    {
        int index = FindIndexByUid(uid);
        TableEntity_Weapon weapon;
        GameManager.Inst.GetWeaponData(items[index].itemID, out weapon);
        items[index].durability = weapon.durability;
    }

    // 같은 UID를 갖는 아이템의 정보를 갱신해주는 함수. ItemID 
    public void UpdateItemInfo(InventoryItemData data)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].uid == data.uid)
            {
                items[i].itemID = data.itemID;
                items[i].type = data.type;
                items[i].amount = data.amount;
                items[i].durability = data.durability;
                items[i].enchant = data.enchant;
            }
        }
    }
}
