using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ChangeSeatRequest : BaseRequest
{
    
    private RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ChangeSeat;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }

    public void SendRequest(int id)
    {
        base.SendRequest(id.ToString());
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.Confirm)
        {
            roomPanel.AskForConfirmAsync(int.Parse(strs[1]));
        }
        else if (returnCode == ReturnCode.NotFind)
        {
            int id = int.Parse(strs[1]);
            int index = int.Parse(strs[2]);
            roomPanel.JoinSeat(id,index);
        }
        else if(returnCode ==ReturnCode.Success)
        {
            roomPanel.OnChangeSeatResponse(int.Parse(strs[1]),int.Parse(strs[2]));
        }
    }
}
