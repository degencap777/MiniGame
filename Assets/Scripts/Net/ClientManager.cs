using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Common;
using UnityEngine;
/// <summary>
/// 用来管理服务器端的Socket连接
/// </summary>
public class ClientManager : BaseManager
{
    public ClientManager(GameFacade facade) : base(facade) { }
    //private const string IP = "39.108.141.81";
    private const string IP = "127.0.0.1";
    private const int PORT = 6688;
    private Socket clientSocket;
    private Message msg;
    public override void OnInit()
    {
        base.OnInit();
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        msg = new Message();
        try
        {
            clientSocket.Connect(IP, PORT);
            Start();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法连接到服务器端，请检查你的网络！" + e);
        }

    }

    private void Start()
    {
        clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
    }

    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int count = clientSocket.EndReceive(ar);
            msg.ReadMessage(count, OnProcessDataCallBack);
            Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnProcessDataCallBack(ActionCode actionCode, string data)
    {
        facade.HandleResponse(actionCode, data);
    }
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接" + e);
        }
    }
}
