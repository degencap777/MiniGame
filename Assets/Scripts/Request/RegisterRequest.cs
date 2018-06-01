using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RegisterRequest : BaseRequest
{

    private RegisterPanel registerPanel;
    // Use this for initialization
    public override void Awake()
    {
        registerPanel = GetComponent<RegisterPanel>();
        requestCode = RequestCode.User;
        actionCode = ActionCode.Register;
        base.Awake();
    }

    public void SendRequest(string username, string password)
    {
        string data = username + "," + password;
        SendRequest(data);
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        registerPanel.OnRegisterResponse(returnCode);
    }
}
