using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using Common;
using UnityEngine;

public class DestroyRequest : BaseRequest
{
    private GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Destroy;
        gamePanel = GetComponent<GamePanel>();
        base.Awake();
    }
    Queue<Vector2> posQueue=new Queue<Vector2>();
    void Update()
    {
        if (posQueue.Count != 0)
        {
            for (int i = 0; i < posQueue.Count; i++)
            {
                Vector2 pos = posQueue.Dequeue();
                gamePanel.DestroyCube(pos);
            }
        }
    }
    public void SendRequest(Vector3 pos)
    {
        base.SendRequest(pos.x+","+pos.z);
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        Vector2 pos = UnityTools.ParseVector2(data);
        posQueue.Enqueue(pos);
    }
}
