using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSceneManager : SpawnManager
{
    private GameObject potal;
    private FollowCamera cam;
    private PlayerController player;
    private SpiderDemon spiderDemon;

    private bool isInit = false;

    private void Start()
    {
        potal = GameObject.Find("Potal");
        if (potal != null)
        {
            //potal.SetActive(false);
        }
        if (!GameObject.Find("MainCamera").TryGetComponent<FollowCamera>(out cam))
            Debug.Log("SpiderSceneManager - Awake - FollowCamera");
        if (!GameObject.Find("Player").TryGetComponent<PlayerController>(out player))
            Debug.Log("SpiderSceneManager - Awake - PlayerController");
    }
    public void StageClear()
    {
        potal.SetActive(true);
    }

    public void InitSpiderDemon(SpiderDemon monster)
    {
        spiderDemon = monster;
        StartCoroutine(CheckState());
    }


    private IEnumerator CheckState()
    {
        yield return null;
        while (true)
        {
            if (GameManager.Inst.GetTarget() == Vector3.zero)
            {
                StageClear();
                break;
            }
            if (player.GetTargetState() == State.Die)
            {

            }
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }
}
