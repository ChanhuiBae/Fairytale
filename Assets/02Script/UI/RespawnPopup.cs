using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RespawnPopup : MonoBehaviour
{
    private Button respawnBtn;
    private Button goHomeBtn;
    private TextMeshProUGUI nickname1;
    private TextMeshProUGUI nickname2;
    private TextMeshProUGUI coin;
    private int price;
    private void Awake()
    {
        gameObject.LeanScale(Vector3.zero, 0f);
        if (!transform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out nickname1))
            Debug.Log("RespawnPopup - Awake - TextMeshProUGUI");
        else
        {
            nickname1.text = GameManager.Inst.PlayerName;
        }
        if (!transform.GetChild(3).TryGetComponent<TextMeshProUGUI>(out nickname2))
            Debug.Log("RespawnPopup - Awake - TextMeshProUGUI");
        else
        {
            nickname2.text = GameManager.Inst.PlayerName;
        }
        if (!transform.GetChild(4).TryGetComponent<TextMeshProUGUI>(out coin))
            Debug.Log("RespawnPopup - Awake - TextMeshProUGUI");
        else
        {
            price = 150;
        }
        if (!transform.Find("RespawnButton").TryGetComponent<Button>(out respawnBtn))
            Debug.Log("RespawnPopup - Awake - EventTrigger");
        else
        {
            respawnBtn.onClick.AddListener(Respawn);
        }
        if (!transform.Find("GoHomeButton").TryGetComponent<Button>(out goHomeBtn))
            Debug.Log("RespawnPopup - Awake - EventTrigger");
        else
        {
            goHomeBtn.onClick.AddListener(GoHome);
        }

    }


    private void Respawn()
    {
        GameManager.Inst.PlayerSpawn();
        GameManager.Inst.PlayerCoin -= 150;
        gameObject.LeanScale(Vector3.zero, 0);
    }


    private void GoHome()
    {
        GameManager.Inst.UpdateHP(30);
        GameManager.Inst.AsyncLoadNextScene(SceneName.HomeScene);
    }

    public void ShowPopup()
    {
        coin.text = price + " / " + GameManager.Inst.PlayerCoin;
        gameObject.LeanScale(Vector3.one, 1f);

    }
}
