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
    public VisualTest VisualTest { get; private set; }
    public int InstanceId;
    public PlayerManager PlayerManager { get; private set; }
    public RoleData RoleData { get; private set; }
    public Animator anim { get; private set; }

    private Rigidbody rb;
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
            if(value)
                anim.SetTrigger("Attack");
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

    public bool IsTransparent = false;
    public bool IsTrueVision = false;
    public bool IsInTrueVision = false;
    public bool IsLock = false;
    private float Timer = 0;
    private bool moveSoundOn = false;

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
        if (Name == "Bird0")
        {
            if (VisualProvider != null)
            {
                VisualProvider.visualRange = 8;
            }
        }
    }

    void Start()
    {
        VisualProvider = GetComponent<VisualProvider>();
        VisualTest = GetComponent<VisualTest>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        healthBar = GetComponent<HealthBar>();
        healthBar.healthLink.targetScript = this;
        healthBar.healthLink.fieldName = "CurrentHp";
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if (Name == "Bird0")
        {
            if (GameFacade.Instance.AltarCount == 3)
            {
                IsTrueVision = true;
            }
            if (PlayerManager.GetCurrentOpTarget() == this.gameObject)
            {
                if (!moveSoundOn)
                {
                    moveSoundOn = true;
                    GameFacade.Instance.PlaySound("BirdFly", true);
                }
            }
            else
            {
                GameFacade.Instance.StopSound("BirdFly");
            }
        }
        if (VisualProvider != null)
        {
            VisualProvider.IsTrueVision = IsTrueVision;
            VisualProvider.noOcclusion = IsSkyVision;
        }
        if (VisualTest != null)
        {
            VisualTest.IsTransparent = IsTransparent;
            IsInTrueVision = VisualTest.isInTrueVision;
        }
        if (!IsDead)
        {
            if (CurrentHp <= 0)
            {
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
        GameFacade.Instance.PlaySound("Die");
        if(anim!=null)
            anim.SetTrigger("Die");
        if (RoleType == RoleType.Hero)
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
        ShowHealthBar();
    }

    void OnDisable()
    {
        HideHealthBar();
    }
    void OnDestroy()
    {
        if (healthBar != null && healthBar.HealthbarPrefab != null)
        {
            healthBar.HealthbarPrefab.parent.GetComponent<HealthbarRoot>().healthBars.Remove(healthBar.HealthbarPrefab);
            Destroy(healthBar.HealthbarPrefab.gameObject);
        }
    }

    public void HideHealthBar()
    {
        if (healthBar != null && healthBar.HealthbarPrefab != null)
            healthBar.HealthbarPrefab.gameObject.SetActive(false);
    }

    public void ShowHealthBar()
    {
        if (healthBar != null && healthBar.HealthbarPrefab != null)
            healthBar.HealthbarPrefab.gameObject.SetActive(true);
    }


    void OnCollisionStay(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.layer == 18||IsMove|| go.layer == 10) return;
        Vector3 dir = transform.position - go.transform.position;
        dir.y = 0;
        transform.Translate(Time.deltaTime*dir.normalized*10,Space.World);
    }
}
