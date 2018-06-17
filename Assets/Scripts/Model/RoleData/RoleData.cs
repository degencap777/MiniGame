using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RoleData
{

    public int Id { get; set; }
    public CampType CampType { get; set; }
    public string Name { get; set; }
    public RoleType RoleType { get; set; }
    public string Description { get; set; }
    private string path;
    public GameObject RolePrefab { get; private set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int MoveSpeed { get; set; }
    public int TurnSpeed { get; set; }
    public bool IsSkyVision { get; set; }

    public RoleData(int id, CampType campType, string name, RoleType roleType, string description, string path, int hp, int mp, int moveSpeed, int turnSpeed, bool isSkyVision)
    {
        Id = id;
        CampType = campType;
        Name = name;
        RoleType = roleType;
        Description = description;
        this.path = path;
        RolePrefab = Resources.Load<GameObject>(path + Name);
        Hp = hp;
        Mp = mp;
        MoveSpeed = moveSpeed;
        TurnSpeed = turnSpeed;
        IsSkyVision = isSkyVision;
    }

    public override string ToString()
    {
        return string.Format("Path: {0}, Id: {1}, CampType: {2}, Name: {3}, RoleType: {4}, Description: {5}, RolePrefab: {6}, Hp: {7}, Mp: {8}, MoveSpeed: {9}, TurnSpeed: {10}, IsSkyVision: {11}", path, Id, CampType, Name, RoleType, Description, RolePrefab, Hp, Mp, MoveSpeed, TurnSpeed, IsSkyVision);
    }
}
