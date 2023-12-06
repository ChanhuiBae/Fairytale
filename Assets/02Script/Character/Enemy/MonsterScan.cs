using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class MonsterScan : MonoBehaviour
{
    private SphereCollider col;
    private void Awake()
    {
        if (!TryGetComponent<SphereCollider>(out col))
            Debug.Log("MosterScan - Awake - SphereCollider");
        else
        {
            col.isTrigger = true;
            col.radius = 7f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<MonsterAI>().SetTarget(other.gameObject);
        }
    }
}