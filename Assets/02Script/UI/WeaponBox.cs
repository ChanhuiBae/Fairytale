using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBox : MonoBehaviour
{
    private Button button;
    private Image chose;
    private TextMeshProUGUI ATKValue;
    private TextMeshProUGUI durabilityValue;
    private TextMeshProUGUI priceText;
    private TextMeshProUGUI priceValue;

    private MeshFilter mesh;
    private MeshRenderer metaral;
    private int ATK;
    private int durability;
    private int price;

    private void Awake()
    {
        if (!transform.Find("Button").TryGetComponent<Button>(out button))
            Debug.Log("WeaponBox - Awake - Button");
        if (!transform.Find("Chose").TryGetComponent<Image>(out chose))
            Debug.Log("WeaponBox - Awake - Image");
        if (!transform.Find("ATK").TryGetComponent<TextMeshProUGUI>(out ATKValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("Durability").TryGetComponent<TextMeshProUGUI>(out durabilityValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("Price").TryGetComponent<TextMeshProUGUI>(out priceValue))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("PriceText").TryGetComponent<TextMeshProUGUI>(out priceText))
            Debug.Log("WeaponBox - Awake - TextMeshProUGUI");
        if (!transform.Find("Weapon").TryGetComponent<MeshFilter>(out mesh))
            Debug.Log("WeaponBox - Awake - MeshFilter");
        if(!transform.Find("Weapon").TryGetComponent<MeshRenderer>(out metaral))
            Debug.Log("WeaponBox - Awake - MeshRenderer");
    }

    private void InitWeaponBox()
    {
        transform.LeanScale(Vector3.zero, 0f);
    }

    private void SeeWeaponBox()
    {

    }
}
