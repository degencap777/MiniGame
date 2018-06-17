using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private Vector3 shootDir;
    private PlayerManager playerManager;
    private float turnSpeed = 3;
    public bool IsAttacking;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded") && IsAttacking == false)
        {
            anim.speed = 1;
            if (Input.GetMouseButtonDown(0))
            {
                IsAttacking = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPoint = hit.point;
                    targetPoint.y = transform.position.y;
                    shootDir = targetPoint - transform.position;
                    anim.SetTrigger("Attack");
                    anim.speed = 2;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(shootDir),
                        turnSpeed * Time.deltaTime);
                    Invoke("Shoot", 0.5f);
                }
            }
        }

    }
    public void SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
    }
}
