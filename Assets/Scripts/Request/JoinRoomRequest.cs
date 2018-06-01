using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class JoinRoomRequest : BaseRequest
{
    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        roomListPanel = GetComponent<RoomListPanel>();
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;
        base.Awake();
    }

    public void SendRequest(int id)
    {
        base.SendRequest(id.ToString());
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split('-');
        string[] strs2 = strs[0].Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs2[0]);
        List<UserData> udList=new List<UserData>();
        if (returnCode == ReturnCode.Success)
        {
            string[] udStrArray = strs[1].Split('|');
            foreach (var udStr in udStrArray)
            {
                UserData ud = new UserData(udStr);
                udList.Add(ud);
            }
            CampType CampType = (CampType)int.Parse(strs2[1]);
            facade.SetCurrentCampType(CampType);
        }
        roomListPanel.OnJoinResponse(returnCode,udList);
    }
}
