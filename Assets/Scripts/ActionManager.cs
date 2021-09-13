using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionManager : MonoBehaviour
{
    Character target;
    int actionNum = 0;
    public static int index;

    public Slider gauge;

    string[] actionWords =
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

    public enum TargetType
    {
        self,
        enemy,
        friend,
    }
    public void OnClickBattlerPanel()
    {
        index = this.transform.GetSiblingIndex();
        
        if (BattleManager.CharactersList[index].battlerStatus.isCanAction)// && BattleManager.CharactersList[index].battlerStatus.isFriend)
        {
            //クリックするごとに行動予定が変化
            actionNum++;
            if(actionNum >= actionWords.Length)
            {
                actionNum = 0;
            }
            //actionNum番目のactionTypeにする
            BattleManager.CharactersList[index].battlerStatus.action = (actionType)Enum.ToObject(typeof(actionType), actionNum);

            //キャラクターパネルのテキストを変更
            String actionName = "ぼうぎょ";

            Text textObj = this.transform.Find("Text").GetComponent<Text>();

            //Targeting(TargetType.enemy);

            //userに自分自身を設定
            BattleManager.CharactersList[index].user = BattleManager.CharactersList[index];

            //各行動の準備　テキストの変更とactionTypeの変更
            switch (actionNum)
            {
                case 0:
                    actionName = "ぼうぎょ";
                    textObj.text = actionName;
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.guard;
                    //Targeting(TargetType.self);
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
                    BattleManager.CharactersList[index].battlerStatus.action = actionType.heal;
                    //Targeting(TargetType.friend);
                    return;
                default:
                    return;
            }
        }
    }
    //hpのゲージを変動させる
    public void GaugeMove()
    {
        index = this.transform.GetSiblingIndex();
        gauge.maxValue = BattleManager.CharactersList[index].battlerStatus.maxHp;
        gauge.value = BattleManager.CharactersList[index].battlerStatus.hp;
    }
    //テキスト情報更新
    public void TextInfo()
    {
        Text textObj = this.transform.Find("Text").GetComponent<Text>();

        String actionName;
        actionNum = (int)BattleManager.CharactersList[index].battlerStatus.action;
        actionName = actionWords[actionNum];
        textObj.text = actionName;
    }
    /*
    public static void Targeting(TargetType targetType)
    {
        //Debug.Log("targeting now");
        //targetを設定
        Character result = BattleManager.CharactersList[index];
        Debug.Log(result.battlerStatus.characterName);
        
        switch (targetType)
        {
            case TargetType.self:
                result = BattleManager.CharactersList[index];
                break;
            case TargetType.enemy:
                result = BattleManager.CharactersList.Find(n => !n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                break;
            case TargetType.friend:
                result = BattleManager.CharactersList.Find(n => n.battlerStatus.isFriend && n.battlerStatus.isAlive);
                break;
            default:
                break;
        }

        BattleManager.CharactersList[index].target = result;
    }
    */
}
