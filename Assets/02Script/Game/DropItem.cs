using System.Collections;
using UnityEngine;
public class DropItem : MonoBehaviour
{
    private SphereCollider col;
    private Rigidbody rig;
    private InventoryItemData item;
    private SpawnManager spawnManager;
    private int amount;
    private bool isDrop;

    public void InitDropCoin(int amount, Vector3 pos, SpawnManager spawnManager)
    {
        if (rig == null)
        {
            if (!TryGetComponent<Rigidbody>(out rig))
                Debug.Log("DropItem - InitDropItem - Rigidbody");
            else
            {
                rig.useGravity = true;
            }
        }
        if (col == null)
        {
            if (!TryGetComponent<SphereCollider>(out col))
                Debug.Log("DropItem - InitDropItem - SphereCollider");
            else
            {
                col.isTrigger = true;
                col.radius = 0.8f;
            }
        }
        this.spawnManager = spawnManager;
        this.amount = amount;
        transform.position = pos;
        isDrop = false;
    }
    public void InitDropItem(int itemID, int amount, float durability, int type,  Vector3 pos, SpawnManager spawnManager)
    {
        if (rig == null)
        {
            if (!TryGetComponent<Rigidbody>(out rig))
                Debug.Log("DropItem - InitDropItem - Rigidbody");
            else
            {
                rig.useGravity = true;
            }
        }
        if (col == null)
        {
            if (!TryGetComponent<SphereCollider>(out col))
                Debug.Log("DropItem - InitDropItem - SphereCollider");
            else
            {
                col.isTrigger = true;
                col.radius = 0.8f;
            }
        }

        this.spawnManager = spawnManager;
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = itemID;
        item.amount = amount;
        item.durability = durability;
        item.type = type;
        transform.position = pos;
        isDrop = false;
    }

    public void Drop()
    {
        isDrop = true;
        rig.velocity = new Vector3(Random.Range(-4, 4), 2, Random.Range(-4, 4));
    }

    public bool IsCoin()
    {
        if (amount > 0)
            return true;
        return false;
    }

    public string GetItemID()
    {
        return item.itemID.ToString();
    }

    private IEnumerator ChangeIsDrop()
    {
        yield return YieldInstructionCache.WaitForSeconds(2f);
        isDrop = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(rig != null)
        {
            if (other.CompareTag("Ground"))
            {
                rig.velocity = Vector3.zero;
                StartCoroutine(ChangeIsDrop());
            }

            if (isDrop && other.CompareTag("Player"))
            {
                if(GameManager.Inst.INVENTORY.CURSlotCount < GameManager.Inst.INVENTORY.MAXSlotCount)
                {
                    if (amount > 0)
                    {
                        GameManager.Inst.PlayerCoin += amount;
                        spawnManager.TakeItemPool(this);
                    }
                    else if (GameManager.Inst.LootingItem(item))
                    {
                        spawnManager.TakeItemPool(this);
                    }
                }
                else
                {
                    // warrning
                }
            }
        }
    }
}
