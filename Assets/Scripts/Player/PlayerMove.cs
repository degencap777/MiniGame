
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    public float speed = 2.0f;
    public float rotateSpeed = 3f;
    private ETCJoystick JoyStick;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float smoothingTime = 0.1f;
    public float margin = 2f;
    private PlayerManager playerManager;
    private Rigidbody rb;
    private bool isMove = false;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        JoyStick= ((GamePanel)GameFacade.Instance.GetCurrentPanel()).Joystick;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded") == false) return;
        if (IsGrounded())
        {
            float h = JoyStick.axisY.axisValue;
            float v = JoyStick.axisX.axisValue;
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                moveDirection = new Vector3(v, 0, h);
                Move();
            }
        }
    }
    
    private void Move()
    {
        transform.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(moveDirection, transform.up), Time.fixedDeltaTime * rotateSpeed);
        transform.Translate(moveDirection*Time.fixedDeltaTime * speed,Space.World);
    }

    public void MoveAsync(Vector3 moveDir)
    {
        moveDirection = moveDir;
        isMove = true;
    }
    // 通过射线检测主角是否落在地面或者物体上  
    bool IsGrounded()
    {
        //这里transform.position 一般在物体的中间位置，注意根据需要修改margin的值
        return Physics.Raycast(transform.position, -Vector3.up, margin);
    }
    public PlayerMove SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
        return this;
    }
}

