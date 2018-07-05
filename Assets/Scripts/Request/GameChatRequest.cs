using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GameChatRequest : BaseRequest {

    private GamePanel gamePanel;
    public override void Awake()
    {
        gamePanel = GetComponent<GamePanel>();
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameChat;
        base.Awake();
    }

    void Update()
    {
        if (msgQueue.Count != 0)
        {
            for (int i = 0; i < msgQueue.Count; i++)
            {
                Msg msg = msgQueue.Dequeue();
                gamePanel.AddMsgItem(msg.Username,msg.Message);
            }
        }
    }
    class Msg
    {
        public string Username;
        public string Message;

        public Msg(string username, string message)
        {
            Username = username;
            Message = message;
        }
    }
    Queue<Msg> msgQueue=new Queue<Msg>();

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split(',');
        msgQueue.Enqueue(new Msg(strs[0],strs[1]));
    }
}
