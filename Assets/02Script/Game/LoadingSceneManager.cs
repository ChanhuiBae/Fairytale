using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    private Image loadingBar;
    private TextMeshProUGUI tipText;

    private void Awake()
    {
        if (!GameObject.Find("Loading").TryGetComponent<Image>(out loadingBar))
            Debug.Log("LoadingSceneManager - Awake - Image");
        if (!GameObject.Find("Tip").TryGetComponent<TextMeshProUGUI>(out tipText))
            Debug.Log("LoadingSceneManager - Awake - TextMeshProUGUI");
        loadingBar.fillAmount = 0f;
        StartCoroutine("LoadAsyncScene");
    }

    IEnumerator LoadAsyncScene()
    {
        yield return null;
        tipText.text = GameManager.Inst.GetTipMessage((int)GameManager.Inst.NextScene);
        yield return YieldInstructionCache.WaitForSeconds(1f);

        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(GameManager.Inst.NextScene.ToString());
        asyncScene.allowSceneActivation = false;
        float timeC = 0f;

        while (!asyncScene.isDone)
        {
            yield return null;
            timeC += Time.deltaTime;

            if (asyncScene.progress >= 0.9f)
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timeC);
                if (loadingBar.fillAmount >= 0.99f)
                {
                    asyncScene.allowSceneActivation = true;
                }
            }
            else
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, asyncScene.progress, timeC);
                if (loadingBar.fillAmount >= asyncScene.progress)
                {
                    timeC = 0f;
                }
            }
        }

    }
}
