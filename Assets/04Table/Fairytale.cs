using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Fairytale : ScriptableObject
{
	public List<TableEntity_Item> ItemData;
	public List<TableEntity_Weapon> WeaponData; 
	public List<TableEntity_Tip> TipMess; 
	public List<TableEntity_Monster> MonsterData; 
	public List<TableEntity_DropList> DropList;
	public List<TableEntity_Enchant> Enchant;
	public List<TableEntity_Respawn> Respawn;

}
