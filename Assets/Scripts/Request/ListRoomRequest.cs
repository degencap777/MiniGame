using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ListRoomRequest : BaseRequest
{
    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        roomListPanel = GetComponent<RoomListPanel>();
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ListRoom;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        List<Room> roomList = new List<Room>();
        List<UserData> udList=new List<UserData>();
        if (data != "0")
        {
            string[] roomArray = data.Split('|');
            foreach (string roomInfo in roomArray)
            {
                string[] strs = roomInfo.Split('-');
                roomList.Add(new Room(new UserData(strs[0]),int.Parse(strs[1]) ));
            }
        }
        roomListPanel.LoadRoomItemSync(roomList);
    }
}
