using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ServerListRequest : BaseRequest
{

    private ServerListPanel serverPanel;
    public override void Awake()
    {
        serverPanel = GetComponent<ServerListPanel>();
        requestCode = RequestCode.User;
        actionCode = ActionCode.ServerSelect;
        base.Awake();
    }

    public void SendRequest(int id)
    {
        string data = id.ToString();
        SendRequest(data);
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        if (returnCode == ReturnCode.Success)
        {
            
        }
    }

}
