using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSceneManager : SpawnManager
{
    private GameObject potal;
    private FollowCamera cam;
    private PlayerController player;
    private Transform doorFirst;
    private BoxCollider doorStart;
    private Vector3 doorFirstPos;
    private Vector3 doorStartPos;

    private bool isInit = false;

    private void Start()
    {
        potal = GameObject.Find("Potal");
        if (potal != null)
        {
            potal.SetActive(false);
        }
        if (!GameObject.Find("MainCamera").TryGetComponent<FollowCamera>(out cam))
            Debug.Log("SpiderSceneManager - Awake - FollowCamera");
        if (!GameObject.Find("Player").TryGetComponent<PlayerController>(out player))
            Debug.Log("SpiderSceneManager - Awake - PlayerController");
        if (!transform.GetChild(0).TryGetComponent<Transform>(out doorFirst))
            Debug.Log("SpiderSceneManager - Awake - transform");
        else
        {
            doorFirstPos = doorFirst.transform.position;
        }
        if (!transform.GetChild(1).TryGetComponent<BoxCollider>(out doorStart))
            Debug.Log("SpiderSceneManager - Awake - BoxCollider");
        else
        {
            doorStartPos = doorStart.transform.position;
        }
        StartCoroutine(TryOpenDoor());
    }

    private IEnumerator TryOpenDoor()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        while (!player.ISCONTROLLER)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        player.ISCONTROLLER = false;
        // CAM
        // open door
        doorFirst.transform.position += new Vector3(0, 1.6f, 0);
        doorStart.transform.position += new Vector3(0, 1.6f, 0);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        player.ISCONTROLLER = true;
        StartCoroutine(TryCloseDoor());
    }

    private IEnumerator TryCloseDoor()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        while (GameManager.Inst.GetTarget() == Vector3.zero)
        {
            yield return null;
        }
        CloseDoor();
    }


    public void CloseDoor()
    {
        doorStart.isTrigger = false;
        doorStart.transform.LeanMoveLocalY(-1.6f,0);
        StartCoroutine(CheckState());
    }

    private void StageClear()
    {
        player.Victory();
        potal.SetActive(true);
    }

    private void ResetScene()
    {
        doorFirst.transform.position = doorFirstPos;
        doorStart.transform.position = doorStartPos;
        StartCoroutine(TryOpenDoor());
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
                ResetScene();
                break;
            }
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }
}
