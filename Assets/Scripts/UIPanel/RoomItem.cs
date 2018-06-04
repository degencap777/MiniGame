using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{

    private Text username;

    private Button joinButton;

    private int id;

    private RoomListPanel panel;

    private Room room;

    private int clientNum;
    // Use this for initialization
    void Awake()
    {
        joinButton = transform.Find("JoinButton").GetComponent<Button>();
        username = transform.Find("name").GetComponent<Text>();
        joinButton.onClick.AddListener(OnJoinClick);
    }
    
    public void SetRoomInfo( Room room, RoomListPanel panel)
    {
        this.id = room.RoomOwner.Id;
        this.username.text = room.RoomOwner.Username;
        if(room.RoomOwner.Username == null)
            Debug.Log(1);
        this.panel = panel;
        this.room = room;
        this.clientNum = room.ClientNum;
    }
    private void OnJoinClick()
    {
        Debug.Log("RoomId:"+id);
        panel.OnJoinClick(id);
    }

    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
