using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerManager : BaseManager
{
    public PlayerManager(GameFacade facade) : base(facade) { }
    private UserData userData;
    private Transform rolePosition;
    private CampType currentCampType;
    public float margin = 1f;
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
    }

    public override void Update()
    {
        base.Update();
    }
    public void SetCurrentCampType(CampType rt)
    {
        currentCampType = rt;
    }

    public Transform GetRolePosition()
    {
        return rolePosition;
    }

    public void EnterPlaying()
    {
        //TODO 进入游戏场景后
    }
    // 通过射线检测主角是否落在地面或者物体上  
    bool IsGrounded()
    {
        //这里transform.position 一般在物体的中间位置，注意根据需要修改margin的值
        return Physics.Raycast(rolePosition.position, -Vector3.up, margin);
    }
}
