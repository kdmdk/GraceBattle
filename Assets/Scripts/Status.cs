using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum actionType
{
    guard = 0,
    punch = 1,
    kick = 2,
    protect = 3,
    heal = 4,
}


[CreateAssetMenu]
public class Status : ScriptableObject
{
    public string characterName;
    public int cost;
    public int maxHp;
    public int defaultAtk;
    public int defaultSpd;
    public int hp;
    public int atk;
    public int spd;
    public bool isFriend = true;
    public bool isCanAction = true;
    public bool isAlive = true;
    public actionType action = actionType.guard;
    public Sprite sprite;
    public Sprite friendSprite;
    public Sprite enemySprite;
}
