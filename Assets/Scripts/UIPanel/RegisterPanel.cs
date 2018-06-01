using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private InputField usernameIF;
    private InputField passwordIF;
    private InputField rePasswordIF;
    private RegisterRequest registerRequest;

    void Awake()
    {
        registerRequest = GetComponent<RegisterRequest>();
        usernameIF = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>();
        passwordIF = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>();
        rePasswordIF = transform.Find("RePasswordLabel/RePasswordInput").GetComponent<InputField>();

        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);
    }
    private void OnCloseClick()
    {
        PlayClickSound();
        HideAnim();
    }
    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }
    private void OnRegisterClick()
    {
        PlayClickSound();
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            msg += "用户名不能为空";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg += "密码不能为空";
        }
        if (passwordIF.text != rePasswordIF.text)
        {
            msg += "密码不一致";
        }
        if (msg != "")
        {
            uiMng.ShowMessage(msg);
            return;
        }
        registerRequest.SendRequest(usernameIF.text, passwordIF.text);
    }

    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiMng.ShowMessageSync("注册成功");
        }
        else
        {
            uiMng.ShowMessageSync("用户名重复");
        }
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
        transform.DOLocalMove(new Vector3(1000, 0, 0), 0.2f).OnComplete(() => uiMng.PopPanel());
    }
}
