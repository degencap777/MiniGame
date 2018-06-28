using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class Player {

	public UserData UserData { get; set; }
    public List<int> RoleInstanceIdList { get; set; }
    public int SeatIndex { get; set; }
    public bool IsLocal { get; set; }
    public CampType CampType { get; private set; }
    public Vector3 SpawnPosition { get; set; }
    private int _currentInstanceId;

    public int CurrentRoleInstanceId
    {
        get { return _currentInstanceId; }
        set
        {
            _currentInstanceId = value;
            if(RoleInstanceIdList.Contains(_currentInstanceId))
                return;
            RoleInstanceIdList.Add(_currentInstanceId);
        }
    }

    public GameObject Reference;
    public Vector3 DeadPostion { get; set; }
    public GameObject Dead { get; set; }

    public Player(UserData userData, int seatIndex)
    {
        UserData = userData;
        RoleInstanceIdList = new List<int>();
        SeatIndex = seatIndex;
        IsLocal = false;
        this.CampType = SeatIndex < GameFacade.FISH_NUM ? CampType.Fish : CampType.Monkey;
    }
}
