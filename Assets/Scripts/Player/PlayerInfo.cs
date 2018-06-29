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
    public RoleData RoleData { get; private set; }

    private float Timer = 0;
    public enum State
    {
        Idle,
        Move,
        UsingSkill,
        UsingItem
    }

    //public void Init(PlayerManager playerManager,int id, CampType campType, string name, RoleType roleType, string description, int hp, int mp, int moveSpeed,
    //    int turnSpeed, bool isSkyVision, Player player,int instanceId, int attackDamage=0)
    //{
    //    PlayerManager = playerManager;
    //    AttackDamage = attackDamage;
    //    RoleId = id;
    //    CampType = campType;
    //    Name = name;
    //    RoleType = roleType;
    //    Description = description;
    //    Hp = hp;
    //    CurrentHp = Hp;
    //    Mp = mp;
    //    CurrentMp = Mp;
    //    MoveSpeed = moveSpeed;
    //    TurnSpeed = turnSpeed;
    //    IsSkyVision = isSkyVision;
    //    Player = player;
    //    VisualProvider = GetComponent<VisualProvider>();
    //    InstanceId = instanceId;
    //}
    public void Init(PlayerManager playerManager, RoleData roleData, Player player, int instanceId)
    {
        PlayerManager = playerManager;
        switch (roleData.RoleType)
        {
            case RoleType.Hero:
            {
                HeroData rd = (HeroData)roleData;
                AttackDamage = rd.attackDamage;
                }
                break;
            case RoleType.Pet:
            {
                PetData rd = (PetData)roleData;
            }
                break;
        }
        RoleId = roleData.Id;
        CampType = roleData.CampType;
        Name = roleData.Name;
        RoleType = roleData.RoleType;
        Description = roleData.Description;
        Hp = roleData.Hp;
        CurrentHp = Hp;
        Mp = roleData.Mp;
        CurrentMp = Mp;
        MoveSpeed = roleData.MoveSpeed;
        TurnSpeed = roleData.TurnSpeed;
        IsSkyVision = roleData.IsSkyVision;
        RoleData = roleData;

        Player = player;
        InstanceId = instanceId;
    }

    void Start()
    {
        VisualProvider = GetComponent<VisualProvider>();
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if(VisualProvider!=null)
            VisualProvider.noOcclusion = IsSkyVision;
        if (CurrentHp <= 0)
        {
            CurrentHp = Hp;
            CurrentMp = Mp;
            if (gameObject==Player.Dead)
            {
                Revive();
            }
            else
            {
                Die();
            }
        }
        //if (Timer>1)
        //{
        //    Timer = 0;
        //    CurrentMp = Mathf.Min(CurrentMp + 1,Mp);
        //    CurrentHp = Mathf.Min(CurrentHp + 1,Hp);
        //}
    }

    public void Damage(int damage)
    {
        CurrentHp = CurrentHp - damage < 0 ? 0 : CurrentHp - damage;
        DebugConsole.Log("Hp=" + CurrentHp);
    }

    private void Die()
    {
        if(RoleType==RoleType.Hero)
            PlayerManager.Die(InstanceId);
        else
        {
            Player.RoleInstanceIdList.Remove(InstanceId);
            Destroy(gameObject);
        }
        DebugConsole.Log("die");
    }

    private void Revive()
    {
        Player.RoleInstanceIdList.Remove(InstanceId);
        Destroy(gameObject);
        PlayerManager.Revive(Player.RoleInstanceIdList[0]);
    }

    public void TimeToDie()
    {
        Destroy(gameObject);
    }
}
