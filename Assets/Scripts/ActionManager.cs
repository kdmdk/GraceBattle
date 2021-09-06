using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionManager : MonoBehaviour
{
    Character target;
    int CNT = 0;
    int index;

    public Slider gauge;

    string[] actionWord =
    {
        "ぼうぎょ",
        "パンチ",
        "キック",
        "かばう",
        "かいふく",
    };

    void Start()
    {
    }
    void Update()
    {
        if (SelectManager.isStart)
        {
            GaugeMove();
            TextInfo();
        }
    }
    public void OnClickBattlerPanel()
    {
        index = this.transform.GetSiblingIndex();
        
        if (BattleManager.CharactersList[index].battlerStatus.isCanAction)// && BattleManager.CharactersList[index].battlerStatus.isFriend)
        {
            CNT += 1;
            if(CNT > 4)
            {
                CNT = 0;
            }
            BattleManager.CharactersList[index].battlerStatus.action = (actionType)Enum.ToObject(typeof(actionType), CNT);
           
            String actionName = "ぼうぎょ";

            Text textObj = this.transform.Find("Text").GetComponent<Text>();

            Character result = BattleManager.CharactersList.Find(n => n.battlerStatus.isFriend && n.battlerStatus.isAlive);
            if (BattleManager.CharactersList[index].battlerStatus.isFriend)
            {
                result = BattleManager.CharactersList.Find(n => !n.battlerStatus.isFriend && n.battlerStatus.isAlive);
            }
            BattleManager.CharactersList[index].target = result;
            BattleManager.CharactersList[index].user = BattleManager.CharactersList[index];
            
            switch (CNT)
            {
                case 0:
                    actionName = "ぼうぎょ";
                    textObj.text = actionName;
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.guard;
                    return;
                case 1:
                    actionName = "パンチ";
                    textObj.text = actionName;
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.punch;
                    return;
                case 2:
                    actionName = "キック";
                    textObj.text = actionName;
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.kick;
                    return;
                case 3:
                    actionName = "かばう";
                    textObj.text = actionName;
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.protect;
                    return;
                case 4:
                    actionName = "かいふく";
                    textObj.text = actionName;

                    result = BattleManager.CharactersList.Find(n => !n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    if (BattleManager.CharactersList[index].battlerStatus.isFriend)
                    {
                        result = BattleManager.CharactersList.Find(n => n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                    }
                    BattleManager.CharactersList[index].target = result;

                    BattleManager.CharactersList[index].battlerStatus.action = actionType.heal;
                    return;
                default:
                    return;
            }
        }
    }
    public void GaugeMove()
    {
        index = this.transform.GetSiblingIndex();
        gauge.maxValue = BattleManager.CharactersList[index].battlerStatus.maxHp;
        gauge.value = BattleManager.CharactersList[index].battlerStatus.hp;
    }
    public void TextInfo()
    {
        Text textObj = this.transform.Find("Text").GetComponent<Text>();

        String actionName;
        CNT = (int)BattleManager.CharactersList[index].battlerStatus.action;
        actionName = actionWord[CNT];
        textObj.text = actionName;
    }
}
