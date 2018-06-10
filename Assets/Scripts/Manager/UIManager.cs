using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager {

    public UIManager(GameFacade facade) : base(facade)
    {
        ParseUIPanelTypeJson();
    }

    //private static UIManage _instance;

    //public static UIManage Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance=new UIManage();
    //        }
    //        return _instance;
    //    }
    //}

    private Transform canvasTransform;

    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }
    private Dictionary<UIPanelType, string> panelPathDict = new Dictionary<UIPanelType, string>();//存储所有面板prefab路径
    private Dictionary<UIPanelType, BasePanel> panelDict = new Dictionary<UIPanelType, BasePanel>();//保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack = new Stack<BasePanel>();
    private MessagePanel msgPanel;
    private UIPanelType panelTypeToPush = UIPanelType.None;
    private UIPanelType currentPanelType = UIPanelType.None;
    public override void OnInit()
    {
        base.OnInit();
        PushPanel(UIPanelType.Message);
        PushPanel(UIPanelType.Login);
    }

    public override void Update()
    {
        base.Update();
        if (panelTypeToPush != UIPanelType.None)
        {
            PushPanel(panelTypeToPush);
            panelTypeToPush = UIPanelType.None;
        }
    }

    public BasePanel PushPanel(UIPanelType panelType)
    {
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause();
        }
        BasePanel panel = GetPanel(panelType);
        panel.OnResume();
        panelStack.Push(panel);
        currentPanelType = panelType;
        return panel;
    }
    public void PushPanelSync(UIPanelType panelType)
    {
        panelTypeToPush = panelType;
    }
    public void PopPanel()
    {
        if (panelStack.Count <= 0) return;
        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();
        if (panelStack.Count <= 0) return;
        BasePanel topPanel2 = panelStack.Peek();
        topPanel2.OnResume();
    }
    private BasePanel GetPanel(UIPanelType panelType)
    {
        BasePanel panel = panelDict.TryGet(panelType);
        if (panel == null)
        {
            string path = panelPathDict.TryGet(panelType);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);
            panel = instPanel.GetComponent<BasePanel>();
            panel.Facade = facade;
            panel.UIMng = this;
            panel.OnEnter();
            panelDict.Add(panelType, instPanel.GetComponent<BasePanel>());
            return instPanel.GetComponent<BasePanel>();
        }
        else
        {
            return panel;
        }
    }
    [Serializable]
    class UIPanelTypeJson
    {
        public List<UIPanelInfo> infoList = new List<UIPanelInfo>();
    }
    private void ParseUIPanelTypeJson()
    {
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");
        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);
        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            panelPathDict.Add(info.PanelType, info.Path);
        }
    }

    public void InjectMsgPanel(MessagePanel msgPanel)
    {
        this.msgPanel = msgPanel;
    }
    public void ShowMessage(string msg)
    {
        if (msgPanel == null)
        {
            Debug.Log("无法显示提示信息，MsgPanel为空");
            return;
        }
        msgPanel.ShowMessageAsync(msg);
    }
    public void ShowMessageSync(string msg)
    {
        if (msgPanel == null)
        {
            Debug.Log("无法显示提示信息，MsgPanel为空");
            return;
        }
        msgPanel.ShowMessageAsync(msg);
    }

    public UIPanelType GetCurrentPanelType()
    {
        return currentPanelType;
    }

    public void DestroyPanel(UIPanelType uiPanelType)
    {
        UnityEngine.Object.DestroyImmediate(panelDict[uiPanelType].transform.gameObject);
        panelDict.Remove(uiPanelType);
    }
}
