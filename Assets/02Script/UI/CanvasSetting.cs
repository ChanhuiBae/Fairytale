using UnityEngine;

public class CanvasSetting : MonoBehaviour
{
    private Canvas canvas;
    private void Awake()
    {
        if (!TryGetComponent<Canvas>(out canvas))
            Debug.Log("CanvasSetting - Awake - Canvas get fail");
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;
        }
    }
}