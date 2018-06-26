using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class Test : BaseRequest
{
    public GameObject go;
    private float timer = 0;
    public override void Awake()
    {
        requestCode = RequestCode.None;
        actionCode = ActionCode.Test;
        base.Awake();
    }

    // Update is called once per frame
	void Update ()
	{
        //if(go!=null)
        // SendRequest("tag:"+go.tag);
        //else
        //{
        //    SendRequest("go不存在" );
        //}
	    timer += Time.deltaTime;
	    if (timer > 1)
	    {
	        timer = 0;
	        string tag = null;
	        GameObject go = null;
	        go = GameObject.Find("Cube (1)");
	        SendRequest("++++");
	        if (go != null)
	        {
	            SendRequest("???");
                tag = go.tag;
	            SendRequest("----");
            }
	        if (tag != null)
	            SendRequest(tag);

	        //if (GameObject.Find("Cube (1)") != null)
	        //{
	        //    SendRequest(GameObject.Find("Cube (1)").tag);
	        //}
            
        }
	}
    
}
