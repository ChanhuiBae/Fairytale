using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    private Button save;
    private void Awake()
    {
        if (!TryGetComponent<Button>(out save))
            Debug.Log("CloseApplication - Awake - Button");
        else
        {
            save.onClick.AddListener(Save);
        }

    }
    private void Save()
    {
        GameManager.Inst.SaveData();
        GameManager.Inst.MessageSave();
    }
}
