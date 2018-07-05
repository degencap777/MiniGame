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
    // Use this for initialization
    void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInfo.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            playerInfo.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f && target != null)
        {
            if (LayerMask.LayerToName(target.layer) == "Cube")
            {
                if (target.GetComponent<MapInfo>().CurrentHp <= 0)
                {
                    playerInfo.IsAttack = false;
                    return;
                }
                target.GetComponent<MapInfo>().Damage(playerInfo.AttackDamage);
            }
            else
            {
                if (target.GetComponent<PlayerInfo>().IsDead)
                {
                    playerInfo.IsAttack = false;
                    return;
                }
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
