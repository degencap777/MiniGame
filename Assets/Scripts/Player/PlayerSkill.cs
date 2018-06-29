using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{

    private CharactorSkills charactorSkills;
    private PlayerManager playerManager;
    private Rigidbody rb;
    public float RotateSpeed { get { return playerInfo.TurnSpeed; } }
    private Vector3 direction=Vector3.zero;
    private PlayerInfo playerInfo;
    private float joystickColdTime = 0;
    private Skill currentSkill = null;
	// Use this for initialization
	void Start ()
	{
	    charactorSkills = GetComponent<CharactorSkills>();
	    rb = transform.GetComponent<Rigidbody>();
	    playerInfo = GetComponent<PlayerInfo>();
	}
	
	// Update is called once per frame
	void Update () {
	    //if (playerInfo.CurrentState==PlayerInfo.State.UseSkill)
        if(playerInfo.ToUseSkill&&playerInfo.anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
	    {
	        if (IsTurnDone())
	        {
	            UseSkill();
	            playerInfo.ToUseSkill = false;
	            playerInfo.anim.SetTrigger("UseSkill");
	        }
            if(direction!=Vector3.zero)
                TurnToUseSkill();
	    }
	    else
	    {
	        currentSkill = null;
	        direction = Vector3.zero;
	        joystickColdTime = 0;

	    }
	}

    private void UseSkill(Skill useSkill=null)
    {
        Skill skill = useSkill ?? currentSkill;
        if (joystickColdTime != 0&&playerInfo.Player.IsLocal)
            GameFacade.Instance.UseSkillSync(skill.GetType().Name,joystickColdTime);
        charactorSkills.Skills[skill.GetType().Name].Direction = direction;
        charactorSkills.UseSkill(skill.GetType().Name);
    }
    public void StartUseSkill(Skill skill,string axis=null)
    {
        if (axis != null)
        {
            joystickColdTime = skill.parameters.TryGet("CD");
            //playerInfo.CurrentState = PlayerInfo.State.UseSkill;
            playerInfo.ToUseSkill = true;
            Vector2 v2 = UnityTools.ParseVector2(axis);
            direction=new Vector3(v2.x,0,v2.y);

            if (playerInfo.anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded")&& IsTurnDone())
            {
                UseSkill(skill);
                if (skill.parameters.TryGet("Trigger") == 0)
                {
                    playerInfo.anim.SetTrigger("UseSkill");
                }
                playerInfo.ToUseSkill = false;
                return;
            }
        }
        else
        {
            UseSkill(skill);
            direction=Vector3.zero;
        }
        currentSkill = skill;
    }
    public PlayerSkill SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
        return this;
    }

    private bool IsTurnDone()
    {
        if (direction == Vector3.zero)
            return true;
        if (Mathf.Abs(transform.rotation.eulerAngles.y - Quaternion.LookRotation(direction, transform.up).eulerAngles.y) <= 20)
        {
            return true;
        }
        return false;
    }
    private void TurnToUseSkill()
    {
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(direction, transform.up), Time.fixedDeltaTime * RotateSpeed));
    }
}
