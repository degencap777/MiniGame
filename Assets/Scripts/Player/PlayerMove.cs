
using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    private float Speed {get { return playerInfo.MoveSpeed; }}
    private float RotateSpeed { get { return playerInfo.TurnSpeed; } }
    private bool IsGrounded { get { return playerInfo.anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"); } }
    private ETCJoystick JoyStick;
    private Vector3 moveDirection = Vector3.zero;
    private float smoothingTime = 0.1f;
    public float margin = 2f;
    private PlayerManager playerManager;
    private Rigidbody rb;
    private PlayerInfo playerInfo;
    private Animator Anim { get { return playerInfo.anim; } }
    private PlayerSkill playerSkill;
    private bool isDeadLimit = false;
    //Dead
    Vector3 deadPosition=Vector3.zero;
    private float radius = 0;

    // Use this for initialization
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        JoyStick= ((GamePanel)GameFacade.Instance.GetCurrentPanel()).Joystick;
        playerSkill = GetComponent<PlayerSkill>();
        playerInfo = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrounded&&playerInfo.Player.CurrentRoleInstanceId==playerInfo.InstanceId)
        {
            float h = JoyStick.axisY.axisValue;
            float v = JoyStick.axisX.axisValue;
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                playerInfo.ToUseSkill = false;
                moveDirection = new Vector3(v, 0, h);
                Anim.SetFloat("Forward", Vector3.Distance(Vector3.zero, moveDirection)); 
                Move();
            }
            else
            {
                Anim.SetFloat("Forward", 0);
            }
        }
        //TODO 动态寻路

        if (isDeadLimit)
        {
            if (Vector3.Distance(transform.position, deadPosition) >= radius)
            {
                transform.position = deadPosition;
            }
        }
    }
    
    private void Move()
    {
        //transform.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(moveDirection, transform.up), Time.fixedDeltaTime * RotateSpeed);
        //transform.Translate(moveDirection*Time.fixedDeltaTime * Speed,Space.World);
        rb.MovePosition(rb.position+ moveDirection * Time.fixedDeltaTime * Speed);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(moveDirection, transform.up), Time.fixedDeltaTime * RotateSpeed));
    }
    
    //// 通过射线检测主角是否落在地面或者物体上  
    //bool IsGrounded()
    //{
    //    //这里transform.position 一般在物体的中间位置，注意根据需要修改margin的值
    //    return Physics.Raycast(transform.position, -Vector3.up, margin);
    //}
    public PlayerMove SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
        return this;
    }

    public PlayerMove AddLimit(Vector3 deadPosition,float radius)
    {
        this.deadPosition = deadPosition;
        this.radius = radius;
        isDeadLimit = true;
        return this;
    }
}

