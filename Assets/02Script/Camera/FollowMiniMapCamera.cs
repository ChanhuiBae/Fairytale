using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMiniMapCamera : MonoBehaviour
{
    private Transform target;
    private Vector3 offset = new Vector3(0, 12, 0);
    private Vector3 m_Rotation = new Vector3(90, 0, 0);

    private void Awake()
    {
        InitFollowCamera();
    }

    private bool InitFollowCamera()
    {
        transform.rotation = Quaternion.Euler(m_Rotation);
        target = GameObject.Find("Player").transform;
        return target != null;
    }

    private void Update()
    {
        if (target == null)
        {
            if (InitFollowCamera())
                Debug.Log("FollowCamera - Update() - target is null");
        }
        else
        {
            transform.position = target.position + offset;

        }
    }
}