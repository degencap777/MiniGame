using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GameOverRequest : BaseRequest
{
    private GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameOver;
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
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        gamePanel.GameOverAsync(returnCode);
    }
}
