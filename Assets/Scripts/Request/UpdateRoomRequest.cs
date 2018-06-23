using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class UpdateRoomRequest : BaseRequest
{

    private RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.UpdateRoom;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        Debug.Log("UpdateDATA: "+data);
        if (data == "null")
        {
            roomPanel.OnExitResponse();
            return;
        }
        List<UserData> udList=new List<UserData>();
        string[] udStrArray = data.Split('|');
        foreach (var udStr in udStrArray)
        {
            udList.Add(new UserData(udStr));
        }
        roomPanel.SetAllPlayerSync(udList);
    }

}
