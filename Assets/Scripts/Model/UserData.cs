using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {

    public UserData(string userData)
    {
        string[] strs = userData.Split(',');
        this.Id = int.Parse(strs[0]);
        this.Username = strs[1];
        this.TotalCount = int.Parse(strs[2]);
        this.WinCount = int.Parse(strs[3]);
    }
    public UserData(string username, int totalCount, int winCount)
    {
        Username = username;
        TotalCount = totalCount;
        WinCount = winCount;
    }
    public UserData(int id, string username, int totalCount, int winCount)
    {
        Id = id;
        Username = username;
        TotalCount = totalCount;
        WinCount = winCount;
    }
    public int Id { get; set; }
    public string Username { get; private set; }
    public int TotalCount { get; set; }
    public int WinCount { get; set; }
}
