using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitileSceneManager : MonoBehaviour
{
    private GameObject wellcomPopup;
    private TextMeshProUGUI nickname;
    private string newNickName;
    private TextMeshProUGUI warningText;
    private Button playBtn;
    private Button createBtn;
    private Button resignBtn;
    private TMP_InputField inputText;

    private bool havePlayerInfo;

    private void Awake()
    {
        InitTitleScene();
    }

    private void InitTitleScene()
    {

        wellcomPopup = GameObject.Find("WellcomPopup");
        if (wellcomPopup == null)
            Debug.Log("TitileSceneManager - InitTitileScene - wellcomPopup");
        if (!GameObject.Find("InputNickname").TryGetComponent<TMP_InputField>(out inputText))
            Debug.Log("TitileSceneManager - InitTitileScene - InputField");
        if (!GameObject.Find("NicknameText").TryGetComponent<TextMeshProUGUI>(out nickname))
            Debug.Log("TitileSceneManager - InitTitileScene - TextMeshProUGUI");
        if (!GameObject.Find("WarningText").TryGetComponent<TextMeshProUGUI>(out warningText))
            Debug.Log("TitileSceneManager - InitTitileScene - TextMeshProUGUI");
        if (!GameObject.Find("PlayBtn").TryGetComponent<Button>(out playBtn))
            Debug.Log("TitileSceneManager - InitTitileScene - Button");
        if (!GameObject.Find("CreateBtn").TryGetComponent<Button>(out createBtn))
            Debug.Log("TitileSceneManager - InitTitileScene - Button");
        if (!GameObject.Find("ResignBtn").TryGetComponent<Button>(out resignBtn))
            Debug.Log("TitileSceneManager - InitTitileScene - Button");

        if (GameManager.Inst.CheckData())
        {
            nickname.text = GameManager.Inst.PlayerInfo.userNickname;
            playBtn.onClick.AddListener(Play);
            resignBtn.onClick.AddListener(Resign);
            createBtn.onClick.RemoveListener(CreateUserInfo); 
            inputText.onValueChanged.RemoveListener(InputField);
            havePlayerInfo = true;
        }
        else
        {
            wellcomPopup.SetActive(false);
            inputText.onValueChanged.AddListener(InputField);
            createBtn.onClick.AddListener(CreateUserInfo);
            playBtn.onClick.RemoveListener(Play);
            resignBtn.onClick.RemoveListener(Resign);
            havePlayerInfo = false;
        }
    }

    public void Play()
    {
        if (havePlayerInfo)
        {
            GameManager.Inst.AsyncLoadNextScene(SceneName.HomeScene);
        }
    }

    public void Resign()
    {
        GameManager.Inst.DeleteData();
        InitTitleScene();
    }

    public void InputField(string input)
    {
        newNickName = input;
    }

    private bool IsEnglish(char ch)
    {
        if ((0x61 <= ch && ch <= 0x7A) || (0x41 <= ch && ch <= 0x5A))
            return true;
        else
            return false;
    }
    bool IsNumeric(char ch)
    {
        if (0x30 <= ch && ch <= 0x39)
            return true;
        else
            return false;
    }

    private bool IsAllowCharacters(string text)
    {
        for(int i = 0; i < text.Length; i++)
        {
            if (IsEnglish(text[i]) || IsNumeric(text[i]))
            {
                continue;
            }
            else
                return false;
        }
        return true;
    }

    public void CreateUserInfo()
    {
        if(newNickName == null)
        {
            WarningText();
        }
        else if (newNickName.Length >= 2 && newNickName.Length < 17 && IsAllowCharacters(newNickName))
        {
            wellcomPopup.SetActive(true);
            GameManager.Inst.CreateUserData(newNickName);
            InitTitleScene();
        }
        else
        {
            WarningText();
        }
    }

    #region WarningText
    private void WarningText()
    {
        Color fromColor = Color.black;
        Color toColor = Color.red;

        warningText.enabled = true;
        LeanTween.value(warningText.gameObject, UpdateValue, fromColor, toColor, 1f)
            .setEase(LeanTweenType.easeInOutQuad);
        LeanTween.value(warningText.gameObject, UpdateValue, toColor, fromColor, 1f)
            .setDelay(1f).setEase(LeanTweenType.easeInOutQuad);
    }

    private void UpdateValue(Color val)
    {
        warningText.color = val;
    }
    #endregion
}
