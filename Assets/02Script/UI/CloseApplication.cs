using UnityEngine;
using UnityEngine.UI;

public class CloseApplication : MonoBehaviour
{
    private Button close;
    private void Awake()
    {
        if (!TryGetComponent<Button>(out close))
            Debug.Log("CloseApplication - Awake - Button");
        else
        {
            close.onClick.AddListener(Close);
        }
    }

    private void Close()
    {
        GameManager.Inst.SaveData();
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
