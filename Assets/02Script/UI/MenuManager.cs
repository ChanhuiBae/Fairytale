using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private Button inventoryBtn;
    private Button inventoryCloseBtn;
    private InventoryPopup inventoryManager;
    private bool isInventoryOpen;
    private RespawnPopup respawnPopup;
    private HP hp;

    public void InitMenuManager()
    {
        if (!GameObject.Find("HP").TryGetComponent<HP>(out hp))
            Debug.Log("MenuManager - Init - HP");
        hp.InitHP(GameManager.Inst.PlayerInfo.maxHP);
        hp.SetCurrentHP(GameManager.Inst.PlayerInfo.curHP);

        if (!GameObject.Find("InventoryPopup").TryGetComponent<InventoryPopup>(out inventoryManager))
            Debug.Log("MenumManager- Awake - InventoryPopupManager");
   
        if (!GameObject.Find("Inventory").TryGetComponent<Button>(out inventoryBtn))
            Debug.Log("MenumManager - Awake - Button");
        else
        {
            inventoryBtn.onClick.AddListener(OpenInventory);
        }

        if (!GameObject.Find("CloseInventory").TryGetComponent<Button>(out inventoryCloseBtn))
            Debug.Log("MenumManager - Init - Button");
        else
        {
            inventoryCloseBtn.onClick.AddListener(CloseInventory);
        }

        if (!GameObject.Find("RespawnPopup").TryGetComponent<RespawnPopup>(out respawnPopup))
            Debug.Log("MenumManager - Init - respawnPopup");

        isInventoryOpen = false;
    }

    public void SetHP(float hpAmount)
    {
        hp.SetCurrentHP(hpAmount);
    }

    private void OpenInventory()
    {
        if (!isInventoryOpen)
        {
            isInventoryOpen = true;
            GameManager.Inst.SaveData();
            StartCoroutine(SetInventory());
        }
    }
    private void CloseInventory()
    {
        Time.timeScale = 1f;
        GameManager.Inst.SaveData();
        inventoryManager.gameObject.LeanScale(Vector3.zero, 0f);
        inventoryManager.CloseWeaponPopup();
        inventoryManager.CloseHelmetPopup();
        inventoryManager.CloseNamePopup();
        isInventoryOpen = false;
    }
    private IEnumerator SetInventory()
    {
        yield return null;
        inventoryManager.UpdatePlayer();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        inventoryManager.UpdateInventory();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        inventoryManager.gameObject.LeanScale(Vector3.one, 0f);
        Time.timeScale = 0f;
    }
    public void UpdateInventory()
    {
        inventoryManager.UpdatePlayer();
        inventoryManager.UpdateInventory();
    }

    public void ShowRespawnUI()
    {
        respawnPopup.ShowPopup();
    }
}
