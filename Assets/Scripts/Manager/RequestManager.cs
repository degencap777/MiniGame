using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RequestManager : BaseManager
{

    Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();
    public RequestManager(GameFacade facade) : base(facade){ }

    public override void OnInit()
    {
        base.OnInit();
        facade.gameObject.AddComponent<HeartCheckAction>();
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
        BaseRequest request = requestDict.TryGet(actionCode);
        if (request == null)
        {
            if (request == null)
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
