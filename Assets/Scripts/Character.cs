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
    
    public void Action()
    {
        int deff = 1;
        if(target.battlerStatus.action == actionType.guard)
        {
            deff = 2;
        }
        else if (target.battlerStatus.action == actionType.protect)
        {
            deff = 4;
        }

        target.battlerStatus.hp -= (user.battlerStatus.atk / 2) / deff;
        //Debug.Log(target.battlerStatus.name + ":" + target.battlerStatus.hp);
        battlerStatus.isCanAction = false;
        Debug.Log(user.battlerStatus.name + ":" + target.battlerStatus.name);
    }
    public void DefAct()
    {
        //Debug.Log("Def");
        battlerStatus.isCanAction = false;
    }
    public void kick()
    {
        int deff = 1;
        if (target.battlerStatus.action == actionType.guard)
        {
            deff = 2;
        }
        else if (target.battlerStatus.action == actionType.protect)
        {
            deff = 4;
        }
        target.battlerStatus.hp -= (user.battlerStatus.atk / 4) / deff;
        //Debug.Log(target.battlerStatus.name + ":" + target.battlerStatus.hp);

        //target.battlerPanel.transform.SetSiblingIndex(BattleManager.CharactersList.Count - 1);
        //BattleManager.CharactersList.Insert(BattleManager.CharactersList.Count - 1, target);

        battlerStatus.isCanAction = false;
        /*
        Debug.Log(user.battlerStatus.name + ":" + target.battlerStatus.name);
        foreach(Character c in BattleManager.CharactersList)
        {
            Debug.Log(c.battlerStatus.name);
        }
        */

    }
    public void protect()
    {
        foreach (Character character in BattleManager.CharactersList)
        {
            if (user.battlerStatus.isFriend)
            {
                if (!character.battlerStatus.isFriend && character.battlerStatus.action != actionType.heal)
                {
                    character.target = user;
                }
            }
            else
            if (!user.battlerStatus.isFriend)
            {
                if (character.battlerStatus.isFriend && character.battlerStatus.action != actionType.heal)
                {
                    character.target = user;
                }
            }
        }
    }
    public void heal()
    {
        target.battlerStatus.hp += 30;
    }
}
