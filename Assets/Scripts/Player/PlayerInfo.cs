using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    
    
    public State CurrentState = State.Idle;
    public Player Player;
    public int RoleId { get; set; }
    public CampType CampType { get; set; }
    public string Name { get; set; }
    public RoleType RoleType { get; set; }
    public string Description { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public float MoveSpeed { get; set; }
    public float TurnSpeed { get; set; }
    public bool IsSkyVision { get; set; }
    public int AttackDamage { get; set; }

    public enum State
    {
        Idle,
        Move,
        UsingSkill,
        UsingItem
    }

    public void Init(int id, CampType campType, string name, RoleType roleType, string description, int hp, int mp, int moveSpeed,
        int turnSpeed, bool isSkyVision, Player player, int attackDamage=0)
    {
        AttackDamage = attackDamage;
        RoleId = id;
        CampType = campType;
        Name = name;
        RoleType = roleType;
        Description = description;
        Hp = hp;
        Mp = mp;
        MoveSpeed = moveSpeed;
        TurnSpeed = turnSpeed;
        IsSkyVision = isSkyVision;
        Player = player;
    }
}
