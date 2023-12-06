using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{
    private Image Icon;
    private Image DIcon;
    private Image UIcon;
    private TextMeshProUGUI logo;

    private void Awake()
    {
        InitIcon();
        StartCoroutine(MoveIcon());
        Invoke("AutoNextScene", 4.5f);
    }

    private void InitIcon()
    {
        if (!GameObject.Find("Icon").TryGetComponent<Image>(out Icon))
            Debug.Log("IntroSceneManager - InitIcon - Image");
        if (!GameObject.Find("IconD").TryGetComponent<Image>(out DIcon))
            Debug.Log("IntroSceneManager - InitIcon - Image");
        if (!GameObject.Find("IconU").TryGetComponent<Image>(out UIcon))
            Debug.Log("IntroSceneManager - InitIcon - Image");
        if (!GameObject.Find("Logo").TryGetComponent<TextMeshProUGUI>(out logo))
            Debug.Log("IntroSceneManager - InitIcon - TextMeshProUGUI");
        LeanTween.scale(logo.gameObject, Vector3.zero, 0.01f);
        UIcon.fillAmount = 0f;
        DIcon.fillAmount = 0f;
    }

    private IEnumerator MoveIcon()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        LeanTween.moveLocalY(Icon.gameObject, 30f, 1.5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.moveLocalX(Icon.gameObject, 60f, 1.5f).setEase(LeanTweenType.easeInSine);

        yield return YieldInstructionCache.WaitForSeconds(0.7f);

        Color color = new Color(1f,1f, 1f, 1f);
        for(float i = 0; i < 1.01f;  i += 0.01f)
        {
            Icon.fillAmount = Mathf.Lerp(1f, 0f, i);
            UIcon.fillAmount = Mathf.Lerp(0f, 1f, i);
            DIcon.fillAmount = Mathf.Lerp(0f, 1f, i);

            color.a = Mathf.Lerp(0.7f, 1f, i);
            UIcon.color = color;
            color.a = Mathf.Lerp(0.5f, 1f, i);
            DIcon.color = color;

            yield return YieldInstructionCache.WaitForSeconds(0.01f);
        }
        LeanTween.scale(DIcon.gameObject,new Vector3(1f, 0.5f, 1f), 0.01f);

        LeanTween.rotateZ(UIcon.gameObject, -180f, 1f);
        yield return YieldInstructionCache.WaitForSeconds(1f);

        LeanTween.scale(logo.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
    }

    private void AutoNextScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
