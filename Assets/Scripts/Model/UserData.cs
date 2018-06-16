using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class UserData  {

    public UserData(string userData)
    {
        string[] strs = userData.Split(',');
        this.Id = int.Parse(strs[0]);
        this.Username = strs[1];
        this.SeatIndex = int.Parse(strs[2]);
        this.CampType = SeatIndex < GameFacade.FISH_NUM ? CampType.Fish : CampType.Monkey;
        this.RoleIndex= SeatIndex < GameFacade.FISH_NUM ? 0 : SeatIndex- GameFacade.FISH_NUM+1;
    }

    //public UserData(string username)
    //{
    //    Username = username;
    //}
    public UserData(int id, string username)
    {
        Id = id;
        Username = username;
    }

    public int Id { get; private set; }
    public string Username { get; private set; }
    public int ServerId { get; set; } 
    public int SeatIndex { get; set; }
    public CampType CampType { get; private set; }
    public int RoleIndex { get; private set; }
    
}
