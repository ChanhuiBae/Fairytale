using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPotal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int buildScene = SceneManager.GetActiveScene().buildIndex;
            if (buildScene == 4)
            {
                GameManager.Inst.AsyncLoadNextScene(SceneName.SpiderScene);
            }
            else if (buildScene == 6)
            {
                GameManager.Inst.AsyncLoadNextScene(SceneName.DragonScene);
            }
            else if (buildScene == 8)
            {
                GameManager.Inst.AsyncLoadNextScene(SceneName.MedusaScene);
            }
        }
    }
}
