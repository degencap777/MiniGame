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
        joinButton.onClick.AddListener(OnJoinClick);
    }
    
    public void SetRoomInfo( Room room, RoomListPanel panel)
    {
        this.id = room.RoomOwner.Id;
        this.username.text = room.RoomOwner.Username;
        this.panel = panel;
        this.room = room;
        this.clientNum = room.ClientNum;
    }
    private void OnJoinClick()
    {
        panel.OnJoinClick(id);
    }

    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
