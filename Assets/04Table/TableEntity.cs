[System.Serializable]
public class TableEntity_Item
{
    public int uid;
    public int type;
    public string name;
    public string resources;
    public string iconResources;
    public int sellGold;
    public int DEF;
    public bool equip;
    public int dropIndex;
}
[System.Serializable]
public class TableEntity_Weapon
{
    public int uid;
    public int type;
    public string name;
    public string resources;
    public string iconResources;
    public int attribute;
    public int ATK;
    public int durability;
    public bool equip;
    public int dropIndex;
    public int sellCoin;
    public int buyCoin;
    public int fixCoin;
}
[System.Serializable]
public class TableEntity_Tip
{
    public int uid;
    public int sceneNumber;
    public string tipText;
}

[System.Serializable]
public class TableEntity_Monster
{
    public int uid;
    public string monsterName;
    public int monsterType;
    public int moveSpeed;
    public int DEF;
    public int maxHP;
    public int spawnType;
    public int onehand;
    public int shield;
    public int ranged;
    public int projectile;
    public int spawnIndex;
}

[System.Serializable]
public class TableEntity_DropList
{
    public int uid;
    public int coin;
    public int weapon1;
    public int weapon2;
    public int weapon3;
    public int p50;
    public int p10;
    public int p5;
}

[System.Serializable]
public class TableEntity_Enchant
{
    public int uid;
    public int weaponID;
    public int coin;
    public int itemID;
    public int amount;
    public int rate;
}

[System.Serializable]
public class TableEntity_Respawn
{
    public int uid;
    public int sceneNumber;
    public float x;
    public float y;
    public float z;
}