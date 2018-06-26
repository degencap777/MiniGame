using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class StartPlayRequest : BaseRequest
{
    private bool isStartPlaying = false;
    private GamePanel gamePanel;
    private CampType campType;
    public override void Awake()
    {
        gamePanel = GetComponent<GamePanel>();
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartPlay;
        base.Awake();
    }

    void Update()
    {
        if (isStartPlaying)
        {
            isStartPlaying = false;
            facade.StartPlaying(campType);
        }
    }
    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        //Debug.Log(Enum.GetName(typeof(UIPanelType), facade.GetCurrentPanelType()));
        campType = (CampType) int.Parse(data);
        isStartPlaying = true;
    }
    
}
