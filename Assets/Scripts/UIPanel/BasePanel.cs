using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected UIManager uiMng;
    protected GameFacade facade = GameFacade.Instance;


    public UIManager UIMng
    {
        set { uiMng = value; }
    }
    public GameFacade Facade { set { facade = value; } }

    protected void PlayClickSound()
    {
        facade.PlaySound("Click");
    }
    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }
    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 界面不显示，退出这个界面，界面被关闭
    /// </summary>
    public virtual void OnExit()
    {

        gameObject.SetActive(false);
    }
}
