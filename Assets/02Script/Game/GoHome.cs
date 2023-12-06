using UnityEngine;

public class GoHome : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Inst.AsyncLoadNextScene(SceneName.HomeScene);
        }
    }
}
