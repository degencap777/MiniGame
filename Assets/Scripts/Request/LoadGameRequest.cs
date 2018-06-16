using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class LoadGameRequest : BaseRequest {

    private LoadGamePanel loadGamePanel;
    public override void Awake()
    {
        loadGamePanel = GetComponent<LoadGamePanel>();
        requestCode = RequestCode.Game;
        actionCode = ActionCode.LoadGame;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("");
    }

    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] str = data.Split('-');
        ReturnCode returnCode = (ReturnCode)int.Parse(str[0]);
        switch (returnCode)
        {
            case ReturnCode.Success:
                UserData localUd = new UserData(str[1]);
                string[] str2 = str[2].Split('|');
                List<UserData> userDatas = new List<UserData>();
                foreach (var ud in str2)
                {
                    userDatas.Add(new UserData(ud));
                }
                facade.InitPlayerData(localUd, userDatas);
                loadGamePanel.OnLoadGameResponse();
                break;

        }
    }
}
