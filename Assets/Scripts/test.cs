using UnityEngine;
using System.Collections;
using Common;

public class test : BaseRequest
{
    public override void Awake()
    {
        actionCode = ActionCode.None;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        if (returnCode == ReturnCode.Success)
        {
            Debug.Log("HeartCheckFromClient");
        }
    }

}