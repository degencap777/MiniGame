using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
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
    public Animator anim { get; private set; }

    private bool _toUseSkill = false;

    public bool ToUseSkill
    {
        get
        {
            return _toUseSkill;
        }
        set
        {
            if (value)
            {
                ToUseItem = false;
                IsAttack = false;
            }
            _toUseSkill = value;
        }
    }

    private bool _toUseItem = false;
    public bool ToUseItem
    {
        get
        {
            return _toUseItem;
        }
        set
        {
            if (value)
            {
                ToUseSkill = false;
                IsAttack = false;
            }
            _toUseItem = value;
        } 
    }

    private bool _isAttack = false;

    public bool IsAttack
    {
        get
        {
            return _isAttack;
        }
        set
        {
            if (value == false)
            {
                anim.SetBool("Attack", false);
            }
            _isAttack = value;
        }
    }

    public bool IsDead;
    private float Timer = 0;

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
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if(VisualProvider!=null)
            VisualProvider.noOcclusion = IsSkyVision;
        if (CurrentHp <= 0&&!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            if (gameObject==Player.Dead)
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            if (RoleType != RoleType.Hero)
            {
                Destroy(gameObject);
            }
            else if (CampType == CampType.Monkey)
            {
                PlayerManager.Revive(InstanceId);
            }
            else
            {
                gameObject.SetActive(false);
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

    public void Die()
    {
        IsDead = true;
        if(anim!=null)
            anim.SetTrigger("Die");
        if (RoleType == RoleType.Hero && CampType == CampType.Fish)
        {
            PlayerManager.Die(InstanceId);
        }
        else
        {
            Player.RoleInstanceIdList.Remove(InstanceId);
        }
        DebugConsole.Log("die");
    }

    private void Revive()
    {
        Player.RoleInstanceIdList.Remove(InstanceId);
        Destroy(gameObject);
        PlayerManager.Revive(Player.RoleInstanceIdList[0]);
    }
    
}
