using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GamePanel : BasePanel
{
    public ETCJoystick Joystick { get; private set; }
    private ShowTimerRequest showTimerRequest;
    private StartPlayRequest startPlayRequest;
    void Awake()
    {
        Joystick = transform.Find("Joystick").GetComponent<ETCJoystick>();
        showTimerRequest = GetComponent<ShowTimerRequest>();
        startPlayRequest = GetComponent<StartPlayRequest>();
    }

    void Start()
    {
        showTimerRequest.SendRequest();
    }
	// Update is called once per frame
	void Update () {


	}

    internal void ShowTimerSync(int time)
    {
        throw new NotImplementedException();
    }
}
