using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class QuitGameRequest : BaseRequest
{
    private GamePanel gamePanel;
    private bool isQuitGame = false;
    private int id = -1;
    public override void Awake()
    {
        gamePanel = GetComponent<GamePanel>();
        requestCode = RequestCode.Game;
        actionCode = ActionCode.QuitGame;
        base.Awake();
    }

    void Update()
    {
        if (isQuitGame)
        {
            gamePanel.QuitGame();
        }
        if (id != -1)
        {
            gamePanel.QuitGame(id);
        }
    }
    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split('|');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.NotFind)
        {
            isQuitGame = true;
        }
        else
        {
            id=Int32.Parse(strs[1].Split(',')[0]);
        }
    }
}
