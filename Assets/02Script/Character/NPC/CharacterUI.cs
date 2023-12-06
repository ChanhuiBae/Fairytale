using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private GameObject npcPointPrefab;
    private Sprite source;
    private GameObject npcPoint;
    private Vector3 pointOffset = new Vector3(0, 6f, 0);
    private HomeSceneManager homeSceneManager;

    public void Awake()
    {
        npcPointPrefab = Resources.Load<GameObject>("Prefabs/NPCPoint");
        switch (transform.tag.ToString())
        {
            case "Shop":
                source = Resources.Load<Sprite>("ItemIcon/itemicon_flask_red");
                if (!GameObject.Find("HomeSceneManager").TryGetComponent<HomeSceneManager>(out homeSceneManager))
                    Debug.Log("CharacterUI - Awake - HomeSceneManager");
                break;
            case "Blacksmith":
                source = Resources.Load<Sprite>("ItemIcon/itemicon_anvil");
                if (!GameObject.Find("HomeSceneManager").TryGetComponent<HomeSceneManager>(out homeSceneManager))
                    Debug.Log("CharacterUI - Awake - HomeSceneManager");
                break;
        }
        
        npcPoint = Instantiate(npcPointPrefab, transform.position + pointOffset, Quaternion.identity, transform);

        Image noneImage;
        if (!npcPoint.transform.GetChild(0).GetChild(0).GetChild(0).TryGetComponent<Image>(out noneImage))
            Debug.Log("CharacterUI - Awake - Image");
        else
        {
            noneImage.sprite = source;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && transform.tag.ToString() == "Shop")
        {
            homeSceneManager.ShowPotionShopPopup();
            GameManager.Inst.PlayerIsController(false);
        }
        else if(other.CompareTag("Player") && transform.tag.ToString() == "Blacksmith")
        {
            homeSceneManager.ShowSmithyPopup();
            GameManager.Inst.PlayerIsController(false);
        }
    }
}
