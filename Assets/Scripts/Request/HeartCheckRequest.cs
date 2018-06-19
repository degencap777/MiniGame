using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class HeartCheckRequest : BaseRequest {

    private float heartCheckTime = 0;
    // Use this for initialization
    public override void Awake()
    {
        requestCode = RequestCode.None;
        actionCode = ActionCode.HeartCheck;
        base.Awake();
    }

    void Update()
    {

        heartCheckTime += Time.deltaTime;
        if (heartCheckTime >= 2)
        {
            SendRequest();
            heartCheckTime = 0;
        }
    }
    public override void SendRequest()
    {
        base.SendRequest("");
    }

}
