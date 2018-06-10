using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RequestManager : BaseManager
{

    Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();
    private float heartCheckTime = 0;
    public RequestManager(GameFacade facade) : base(facade){ }

    public override void OnInit()
    {
        base.OnInit();
        facade.gameObject.AddComponent<HeartCheckRequest>();
        facade.gameObject.AddComponent<test>();
    }

    public override void Update()
    {
        base.Update();
        heartCheckTime += Time.deltaTime;
        if (heartCheckTime >= 2)
        {
            requestDict.TryGet(ActionCode.None).SendRequest();
            heartCheckTime = 0;
        }
    }

    public void AddRequest(ActionCode actionCode, BaseRequest baseRequest)
    {
        if(requestDict.ContainsKey(actionCode))return;
        requestDict.Add(actionCode, baseRequest);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestDict.Remove(actionCode);
    }
    
    public void HandleResponse(ActionCode actionCode, string data)
    {
        if (actionCode == ActionCode.ChangeSeat)
        {
            foreach (var r in requestDict)
            {
                Debug.Log(Enum.GetName(typeof(ActionCode),r.Key));
            }
        }
        BaseRequest request = requestDict.TryGet(actionCode);
        if (request == null)
        {
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request和Action类");
            return;
        }
        request.OnResponse(data);
    }

    //public void DebugRequestDict()
    //{
    //    BaseRequest request = requestDict.TryGet(ActionCode.None);
    //    if (request == null)
    //    {
    //        Debug.LogWarning("无法得到None");

    //    }
    //}
}
