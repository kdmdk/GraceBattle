using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public GameObject battlerPanel { get; set; }
    public Status battlerStatus { get; set; }
    public Character target;
    public Character user;
    public SpriteRenderer battlerImg;

    Dictionary<actionType, int> defensePoint = new Dictionary<actionType, int>()
    {
        {actionType.guard, 2},
        {actionType.protect, 4},
    };

    public enum TargetType
    {
        self,
        enemy,
        friend,
    }

    //パンチのとき（基本攻撃）
    public void Punch()
    {
        Targeting(TargetType.enemy);
        CommonAction();
        Attack(2);
    }
    //防御行動
    public void Guard()
    {
        Targeting(TargetType.self);
        CommonAction();
    }
    //キックのとき（特殊攻撃）
    public void Kick()
    {
        Targeting(TargetType.enemy);
        CommonAction();
        Attack(1);
        BattleManager.CharactersList.Remove(target);
        BattleManager.CharactersList.Insert(BattleManager.CharactersList.Count, target);
    }
    //かばうのとき（敵のターゲットを自分にする）
    public void Protect()
    {
        Targeting(TargetType.self);
        CommonAction();
    }
    //回復行動
    public void Heal()
    {
        Targeting(TargetType.friend);
        CommonAction();
        target.battlerStatus.hp += 30;
    }
    //汎用攻撃(攻撃の強さ、数が少ないほどつよい)
    void Attack(int powerRank)
    {
        //エラー処理
        if(powerRank == 0)
        {
            Debug.Log("Error!:"+ powerRank);
            powerRank = -1;
        }
        //敵がぼうぎょ、かばうを使っているときダメージを軽減
        int deff = 1;
        if (target.battlerStatus.action == actionType.guard)
        {
            deff = defensePoint[actionType.guard];
        }
        else if (target.battlerStatus.action == actionType.protect)
        {
            deff = defensePoint[actionType.protect];
        }
        //敵がかばうを使っているとき
        foreach (Character chara in BattleManager.CharactersList)
        {
            if(chara.battlerStatus.action == actionType.protect)
            {
                user.target = chara;
            }
        }
        target.battlerStatus.hp -= (user.battlerStatus.atk / powerRank) / deff;
    }
    //共通行動
    void CommonAction()
    {
        //死んでる場合ターゲットを切り替え
        Debug.Log($"{target.battlerStatus.characterName}は生きているか？{target.battlerStatus.isAlive}");
        Debug.Log($"{user.battlerStatus.characterName}は{target.battlerStatus.characterName}に{user.battlerStatus.action}をした");
        //行動不能になる
        battlerStatus.isCanAction = false;
    }

    void Targeting(TargetType type)
    {
        switch (type)
        {
            case TargetType.self:
                {
                    target = user;
                }
                break;
            case TargetType.enemy:
                { 
                    if (user.battlerStatus.isFriend)
                    {
                        target = BattleManager.CharactersList.Find(n => !n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    }
                    else
                    {
                        target = BattleManager.CharactersList.Find(n => n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    }
                }
                break;
            case TargetType.friend:
                {
                    if (user.battlerStatus.isFriend)
                    {
                        target = BattleManager.CharactersList.Find(n => n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    }
                    else
                    {
                        target = BattleManager.CharactersList.Find(n => !n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    }
                }
                break;
            default:
                break;

        }
        
    }
}
