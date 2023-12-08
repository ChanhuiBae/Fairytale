using System.Collections;
using UnityEngine;
using Redcode.Pools;

public enum SpawnType
{
    ST_Once = 0, // 초기화 시 한번 max까지
    ST_Repeat = 1, // max미만 시 시간 간격을 두고
    ST_Max = 2, // 0일 때 max까지
}

[RequireComponent(typeof(SphereCollider))]

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private int maxCount; // 해당 존에 동시에 등장할 수 있는 최대 몬스터의 수
    private int curCount; // 해당 존에 현재 스폰된 마릿수
    private PoolManager monsterPool;
    private int curItemCount; // drop된 item의 수
    private PoolManager dropItemPool;
    private SphereCollider col;

    private SpawnType spawnType;
    [SerializeField]
    private int spawnMonsterTableID;
    [SerializeField]
    private float spawnRadius;
    private TableEntity_Monster monster;
    private TableEntity_DropList dropList;

    private Vector3 spawnPos = Vector3.zero;
    private MonsterBase monsterBase;

    private Vector3 dropPos;
    private DropItem coin;
    private DropItem weapon1;
    private DropItem weapon2;
    private DropItem weapon3;
    private DropItem p50; // 50 percent
    private DropItem p10; // 10 percent

    private SpawnType IntToST(int type)
    {
        switch(type)
        {
            case 1:
                return SpawnType.ST_Repeat;
            case 2:
                return SpawnType.ST_Max;
            default:
                return SpawnType.ST_Once;
        }
    }
    private void Awake()
    {
        if (!GameObject.Find("MonsterPool").TryGetComponent<PoolManager>(out monsterPool))
            Debug.Log("SpawnManager - Awake - PoolManager");
        if (!GameObject.Find("DropItemPool").TryGetComponent<PoolManager>(out dropItemPool))
            Debug.Log("SpawnManager - Awake - PoolManager ");
        if (!TryGetComponent<SphereCollider>(out col))
            Debug.Log("SpawnManager - Awake - SphereCollider");
        else
        {
            col.isTrigger = true;
            col.radius = spawnRadius;
        }
        StartCoroutine(InitSpawnManager());
    }
    private IEnumerator InitSpawnManager()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        GameManager.Inst.GetMonsterData(spawnMonsterTableID, out monster); 
        spawnType = IntToST(monster.spawnType);
        GameManager.Inst.GetDropData(spawnMonsterTableID, out dropList);
        curCount = 0;
        curItemCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (spawnType)
            {
                case SpawnType.ST_Repeat:
                    StartCoroutine(TryRepeatSpawn());
                    break;
                case SpawnType.ST_Max:
                    StartCoroutine(TryMaxSpawn());
                    break;
                case SpawnType.ST_Once:
                    StartCoroutine(TryOnceSpawn());
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (spawnType)
            {
                case SpawnType.ST_Repeat:
                    StopCoroutine(TryRepeatSpawn());
                    break;
                case SpawnType.ST_Max:
                    StopCoroutine(TryMaxSpawn());
                    break;
            }
        }
    }

    private IEnumerator TryRepeatSpawn()
    {
        if (curCount == 0)
        {
            while (curCount < maxCount)
            {
                SpawnUnit();
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
        }
        while (true)
        {
            if(curCount < maxCount)
            {
                SpawnUnit();
            }
            yield return YieldInstructionCache.WaitForSeconds(60f);
        }
    }

    private IEnumerator TryMaxSpawn()
    {
        while (true)
        {
            if(curCount == 0)
            {
                while (curCount < maxCount)
                {
                    SpawnUnit();
                    yield return YieldInstructionCache.WaitForSeconds(1f);
                }
            }
            yield return YieldInstructionCache.WaitForSeconds(60f);
        }
    }

    private IEnumerator TryOnceSpawn()
    {
        if (curCount == 0)
        {
            while (curCount < maxCount)
            {
                SpawnUnit();
                yield return YieldInstructionCache.WaitForSeconds(1f);
            }
        }
    }

    private void SpawnUnit()
    {
        curCount++;
        monsterBase = monsterPool.GetFromPool<MonsterBase>(monster.spawnIndex);
        
        spawnPos = transform.position;
        if(monster.spawnType != 0)
        {
            spawnPos.x += Random.Range(-5f, 5f);
            spawnPos.y = 0.25f;
            spawnPos.z += Random.Range(-5f, 5f);
        }
        monsterBase.transform.position = spawnPos;
        monsterBase.InitMonster(monster, spawnPos, this);
        monsterBase.Spawn();
        GameManager.Inst.AddTarget(monsterBase.transform);
    }
    public void TakeMonsterPool(MonsterBase Mbase)
    {
        GameManager.Inst.DeleteTarget(Mbase.transform);
        monsterPool.TakeToPool<MonsterBase>(monster.uid.ToString(), Mbase);
        curCount--;
    }

    public void DropItem(Vector3 pos)
    {
        TableEntity_Weapon weapon;
        TableEntity_Item item;
        int random;

        for (int i = 0; i < dropList.coin/50; i++)
        {
            curItemCount++;
            coin = dropItemPool.GetFromPool<DropItem>(0);
            coin.InitDropCoin(50, pos, this);
            coin.Drop();
        }

        curCount++;
        GameManager.Inst.GetWeaponData(dropList.weapon1, out weapon);
        weapon1 = dropItemPool.GetFromPool<DropItem>(weapon.dropIndex);
        weapon1.InitDropItem(weapon.uid, 1, weapon.durability, weapon.type, pos, this);
        weapon1.Drop();

        if (dropList.weapon2 != 0)
        {
            curCount++;
            GameManager.Inst.GetWeaponData(dropList.weapon2, out weapon);
            weapon2 = dropItemPool.GetFromPool<DropItem>(weapon.dropIndex);
            weapon2.InitDropItem(weapon.uid, 10, weapon.durability, weapon.type, pos, this);
            weapon2.Drop();
        }
        if (dropList.weapon3 != 0)
        {
            curCount++;
            GameManager.Inst.GetWeaponData(dropList.weapon3, out weapon);
            weapon3 = dropItemPool.GetFromPool<DropItem>(weapon.dropIndex);
            weapon3.InitDropItem(weapon.uid, 10, weapon.durability, weapon.type, pos, this);
            weapon2.Drop();
        }

        random = Random.Range(0, 99);
        if (random < 50)
        {
            curCount++;
            GameManager.Inst.GetItemData(dropList.p50, out item);
            p50 = dropItemPool.GetFromPool<DropItem>(item.dropIndex);
            p50.InitDropItem(item.uid, 1, 1, item.type, pos, this);
            p50.Drop();
        }
        random = Random.Range(0, 99);
        if (random < 10)
        {
            curCount++;
            GameManager.Inst.GetItemData(dropList.p10, out item);
            p10 = dropItemPool.GetFromPool<DropItem>(item.dropIndex);
            p10.InitDropItem(item.uid, 1, 1, item.type, pos, this);
            p10.Drop();
        }
        if (dropList.p5 != 0)
        {
            random = Random.Range(0, 99);
            if (random < 5)
            {
                curCount++;
                GameManager.Inst.GetItemData(dropList.p10, out item);
                p10 = dropItemPool.GetFromPool<DropItem>(item.dropIndex);
                p10.InitDropItem(item.uid, 1, 1, item.type, pos, this);
                p10.Drop();
            }
        }
    }

    public void TakeItemPool(DropItem dropItem)
    {
        if (dropItem.IsCoin())
        {
            dropItemPool.TakeToPool<DropItem>("Coin", dropItem);
            curItemCount--;
        }
        else
        {
            dropItemPool.TakeToPool<DropItem>(dropItem.GetItemID(), dropItem);
            curItemCount--;
        }

    }
}
