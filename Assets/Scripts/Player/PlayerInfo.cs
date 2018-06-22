﻿using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public CampType CampType;
    [Range(0, 11)]
    public int RoleIndex = -1;

    public float MoveSpeed = 2;
    public float TurnSpeed = 3;
    public State CurrentState = State.Idle;
    public enum State
    {
        Idle,
        Move,
        UsingSkill,
        UsingItem
    }
}