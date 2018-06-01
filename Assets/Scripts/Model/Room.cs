using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Room(UserData roomOwner,int clientNum)
    {
        this.ClientNum = clientNum;
        this.RoomOwner = roomOwner;
    }
    public UserData RoomOwner { get; set; }
    public List<UserData> UdList{ get; set; }
    public int ClientNum { get; set; }
}
