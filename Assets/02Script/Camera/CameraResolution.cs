using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    // 레터박스를 활용하여 해상도 고정
    private Camera cam;

    private void Awake()
    {
        if (!TryGetComponent<Camera>(out cam))
            Debug.Log("CameraResolution - Awake - Camera get fail");

        Rect rt = cam.rect;

        float scale_Height = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float scale_width = 1f / scale_Height;

        if(scale_Height < 1f)
        {
            rt.height = scale_Height;
            rt.y = (1f - scale_Height) / 2f;
        }
        else
        {
            rt.width = scale_width;
            rt.x = (1f - scale_width) / 2f;
        }
        cam.rect = rt;
    }
}
