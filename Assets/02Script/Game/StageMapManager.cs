using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageMapManager : MonoBehaviour
{
    private Canvas stageMap;
    private Canvas mainCanvas;
    private Button homeBtn;
    private Button demonBtn;
    private Button medusaBtn;
    private Button dragonBtn;
    private Transform playerPiece;
    private Vector3 startPiecePos;

    private void Awake()
    {
        try
        {
            TryGetComponent<Canvas>(out stageMap);
            stageMap.renderMode = RenderMode.ScreenSpaceCamera;
            stageMap.worldCamera = Camera.main;
            stageMap.planeDistance = 1f;
            stageMap.enabled = false;

            if (!GameObject.Find("MainCanvas").TryGetComponent<Canvas>(out mainCanvas))
                Debug.Log("StageMapManager - Awake - Canvas");

            transform.Find("Scroll View/Viewport/Content/HomeIcon/Space/HomeBtn").TryGetComponent<Button>(out homeBtn);
            transform.Find("Scroll View/Viewport/Content/DemonStageIcon/Space/DemonBtn").TryGetComponent<Button>(out demonBtn);
            transform.Find("Scroll View/Viewport/Content/MedusaStageIcon/Space/MedusaBtn").TryGetComponent<Button>(out medusaBtn);
            transform.Find("Scroll View/Viewport/Content/DragonStageIcon/Space/DragonBtn").TryGetComponent<Button>(out dragonBtn);
            transform.Find("Scroll View/Viewport/Content/PlayerPiece").TryGetComponent<Transform>(out playerPiece);
            startPiecePos = playerPiece.position;

            if (SceneManager.GetActiveScene().buildIndex == 3)
                homeBtn.onClick.AddListener(CloseStageMap);
            else
                homeBtn.onClick.AddListener(GoHome);
            demonBtn.onClick.AddListener(GoDemonScene);
            medusaBtn.onClick.AddListener(GoMedusaScene);
            dragonBtn.onClick.AddListener(GoDragonScene);
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Message);
        }
    }
    
    public void OpenStageMap()
    {
        GameManager.Inst.PlayerIsController(false);
        stageMap.enabled = true;
        mainCanvas.enabled = false;
    }

    private void CloseStageMap()
    {
        stageMap.enabled = true;
        mainCanvas.enabled = false;
        GameManager.Inst.PlayerIsController(false);
    }

    private void GoHome()
    {
        playerPiece.transform.position = homeBtn.transform.position;
        GameManager.Inst.AsyncLoadNextScene(SceneName.HomeScene);
    }

    private void GoDemonScene()
    {
        playerPiece.transform.position = demonBtn.transform.position;
        GameManager.Inst.AsyncLoadNextScene(SceneName.DemonScene);
    }

    private void GoMedusaScene()
    {
        playerPiece.transform.position = medusaBtn.transform.position;
        GameManager.Inst.AsyncLoadNextScene(SceneName.GoblinScene);
    }

    private void GoDragonScene()
    {
        playerPiece.transform.position = dragonBtn.transform.position;
        GameManager.Inst.AsyncLoadNextScene(SceneName.GuardianScene);
    }
}
