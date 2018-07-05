using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class DoDestroy : MonoBehaviour
{

    private float time = 0;
    private Object obj;
    private Action action;
    public void Set(Action action, Object obj,float time=0)
    {
        this.time = time;
        this.obj = obj;
        this.action=action;
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
        action();
    }
}
