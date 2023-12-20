using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private Button inventoryBtn;
    private Button inventoryCloseBtn;
    private Button helfBtn;
    private Transform helfPopup;
    private Transform warningPopup;
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


        if (!GameObject.Find("WarningPopup").TryGetComponent<Transform>(out warningPopup))
            Debug.Log("MenumManager - Awake - Transform");
        else
        {
            warningPopup.gameObject.SetActive(false);
        }

        if (!GameObject.Find("HelfPopup").TryGetComponent<Transform>(out helfPopup))
            Debug.Log("MenumManager - Awake - Transform");
        else
        {
            helfPopup.gameObject.SetActive(false);
        }

        if (!GameObject.Find("Helf").TryGetComponent<Button>(out helfBtn))
            Debug.Log("MenumManager - Awake - Button");
        else
        {
            helfBtn.onClick.AddListener(OnClickHelf);
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

    public void OnClickHelf()
    {
        if (helfPopup.gameObject.activeSelf)
        {
            helfPopup.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            helfPopup.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowWarningPopup()
    {
        warningPopup.gameObject.SetActive(true);
        StartCoroutine(CloseWarningPopup());
    }

    private IEnumerator CloseWarningPopup()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        warningPopup.gameObject.SetActive(false);
    }
}
