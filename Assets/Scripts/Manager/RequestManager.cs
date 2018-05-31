using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RequestManager : BaseManager
{

    public RequestManager(GameFacade facade) : base(facade) { }
    Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();

    public void AddRequest(ActionCode actionCode, BaseRequest baseRequest)
    {
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
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request类");
            return;
        }
        request.OnResponse(data);
    }
}
