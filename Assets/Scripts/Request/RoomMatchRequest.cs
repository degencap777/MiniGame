using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RoomMatchRequest : BaseRequest {
    
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.MatchRoom;
        base.Awake();
    }

    public void SendRequest(int rank)
    {
        base.SendRequest(rank.ToString());
    }

    public override void OnResponse(string data)
    {
        
    }
}
