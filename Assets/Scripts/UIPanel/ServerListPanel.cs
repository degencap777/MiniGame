using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ServerListPanel : BasePanel
{
    public GameObject GridContent;
    private Dictionary<string,Transform> servers = new Dictionary<string, Transform>();
    private int serverId;
    private ServerListRequest serverRequest;
    void Awake()
    {
        serverRequest = GetComponent<ServerListRequest>();
        for (int i = 0; i < GridContent.transform.childCount; i++)
        {
            Transform server = GridContent.transform.GetChild(i);
            string name = server.GetChild(0).GetComponent<Text>().text;
            servers.Add(name,server);
        }
    }

    void Start()
    {
    }

    public void OnServerClick()
    {
        PlayClickSound();
        serverRequest.SendRequest(serverId);
        uiMng.PushPanelSync(UIPanelType.RoomList);
    }

    public void OnServerSelectResponse()
    {
        uiMng.PushPanelSync(UIPanelType.RoomList);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();
    }
    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }
    public override void OnPause()
    {
        base.OnPause();
        HideAnim();
    }
    public override void OnExit()
    {
        base.OnExit();
        HideAnim();
    }

    private void EnterAnim()
    {
        //transform.localScale = Vector3.zero;
        //transform.DOScale(1, 0.2f);
        //transform.localPosition = new Vector3(1000, 0, 0);
        //transform.DOLocalMove(Vector3.zero, 0.2f);
    }

    private void HideAnim()
    {
        gameObject.SetActive(false);
        //transform.DOScale(0, 0.2f);
        //transform.DOLocalMove(new Vector3(1000, 0, 0), 0.2f).OnComplete(() => gameObject.SetActive(false));
    }
}
