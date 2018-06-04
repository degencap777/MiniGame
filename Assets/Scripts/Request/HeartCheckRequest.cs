using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class HeartCheckRequest : BaseRequest {

    // Use this for initialization
    public override void Awake()
    {
        requestCode = RequestCode.None;
        actionCode = ActionCode.HeartCheck;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

}
