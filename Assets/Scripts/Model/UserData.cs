using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {

    public UserData(string userData)
    {
        string[] strs = userData.Split(',');
        this.Id = int.Parse(strs[0]);
        this.Username = strs[1];
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
    public int Id { get; set; }
    public string Username { get; private set; }
    public int TotalCount { get; set; }
    public int WinCount { get; set; }
}
