using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RoomChatRequest : BaseRequest
{

    private RoomPanel roomPanel;
    public override void Awake()
    {
        roomPanel = GetComponent<RoomPanel>();
        requestCode = RequestCode.Room;
        actionCode = ActionCode.RoomChat;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        roomPanel.AddMsgItemSync(data);
    }
}
