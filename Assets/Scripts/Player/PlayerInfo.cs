using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

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

    private HealthBar healthBar;
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

    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            if (value)
            {
                IsMove = false;
                ToUseSkill = false;
                ToUseItem = false;
                IsAttack = false;
            }
            _isDead = value;
        }
    }
    private bool _isMove = false;

    public bool IsMove
    {
        get { return _isMove; }
        set {
            if (value)
            {
                ToUseSkill = false;
                ToUseItem = false;
                IsAttack = false;
            }
            _isMove = value;
        }
    }
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
        healthBar = GetComponent<HealthBar>();
        healthBar.healthLink.targetScript = this;
        healthBar.healthLink.fieldName = "CurrentHp";
    }
    void Update()
    {
        Timer += Time.deltaTime;
        
        if(VisualProvider!=null)
            VisualProvider.noOcclusion = IsSkyVision;
        if (!IsDead)
        {
            Debug.Log(IsDead);
            if (CurrentHp <= 0)
            {
                Debug.Log("这不是已经死了吗");
                IsDead = true;
                if (gameObject == Player.Dead)
                {
                    Player.RoleInstanceIdList.Remove(InstanceId);
                    Destroy(gameObject);
                    PlayerManager.Revive(Player.RoleInstanceIdList[0]);
                }
                else
                {
                    Die();
                }
            }
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
        if (IsDead) return;
        CurrentHp = CurrentHp - damage < 0 ? 0 : CurrentHp - damage;
        DebugConsole.Log("Hp=" + CurrentHp);
    }

    public void Die()
    {
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
        Debug.Log("die");
    }

    public void Revive()
    {
        anim.SetTrigger("Revive");
        IsDead = false;
        CurrentHp = Hp;
        CurrentMp = Mp;
    }

    void OnEnable()
    {
        if (healthBar != null && healthBar.HealthbarPrefab != null)
            healthBar.HealthbarPrefab.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (healthBar != null&& healthBar.HealthbarPrefab!=null)
            healthBar.HealthbarPrefab.gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        if (healthBar != null && healthBar.HealthbarPrefab != null)
        {
            healthBar.HealthbarPrefab.parent.GetComponent<HealthbarRoot>().healthBars.Remove(healthBar.HealthbarPrefab);
            Destroy(healthBar.HealthbarPrefab.gameObject);
        }
    }
}
