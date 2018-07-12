using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private Vector3 shootDir;
    private PlayerManager playerManager;
    private GameObject target;
    private float CD = 2;
    private float timer = 0;
    private bool isReady = true;
    
    // Use this for initialization
    void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            timer += Time.deltaTime;
            if (timer >= CD)
            {
                isReady = true;
                timer = 0;
            }

        }
        if (playerInfo.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            playerInfo.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && target != null && !playerInfo.IsLock&& isReady)
        {
            playerInfo.IsAttack = false;
            isReady = false;
            Debug.Log("Attacking正在攻击");
            if (LayerMask.LayerToName(target.layer) == "Cube")
            {
                if (target.GetComponent<MapInfo>().CurrentHp <= 0)
                {
                    return;
                }
                playerInfo.IsAttack = true;
                target.GetComponent<MapInfo>().Damage(playerInfo.AttackDamage);
            }
            else if (target.tag == "Vulnerable")
            {
                Destroy(target);
            }
            else
            {
                if (target.GetComponent<PlayerInfo>().IsDead)
                {
                    return;
                }
                playerInfo.IsAttack = true;
                target.GetComponent<PlayerInfo>().Damage(playerInfo.AttackDamage);
            }
        }
        if (target == null)
        {
            playerInfo.IsAttack = false;
        }
    }
    public void SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
    }

    public void Attack(GameObject target)
    {
        if (playerInfo.IsAttack) return;
        this.target = target;
        playerInfo.IsAttack = true;
        playerInfo.anim.SetBool("Attack", true);
    }
}
