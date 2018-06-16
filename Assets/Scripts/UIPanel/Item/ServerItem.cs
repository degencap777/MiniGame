using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerItem:MonoBehaviour
{

    public int ServerId;
    public string ServerName;
    public ServerListPanel serverListPanel;
    void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(OnServerClick);
    }

    private void OnServerClick()
    {
        serverListPanel.OnServerClick(ServerId);
    }
}
