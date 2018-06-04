using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class LoadGameRequest : BaseRequest {

    private LoadGamePanel loadGamePanel;
    public override void Awake()
    {
        loadGamePanel = GetComponent<LoadGamePanel>();
        requestCode = RequestCode.Game;
        actionCode = ActionCode.LoadGame;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        loadGamePanel.OnLoadGameResponse();
    }
}
