using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ShowTimerRequest : BaseRequest
{

    private GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.ShowTimer;
        gamePanel = GetComponent<GamePanel>();
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        int time = int.Parse(data);
        gamePanel.ShowTimerSync(time);
    }
}
