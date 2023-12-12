using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // input output controll

[System.Serializable]
public class PlayerData
{
    public string userNickname;
    public int coin;
    public float curHP;
    public float maxHP;
    public float DEF;
    public int maxStamina;
    public int uidCounter;  // 0시작, 아이템 습득 시 1씩 증가, 아이템 고유 ID(uid)
    public Inventory inventory;
    public TableEntity_Weapon onehand;
    public int i_onehand; // i_가 들어간 객체는 각 인벤토리 uid 정보이다
    public TableEntity_Weapon shield;
    public int i_shield;
    public TableEntity_Weapon ranged;
    public int i_ranged;
    public TableEntity_Weapon projectile;
    public TableEntity_Item helmet;
    public int i_helmet;
    public string usedTime;
    public int shopHpPotion;
    public int shopStaminaPotion;
}

public enum SceneName
{
    IntroScene,
    TitleScene,
    LoadingScene,
    HomeScene,
    DemonScene,
    SpiderScene,
    GuardianScene,
    DragonScene,
    GoblinScene,
    MedusaScene
}


public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private PlayerData pData;

    private PlayerController player;
    private FadeManager fadeManager;
    private MenuManager menuManager;
    private List<Transform> targetList;

    public void Awake()
    {
        base.Awake();
        dataPath = Application.persistentDataPath + "/save";
        // 해당 경로에 save라는 폴더를 생성 후 그곳에 저장

        #region TableData
        table = Resources.Load<Fairytale>("Fairytale");
        for (int i = 0; i < table.ItemData.Count; i++)
        {
            itemDataTable.Add(table.ItemData[i].uid, table.ItemData[i]);
        }

        for(int i = 0; i < table.WeaponData.Count; i++)
        {
            weaponDataTable.Add(table.WeaponData[i].uid, table.WeaponData[i]);
            weaponItemID.Add(table.WeaponData[i].uid);
        }

        for (int i = 0; i < table.TipMess.Count; i++)
        {
            if (table.TipMess[i].sceneNumber == 3)
            {
                homeTip.Add(table.TipMess[i]);
            }
            else if (table.TipMess[i].sceneNumber > 3)
            {
                stageTip.Add(table.TipMess[i]);
            }
        }

        for (int i = 0; i < table.MonsterData.Count; i++)
        {
            monsterData.Add(table.MonsterData[i].uid, table.MonsterData[i]);
        }

        for(int i = 0; i < table.DropList.Count; i++)
        {
            dropDataTable.Add(table.DropList[i].uid, table.DropList[i]);
        }

        for(int i = 0; i < table.Enchant.Count; i++)
        {
            enchantTable.Add(table.Enchant[i].weaponID, table.Enchant[i]);
        }

        for(int i = 0; i < table.Respawn.Count; i++)
        {
            if (table.Respawn[i].sceneNumber == 4)
            {
                demonSceneSpawn.Add(new Vector3(table.Respawn[i].x, table.Respawn[i].y, table.Respawn[i].z));
            }
            else if (table.Respawn[i].sceneNumber == 5)
            {
                spiderSceneSpawn = new Vector3(table.Respawn[i].x, table.Respawn[i].y, table.Respawn[i].z);
            }
        }

        #endregion

        pData = new PlayerData();
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level > 2)
        {
            StartCoroutine(InitGameManager());
        }
    }

    private IEnumerator InitGameManager()
    {
        LoadData();
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        bool end = false;
        targetList = new List<Transform>();

        while (!end)
        {
            end = UpdatePlayer();
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }

        if(menuManager == null)
        {
            GameObject.Find("MainCanvas").TryGetComponent<MenuManager>(out menuManager);
            if(menuManager != null)
                menuManager.InitMenuManager();
        }
        if(fadeManager == null)
        {
            GameObject.Find("MainCanvas").TryGetComponent<FadeManager>(out fadeManager);
            if (fadeManager != null)
            {
                fadeManager.Fade_InOut(true);
                player.ISCONTROLLER = true;
            }
        }
  
    }

    #region UpdateGMInfo
    public bool UpdatePlayer()
    {
        if(!GameObject.Find("Player").TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("GameManger - UpdateGMInfo - PlayerController");
            return false;
        }
        else
        {
            player.InitPlayerController();
            // stamina는 scene 전환 시 max
            return true;
        }
    }
    #endregion

    #region TableData

    private Dictionary<int, TableEntity_Monster> monsterData = new Dictionary<int, TableEntity_Monster>();
    public bool GetMonsterData(int itemID, out TableEntity_Monster moster)
    {
        return monsterData.TryGetValue(itemID, out moster);
    }


    private List<TableEntity_Tip> homeTip = new List<TableEntity_Tip>();
    private List<TableEntity_Tip> stageTip = new List<TableEntity_Tip>();
    private List<Vector3> demonSceneSpawn = new List<Vector3>();
    private Vector3 spiderSceneSpawn = Vector3.zero;
    private int random;

    public string GetTipMessage(int nextSceneNum)
    {
        string result = "";
        if (nextSceneNum == 3)
        {
            random = UnityEngine.Random.Range(0, homeTip.Count);
            result = homeTip[random].tipText;
        }
        else if (nextSceneNum > 3)
        {
            random = UnityEngine.Random.Range(0, stageTip.Count);
            result = stageTip[random].tipText;
        }
        return result;
    }

    public Vector3 GetSpawnPos(int sceneNum, Vector3 player)
    {
        Vector3 result = Vector3.zero;
        float minDistance = 999;
        float distance;
        if(sceneNum == 4)
        {
           for(int i =0; i< demonSceneSpawn.Count; i++)
            {
                distance = Vector3.Distance(player, demonSceneSpawn[i]);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    result = demonSceneSpawn[i];
                }
            }
        }
        else if (sceneNum == 5)
        {
            return spiderSceneSpawn;
        }
        return result;
    }

    private Fairytale table;
    private Dictionary<int, TableEntity_Item> itemDataTable = new Dictionary<int, TableEntity_Item>();
    private List<int> weaponItemID = new List<int>();
    private Dictionary<int, TableEntity_Weapon> weaponDataTable = new Dictionary<int, TableEntity_Weapon>();
    private Dictionary<int, TableEntity_DropList> dropDataTable = new Dictionary<int, TableEntity_DropList>();
    private Dictionary<int, TableEntity_Enchant> enchantTable = new Dictionary<int, TableEntity_Enchant>();


    public bool GetItemData(int itemID, out TableEntity_Item data)
    {
        return itemDataTable.TryGetValue(itemID, out data);
    }

    public bool GetWeaponData(int weaponID, out TableEntity_Weapon data)
    {
        return weaponDataTable.TryGetValue(weaponID, out data);
    }

    public bool GetDropData(int monsterID, out TableEntity_DropList data)
    {
        return dropDataTable.TryGetValue(monsterID, out data);
    }

    public bool GetEnchantData(int weponID, out TableEntity_Enchant data)
    {
        return enchantTable.TryGetValue(weponID, out data);
    }

    #endregion

    #region Save&Load
    private string dataPath;
    public void SaveData()
    {
        string data = JsonUtility.ToJson(pData);
        File.WriteAllText(dataPath, data);
    }

    public bool LoadData()
    {
        if (File.Exists(dataPath))
        {
            string data = File.ReadAllText(dataPath);
            pData = JsonUtility.FromJson<PlayerData>(data);
            return true;
        }

        return false;
    }

    public void DeleteData()
    {
        File.Delete(dataPath);
    }

    public bool CheckData()
    {
        if (File.Exists(dataPath))
        {
            return LoadData();
        }
        return false;
    }
    #endregion

    #region updateUserData
    public void CreateUserData(string newNickName)
    {
        pData.userNickname = newNickName;
        pData.coin = 1000;
        pData.uidCounter = 0;
        pData.curHP = pData.maxHP = 500;
        pData.DEF = 3;
        pData.maxStamina = 200;
        pData.i_helmet = -1;
        TableEntity_Item helmet = new TableEntity_Item();
        helmet.uid = -1;
        helmet.resources = "";
        helmet.name = "";
        helmet.equip = true;
        helmet.type = 5;
        helmet.DEF = 0;
        pData.helmet = helmet;
        pData.inventory = new Inventory();
        InventoryItemData item;
        item = new InventoryItemData();
        weaponDataTable.TryGetValue(10501, out pData.projectile);
        item.uid = -1;
        item.itemID = pData.projectile.uid;
        item.type = 4;
        item.amount = 10;
        item.durability = 1;
        item.enchant = false;
        pData.inventory.AddWeapon(item);
        item = new InventoryItemData();
        weaponDataTable.TryGetValue(10103, out pData.onehand);
        item.uid = -1;
        item.itemID = pData.onehand.uid;
        item.type = 1;
        item.durability = pData.onehand.durability;
        item.amount = 1;
        item.enchant = true;
        pData.inventory.AddWeapon(item);
        pData.i_onehand = pData.inventory.FindIndexByItemID(pData.onehand.uid);
        item = new InventoryItemData();
        weaponDataTable.TryGetValue(10201, out pData.shield);
        item.uid = -1;
        item.itemID = pData.shield.uid;
        item.type = 2;
        item.amount = 1;
        item.durability = pData.shield.durability-200;
        item.enchant = true;
        pData.inventory.AddWeapon(item);
        pData.i_shield = pData.inventory.FindIndexByItemID(pData.shield.uid);
        item = new InventoryItemData();
        weaponDataTable.TryGetValue(10301, out pData.ranged);
        item.uid = -1;
        item.itemID = pData.ranged.uid;
        item.type = 3;
        item.amount = 1;
        item.durability = pData.ranged.durability;
        item.enchant = true;
        pData.inventory.AddWeapon(item);
        pData.i_ranged = pData.inventory.FindIndexByItemID(pData.ranged.uid);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 30107;
        item.amount = 5;
        item.durability = 1;
        item.type = 7;
        item.enchant = false;
        pData.inventory.AddItem(item);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 30108;
        item.amount = 5;
        item.durability = 1;
        item.type = 7;
        item.enchant = false;
        pData.inventory.AddItem(item); 
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 30102;
        item.amount = 3;
        item.durability = 1;
        item.type = 6;
        item.enchant = false;
        pData.inventory.AddItem(item);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 20101;
        item.amount = 1;
        item.durability = 1;
        item.type = 5;
        item.enchant = false;
        pData.inventory.AddItem(item);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 10101;
        item.amount = 1;
        item.durability = 200;
        item.type = 1;
        item.enchant = false;
        pData.inventory.AddWeapon(item);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 10401;
        item.amount = 1;
        item.durability = 360;
        item.type = 4;
        item.enchant = false;
        pData.inventory.AddWeapon(item);
        item = new InventoryItemData();
        item.uid = -1;
        item.itemID = 10404;
        item.amount = 1;
        item.durability = 360;
        item.type = 4;
        item.enchant = true;
        pData.inventory.AddWeapon(item);

        SetUsedTime();
        pData.shopHpPotion = 10;
        pData.shopStaminaPotion = 10;
        SaveData();
    }

    public void PlayerDieReset()
    {
        MonsterBase MBase;
        for(int i =0; i < targetList.Count; i++)
        {
            MBase = targetList[i].GetComponent<MonsterBase>();
            if (MBase != null)
            {
                MBase.ResetHP();
            }
        }
        menuManager.ShowRespawnUI();
    }

    public void PlayerSpawn()
    {
        player.Spawn();
    }

    #region Player Setter
    public void UpdateHP(float hpAmount)
    {
        pData.curHP = hpAmount;
        menuManager.SetHP(pData.curHP);
        if (pData.curHP <= 0)
        {
            pData.curHP = 0;
        }
    }

    public void WarnOnehand(float damage)
    {
        pData.inventory.WarnItem(pData.i_onehand, damage);
    }

    public void WarnShield(float damage)
    {
        pData.inventory.WarnItem(pData.i_shield, damage);
    }

    public void WarnRanged(float damage)
    {
        pData.inventory.WarnItem(pData.i_ranged, damage);
    }

    public void SetWeapon(int uid, int itemID)
    {
        TableEntity_Weapon weapon;
        if (pData.inventory.FindIndexByUid(uid) > -1 && weaponDataTable.TryGetValue(itemID, out weapon))
        {
            if (weapon.equip)
            {
                switch (weapon.type)
                {
                    case 4:
                        pData.ranged = weapon;
                        pData.i_ranged = uid;
                        player.InitRanged(weapon, pData.inventory.GetItemEnchant(uid));
                        if (weapon.uid > 10403)
                        {
                            GetWeaponData(10503, out pData.projectile);
                            player.InitProjectileInfo(pData.projectile);
                        }
                        else
                        {
                            GetWeaponData(10502, out pData.projectile);
                            player.InitProjectileInfo(pData.projectile);
                        }
                        break;
                    case 3:
                        pData.ranged = weapon;
                        pData.i_ranged = uid;
                        player.InitRanged(weapon, pData.inventory.GetItemEnchant(uid));
                        GetWeaponData(10501, out pData.projectile);
                        player.InitProjectileInfo(pData.projectile);
                        break;
                    case 1:
                        pData.onehand = weapon;
                        pData.i_onehand = uid;
                        player.InitOneHand(weapon, pData.inventory.GetItemEnchant(uid));
                        break;
                    case 2:
                        pData.shield = weapon;
                        pData.i_shield = uid;
                        player.InitShield(weapon, pData.inventory.GetItemEnchant(uid));
                        break;
                }
            }
        }
        SaveData();
        menuManager.UpdateInventory();
    }

    public void SetHelmet(int uid, int itemID)
    {
        TableEntity_Item helmet;
        if (pData.inventory.FindIndexByUid(uid) > -1 && itemDataTable.TryGetValue(itemID, out helmet))
        {
            if (helmet.equip)
            {
                pData.helmet = helmet;
                pData.i_helmet = uid;
                pData.DEF = 3 + helmet.DEF;
                player.InitHelmet(helmet);
            }
        }
        SaveData();
        menuManager.UpdateInventory();
    }

    public void SetWeaponNull(int type) // using durability is zero so user weared weapon is null
    {
        TableEntity_Weapon weapon = new TableEntity_Weapon();
        weapon.uid = -1;
        weapon.type = type;
        weapon.ATK = 0;
        switch (type)
        {
            case 1:
                pData.onehand = null;
                pData.i_onehand = -1;
                player.InitOneHand(weapon, false);
                break;
            case 2:
                pData.shield = null;
                pData.i_shield = -1;
                player.InitShield(weapon, false);
                break;
            default:
                pData.ranged = null;
                pData.i_ranged = -1;
                player.InitRanged(weapon, false);
                pData.projectile = null;
                player.InitProjectileInfo(weapon);
                break;
        }
        SaveData();
    }

    public void SetWeaponNullAtInven(int type) // change at inventory
    {
        TableEntity_Weapon weapon = new TableEntity_Weapon();
        weapon.uid = -1;
        weapon.type = type;
        weapon.ATK = 0;
        switch (type)
        {
            case 1:
                pData.onehand = null;
                pData.i_onehand = -1;
                player.InitOneHand(weapon, false);
                break;
            case 2:
                pData.shield = null;
                pData.i_shield = -1;
                player.InitShield(weapon, false);
                break;
            case 5:
                pData.helmet = null;
                pData.i_helmet = -1;
                pData.DEF = 3;
                TableEntity_Item item = new TableEntity_Item();
                item.uid = -1;
                item.resources = "";
                player.InitHelmet(item);
                break;
            default:
                pData.ranged = null;
                pData.i_ranged = -1;
                player.InitRanged(weapon, false);
                pData.projectile = weapon;
                break;
        }
        SaveData();
        menuManager.UpdateInventory();
    }

    public bool LootingItem(InventoryItemData newItem)
    {
        if (pData.inventory == null)
            Debug.Log("inventory is null");
        else if (!pData.inventory.IsFull())
        {
            if(newItem.itemID > 20000)
            {
                pData.inventory.AddItem(newItem);
            }
            else
            {
                pData.inventory.AddWeapon(newItem);
            }
            return true;
        }
        return false;
    }

    public void SetUsedTime()
    {
        DateTime time = DateTime.Now;
        pData.usedTime = time.ToString();
    }
    
    public void PlayerIsController(bool value)
    {
        player.ISCONTROLLER = value;
        player.StopMove();
    }

    public void AddTarget(Transform target)
    {
        targetList.Add(target);
    }

    public void DeleteTarget(Transform target)
    {
        targetList.Remove(target);
    }

    #endregion
    #endregion

    #region PlayerDataGetter
    public PlayerData PlayerInfo
    {
        get => pData;
    }
    public Inventory INVENTORY
    {
        get
        {
            return pData.inventory;
        }
    }

    public int PlayerUIDMaker
    {
        get
        {
            return pData.uidCounter++;
        }
    }

    public int PlayerCoin
    {
        get => pData.coin;
        set => pData.coin = value;
    }

    public string PlayerName
    {
        get => pData.userNickname;
    }

    public int ShopHpPotion
    {
        get => pData.shopHpPotion;
        set => pData.shopHpPotion = value;
    }

    public int ShopStaminaPotion
    {
        get => pData.shopStaminaPotion;
        set => pData.shopStaminaPotion = value;
    }


    public bool CheckItem(int itemID)
    {
        if (pData.inventory == null)
            Debug.Log("inventory is null");
        else
        {
            if (pData.inventory.GetItemAmount(itemID) > 0)
                return true;
        }
        return false;
    }

    public Vector3 GetTarget()
    {
        if(targetList.Count == 0)
        {
            return Vector3.zero;
        }
        else if(targetList.Count == 1)
        {
            return targetList[0].position;
        }
        else
        {
            Vector3 result = Vector3.zero;
            float min = 999;
            float distance;
            for(int i = 0; i < targetList.Count; i++)
            {
                distance = Vector3.Distance(player.transform.position, targetList[i].position);
                if(distance < min)
                {
                    min = distance;
                    result = targetList[i].position;
                }
            }
            if(min > 10)
            {
                return Vector3.zero;
            }
            return result;
        }
    }

    public TableEntity_Weapon GetCanBuyWeapon()
    {
        int random = UnityEngine.Random.Range(0, weaponItemID.Count -1);
        TableEntity_Weapon result;
        random = weaponItemID[random];
        weaponDataTable.TryGetValue(random, out result);
        while(result.buyCoin == 0)
        {
            random = UnityEngine.Random.Range(0, weaponDataTable.Count - 1);
            random = weaponItemID[random];
            weaponDataTable.TryGetValue(random, out result);
        }
        return result;
    }
    #endregion

    #region LoadingLogic
    private SceneName nextScene;
    public SceneName NextScene
    {
        get => nextScene;
    }

    public string UsedTime
    {
        get => pData.usedTime;
    }

    public void AsyncLoadNextScene(SceneName scene)
    {
        SaveData();
        nextScene = scene;
        if(SceneManager.GetActiveScene().buildIndex > 2)
            fadeManager.Fade_InOut(false);
        SceneManager.LoadScene("LoadingScene");
    }
    #endregion
}
