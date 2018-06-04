using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        roomPanel = GetComponent<RoomPanel>();
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
        base.Awake();
    }
    public void SetPanel(BasePanel panel)
    {
        roomPanel = panel as RoomPanel;
    }
    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        CampType CampType = (CampType)int.Parse(strs[1]);
        facade.SetCurrentCampType(CampType);
        if (returnCode == ReturnCode.Success)
        {
            roomPanel.SetLocalPlayerSync();
        }
    }
}
