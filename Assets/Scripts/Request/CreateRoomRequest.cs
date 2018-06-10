using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
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
        Debug.Log("CreateRoomDATA: "+data);
        string[] strs = data.Split('-');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        UserData ud=new UserData(strs[1]);
        if (returnCode == ReturnCode.Success)
        {
            roomPanel.SetRoomOwnerSync(ud);
        }
    }
}
