using System.Collections;
using UnityEngine;

public enum CameraMode
{
    Play,
    Front,
    Back,
    Aim
}

public class FollowCamera : MonoBehaviour
{
    private Transform target;
    private Transform aimPos;

    private Vector3 offset = new Vector3 (0, 5f, -7);
    private Vector3 m_Rotation = new Vector3(30, 0, 0);
    private Vector3 b_Rotation = Vector3.zero;
    private Vector3 bowRotation = Vector3.zero;


    private Vector3 delta = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private bool startSpring = false;
    private bool isSpring = false;

    private CameraMode mode = CameraMode.Play;
    public void SetAim()
    {
        mode = CameraMode.Aim;
        SetAimMode();
    }

    public void SetPlay()
    {
        mode = CameraMode.Play;
        SetMainMode();
    }

    public void SetBack()
    {
        mode = CameraMode.Back;
    }

    private void Awake()
    {
        InitFollowCamera();
    }

    private bool InitFollowCamera()
    {
        transform.rotation = Quaternion.Euler(m_Rotation);
        target = GameObject.Find("Player").transform;
        aimPos = target.GetChild(1).transform;
        bowRotation = Vector3.zero;
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
            if (mode == CameraMode.Aim)
            {
                transform.rotation = Quaternion.Euler(b_Rotation + bowRotation);
            }
            else if(mode == CameraMode.Front)
            {
                transform.position = target.position + target.forward + Vector3.up;
            }
            else if(mode == CameraMode.Back)
            {
                transform.position = target.position - target.forward + Vector3.up;
            }
            else
            {
                if (!startSpring)
                {
                    delta = target.position + offset - transform.position;
                    if (delta == Vector3.zero || (delta.x < 0.02f && delta.y < 0.02f && delta.z < 0.02f))
                    {
                        startSpring = true;
                    }
                    else
                    {
                        transform.position = target.position + offset;

                    }
                }
                else if (!isSpring)
                {
                    if (startPos == Vector3.zero)
                    {
                        delta = target.position + offset - transform.position;
                        delta.x = Mathf.Abs(delta.x);
                        delta.y = Mathf.Abs(delta.y);
                        delta.z = Mathf.Abs(delta.z);

                        if (delta.x > 0.01f || delta.y > 0.01f || delta.z > 0.01f)
                        {
                            startPos = transform.position;
                        }
                    }
                    else
                    {
                        delta = target.position + offset - startPos;
                        delta.x = Mathf.Abs(delta.x);
                        delta.y = Mathf.Abs(delta.y);
                        delta.z = Mathf.Abs(delta.z);

                        if (delta.x > 1f || delta.y > 1f || delta.z > 1f)
                        {
                            StartCoroutine(SpringArm(startPos, 2));
                            isSpring = true;
                          
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SpringArm(Vector3 startP, int t)
    {
        float i = 0f;
        while (i < t)
        {
            transform.position = Vector3.Lerp(startP, target.position + offset, i);

            yield return null;
            i += 0.01f;
        }
        startPos = Vector3.zero;
        isSpring = false;
        startSpring = false;
    }

    private void SetMainMode()
    {
        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(m_Rotation);
        b_Rotation = Vector3.zero;
    }
    private void SetAimMode()
    {
        StopAllCoroutines();
        StartCoroutine(SpringArm(Vector3.zero, 0));
        transform.position = target.position;
        transform.position += Vector3.up;
        transform.position += target.forward;
        b_Rotation = target.rotation.eulerAngles;
        bowRotation = Vector3.zero;
        
    }

    public void VerticalRotationCam(Vector3 move)
    {
        bowRotation += move;
        if (bowRotation.x > 15f)
            bowRotation.x = 15f;
        else if (bowRotation.x < -15f)
            bowRotation.x = -15f;
    }
}

