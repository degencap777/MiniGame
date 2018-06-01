using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerManager : BaseManager
{
    public PlayerManager(GameFacade facade) : base(facade) { }
    private UserData userData;
    private Transform rolePositions;
    private CampType currentCampType;
    public UserData UserData
    {
        get { return userData; }
        set { userData = value; }
    }

    public void UpdateResult(int totalCount, int winCount)
    {
        //userData.TotalCount = totalCount;
        //userData.WinCount = winCount;
    }
    public override void OnInit()
    {
        base.OnInit();
        //rolePositions = GameObject.Find("RolePositions").transform;
    }

    public override void Update()
    {
        base.Update();
    }
    public void SetCurrentCampType(CampType rt)
    {
        currentCampType = rt;
    }
}
