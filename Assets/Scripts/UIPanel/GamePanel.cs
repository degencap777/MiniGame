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
    private Knapsack knapsack;
    private SkillJoystickItem skillJoystickItem;
    void Awake()
    {
        Joystick = transform.Find("Joystick").GetComponent<ETCJoystick>();
        showTimerRequest = GetComponent<ShowTimerRequest>();
        startPlayRequest = GetComponent<StartPlayRequest>();
        knapsack = transform.Find("KnapsackPanel").GetComponent<Knapsack>();
        skillJoystickItem = transform.Find("SkillJoystickItem").GetComponent<SkillJoystickItem>();
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

    public void UseSkill(string skillName,string axis=null)
    {
        facade.UseSkill(skillName,axis);
    }

    public void UseSkillSync(float coldTime)
    {
        skillJoystickItem.UseSkillSync(coldTime);
    }
    public void UseItem(string itemName, string point = null)
    {
        facade.UseItem(itemName, point);
    }

    public void UseItemSync(bool isUse)
    {
        knapsack.UseItemSync(isUse);
    }

    public void SetPlayer(GameObject player)
    {
        knapsack.Player = player;
    }
}
