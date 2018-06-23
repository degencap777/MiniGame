using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class QuitBattleRequest : BaseRequest {
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.QuitBattle;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }
}
