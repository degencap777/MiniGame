using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private Button closeButton;
    private InputField usernameInputField;
    private InputField passwordInputField;
    private LoginRequest loginRequest;

    void Awake()
    {
        loginRequest = GetComponent<LoginRequest>();

        usernameInputField = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>();
        passwordInputField = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>();
        transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(OnQuitClick);

        transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(OnLoginClick);
        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();
    }

    private void OnLoginClick()
    {
        PlayClickSound();
        string msg = "";
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            msg += "用户名不能为空";
        }
        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            msg += "密码不能为空";
        }
        if (msg != "")
        {
            uiMng.ShowMessage(msg);
            return;
        }
        loginRequest.SendRequest(usernameInputField.text, passwordInputField.text);
    }

    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiMng.PushPanelSync(UIPanelType.RoomList);
        }
        else if (returnCode == ReturnCode.NotFind)
        {
            uiMng.ShowMessageSync("用户名或密码错误无法登录，请重新输入");
        }
        else
        {
            uiMng.ShowMessageSync("该用户已经登录");
        }
    }
    private void OnRegisterClick()
    {
        PlayClickSound();
        uiMng.PushPanel(UIPanelType.Register);
    }
    private void OnQuitClick()
    {
        Application.Quit();
    }

    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }
    public override void OnPause()
    {
        base.OnPause();
        HideAnim();
    }
    public override void OnExit()
    {
        base.OnExit();
        HideAnim();
    }

    private void EnterAnim()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f);
        transform.localPosition = new Vector3(1000, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.2f);
    }

    private void HideAnim()
    {

        transform.DOScale(0, 0.2f);
        transform.DOLocalMove(new Vector3(1000, 0, 0), 0.2f).OnComplete(() => gameObject.SetActive(false));
    }
}
