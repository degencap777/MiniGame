using UnityEngine;
using System.Collections;

public class CharacterAnimationDungeon : MonoBehaviour {

	private CharacterController cc;
	private Animation anim;
    
    public float margin = 1.05f;

    // Use this for initialization
    void Start () {
		
		cc= GetComponentInChildren<CharacterController>();
		anim = GetComponentInChildren<Animation>();
	}
    // 通过射线检测主角是否落在地面或者物体上  
    bool IsGrounded()
    {
        //这里transform.position 一般在物体的中间位置，注意根据需要修改margin的值
        return Physics.Raycast(transform.position, -Vector3.up, margin);
    }

    // Wait end of frame to manage charactercontroller, because gravity is managed by virtual controller
    void FixedUpdate(){
		if (IsGrounded() && (ETCInput.GetAxis("Vertical")!=0 || ETCInput.GetAxis("Horizontal")!=0)){
			anim.CrossFade("soldierRun");
            Debug.Log(1);
		}
		
		if (IsGrounded() && ETCInput.GetAxis("Vertical")==0 && ETCInput.GetAxis("Horizontal")==0){
			anim.CrossFade("soldierIdleRelaxed");
		    Debug.Log(2);
        }
		
		if (!IsGrounded())
        {
		    Debug.Log(3);
            anim.CrossFade("soldierFalling");
		}

	}
}
