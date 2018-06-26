using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    
    
    public State CurrentState = State.Idle;
    public Player Player;
    public int RoleId;
    public CampType CampType;
    public string Name;
    public RoleType RoleType;
    public string Description;
    public int Hp;
    public int CurrentHp;
    public int Mp;
    public int CurrentMp;
    public float MoveSpeed;
    public float TurnSpeed;
    public bool IsSkyVision;
    public int AttackDamage;
    public VisualProvider VisualProvider { get; private set; }
    public int InstanceId;
    public PlayerManager PlayerManager { get; private set; }

    private float Timer = 0;
    public enum State
    {
        Idle,
        Move,
        UsingSkill,
        UsingItem
    }

    public void Init(PlayerManager playerManager,int id, CampType campType, string name, RoleType roleType, string description, int hp, int mp, int moveSpeed,
        int turnSpeed, bool isSkyVision, Player player,int instanceId, int attackDamage=0)
    {
        PlayerManager = playerManager;
        AttackDamage = attackDamage;
        RoleId = id;
        CampType = campType;
        Name = name;
        RoleType = roleType;
        Description = description;
        Hp = hp;
        CurrentHp = Hp;
        Mp = mp;
        CurrentMp = Mp;
        MoveSpeed = moveSpeed;
        TurnSpeed = turnSpeed;
        IsSkyVision = isSkyVision;
        Player = player;
        VisualProvider = GetComponent<VisualProvider>();
        InstanceId = instanceId;
    }

    void Update()
    {
        Timer += Time.deltaTime;
        if(VisualProvider!=null)
            VisualProvider.noOcclusion = IsSkyVision;
        if (CurrentHp <= 0)
        {
            if (gameObject == Player.Dead)
            {
                Revive();
            }
            else
            {
                Die();
            }
            CurrentHp = Hp;
            CurrentMp = Mp;
        }
        if (Timer>1)
        {
            Timer = 0;
            CurrentMp = Mathf.Min(CurrentMp + 1,Mp);
            CurrentHp = Mathf.Min(CurrentHp + 1,Hp);
        }
    }

    public void Damage(int damage)
    {
        CurrentHp = CurrentHp - damage < 0 ? 0 : CurrentHp - damage;
        DebugConsole.Log("Hp=" + CurrentHp);
    }

    private void Die()
    {
        PlayerManager.Die(InstanceId);
        DebugConsole.Log("die");
    }

    private void Revive()
    {
        PlayerManager.Revive(Player.RoleInstanceIdList[0]);
        DebugConsole.Log("复活");
    }
}
