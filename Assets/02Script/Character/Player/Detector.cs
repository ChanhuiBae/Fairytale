using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private List<Transform> targets = new List<Transform>();
    private Transform priority_target;

    private void Awake()
    {
        priority_target = transform;
    }

    private IEnumerator FixedUpdateTarget()
    {
        while(targets.Count > 1)
        {
            UpdateTarget();
            yield return YieldInstructionCache.WaitForSeconds(1);
        }
    }

    private void UpdateTarget()
    {
        float min = 100;
        float distance;
        Transform newTarget = transform;
        for(int i = 0; i < targets.Count; i++)
        {
            distance = Vector3.Distance(transform.position, targets[i].position);
            if (distance < min)
            {
                min = distance;
                newTarget = targets[i];
            }
        }
        priority_target = newTarget;
    }
    public Vector3 GetTarget()
    {
        if (targets.Count == 0)
        {
            return Vector3.zero;
        }
        return priority_target.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MonsterBase>(out MonsterBase monster))
        {
            targets.Add(other.transform);
            if (targets.Count == 1)
            {
                priority_target = targets[0];
            }
            else if (targets.Count == 2)
            {
                StartCoroutine(FixedUpdateTarget());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (priority_target == other.transform)
                priority_target = transform;
            targets.Remove(other.transform);
        }
    }
}
