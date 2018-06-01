using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class BaseAction : MonoBehaviour {

    protected ActionCode actionCode = ActionCode.None;

    protected GameFacade _facade;

    protected GameFacade facade
    {
        get
        {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }
    }

    // Use this for initialization
    public virtual void Awake()
    {
        facade.AddAction(actionCode, this);
    }
    
    public virtual void OnResponse(string data) { }

    public virtual void OnDestroy()
    {
        if (facade != null)
            facade.RemoveAction(actionCode);
    }
}
