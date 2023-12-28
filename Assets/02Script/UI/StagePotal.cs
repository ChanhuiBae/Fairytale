using UnityEngine;

public class StagePotal : MonoBehaviour
{
    private StageMapManager stageMap;
    private void Awake()
    {
        if (!GameObject.Find("StageMap").TryGetComponent<StageMapManager>(out stageMap))
            Debug.Log("StagePotal - Awake - StageMapManager");
        else
        {
            stageMap.InitStageMap();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageMap.OpenStageMap();
        }
    }
}
