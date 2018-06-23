using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : BasePanel
{
    private Text text;
    private float showTime = 1;
    private string message = null;
    void Update()
    {
        if (message != null)
        {
            ShowMessage(message);
            message = null;
        }
        SetUiIndex();
    }

    private void SetUiIndex()
    {
        int count = transform.parent.childCount;
            //参数为物体在当前所在的子物体列表中的顺序
            //count-1指把child物体在当前子物体列表的顺序设置为最后一个，0为第一个
        transform.SetSiblingIndex(count - 1);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        text = GetComponent<Text>();
        text.enabled = false;
        uiMng.InjectMsgPanel(this);
    }


    public void ShowMessageAsync(string msg)
    {
        message = msg;
    }
    public void ShowMessage(string msg)
    {
        text.canvasRenderer.SetAlpha(1);
        text.text = msg;
        text.enabled = true;
        Invoke("Hide", showTime);
    }

    private void Hide()
    {
        text.CrossFadeAlpha(0, 1, false);
    }
}
