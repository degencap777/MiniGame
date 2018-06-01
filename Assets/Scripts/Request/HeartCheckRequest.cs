using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class HeartCheckAction : BaseRequest {

    // Use this for initialization
    public override void Awake()
    {
        actionCode = ActionCode.HeartCheck;
        base.Awake();
    }
    
}
