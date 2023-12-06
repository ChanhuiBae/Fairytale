using UnityEngine;

public class ItemCamera : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        if (!TryGetComponent<Camera>(out cam))
            Debug.Log("ItemCamera - Awake - Camera");
    }
    
    public void InitItemCamera(int index, int type, string resoures)
    {
        transform.position += index * 10 * Vector3.right;
        cam.targetTexture = Resources.Load<RenderTexture>("RenderTexture/ItemTexture " + index);
        GameObject obj = Resources.Load<GameObject>(resoures);
        transform.GetChild(type).GetComponent<MeshFilter>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        transform.GetChild(type).GetComponent<MeshRenderer>().sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
    }

}
