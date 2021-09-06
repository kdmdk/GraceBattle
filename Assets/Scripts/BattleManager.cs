using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Status[] statuses;
    public GameObject battlerPanel;
    public GameObject timeLine;
    bool onLoaded;
    public SpriteRenderer[] battlerImage;
    GameObject[] Panels = new GameObject[SelectManager.maxNum * 2];

    [SerializeField]
    public static List<Character> CharactersList = new List<Character>();
    //internal static object instanse;
    public static GameObject resultPanel;

    void Awake()
    {
        resultPanel = GameObject.Find("ResultPanel");
    }

    void Update()
    {
        if (!SelectManager.isStart || onLoaded)
        {
            return;
        }
        LoadCharacterData();
    }
    public void Reset()
    {
        //選択フラグを全てオフに
        for (int i = 0; i < SelectManager.onSelect.Length; i++)
        {
            SelectManager.onSelect[i] = false;
        }
        SelectManager.isStart = false;

        //スクリプタブルオブジェクトをリセット　こういう使い方はよくないかも。。
        for(int i =0; i < statuses.Length;i++)
        {
            statuses[i].isFriend = true;
            statuses[i].isCanAction = true;
            statuses[i].isAlive = true;
            statuses[i].action = actionType.guard;
            statuses[i].hp = statuses[i].maxHp;
            statuses[i].atk = statuses[i].defaultAtk;
            statuses[i].spd = statuses[i].defaultSpd;
        }

        SelectManager.isStart = false;
        BattleManager.CharactersList.Clear();
        resultPanel.SetActive(false);
    }
    void LoadCharacterData()
    {
        //味方のパネル生成処理
        int cntFriend = 0;
        int[] tmpFriend = new int[SelectManager.maxNum];
        
        for (int i = 0; i < SelectManager.onSelect.Length; i++)
        {
            if (SelectManager.onSelect[i])
            {
                Panels[cntFriend] = Instantiate(battlerPanel, Vector3.zero, Quaternion.identity, timeLine.transform);
                GameObject characterImage = Panels[cntFriend].transform.Find("CharacterImage").gameObject;
                characterImage.GetComponent<Image>().sprite = statuses[i].sprite;

                Text textObj = Panels[cntFriend].transform.Find("Text").GetComponent<Text>();
                textObj.text = statuses[i].spd.ToString();

                //リストにパネルとステータスを追加する処理
                CharactersList.Add(new Character() { battlerPanel = Panels[cntFriend], battlerStatus = statuses[i] });

                CharactersList[cntFriend].battlerStatus.isFriend = true;

                tmpFriend[cntFriend] = i;
                cntFriend++;
            }
        }
        //敵のパネル生成処理
        int cntEnemy = 0;
        int[] tmpEnemy = new int[SelectManager.maxNum];
        
        for (int i = 0;i < SelectManager.onSelect.Length; i++)
        {
            if (!SelectManager.onSelect[i] && cntEnemy < SelectManager.maxNum)
            {
                Panels[cntEnemy + cntFriend] = Instantiate(battlerPanel, Vector3.zero, Quaternion.identity, timeLine.transform);
                Panels[cntEnemy + cntFriend].gameObject.GetComponent<Image>().color = new Color(0.8f, 0, 0, 1.0f);
                GameObject characterImage = Panels[cntEnemy + cntFriend].transform.Find("CharacterImage").gameObject;
                characterImage.GetComponent<Image>().sprite = statuses[i].sprite;

                Text textObj = Panels[cntEnemy + cntFriend].transform.Find("Text").GetComponent<Text>();
                textObj.text = statuses[i].spd.ToString();

                //リストにパネルとステータスを追加する処理
                CharactersList.Add(new Character() { battlerPanel = Panels[cntEnemy + cntFriend], battlerStatus = statuses[i] });

                CharactersList[cntEnemy + cntFriend].battlerStatus.isFriend = false;

                tmpEnemy[cntEnemy] = i;
                cntEnemy++;
            }
        }

        //フィールド上のキャラクターを表示
        for (int i = 0; i < SelectManager.maxNum; i++)
        {
            battlerImage[i].sprite = statuses[tmpFriend[i]].friendSprite;
            CharactersList[i].battlerImg = battlerImage[i];
        }
        for (int i = 0; i < SelectManager.maxNum; i++)
        {
            battlerImage[i + (battlerImage.Length / 2)].sprite = statuses[tmpEnemy[i]].enemySprite;
            CharactersList[i+SelectManager.maxNum].battlerImg = battlerImage[i + (battlerImage.Length / 2)];
        }

        //Characterクラスをリスト化したCharactersListを素早さで昇順ソート

        CharactersList.Sort((a, b) => b.battlerStatus.spd - a.battlerStatus.spd);

        //battlePanelにインデックスをつけてヒエラルキー上のオブジェクトを並び替える
        int panelCount = 0;
        foreach(Character character in CharactersList)
        {
            character.battlerPanel.transform.SetSiblingIndex(panelCount);
            character.target = character;

            panelCount++;
        }

        onLoaded = true;
    }


    public void OnClickReady()
    {
        //条件に従って行動をとる
        foreach (Character character in CharactersList)
        {
            if (character.battlerStatus.action == actionType.guard && character.battlerStatus.isCanAction)
            {
                character.DefAct();
            }
            if (character.battlerStatus.action == actionType.punch && character.battlerStatus.isCanAction)
            {
                character.Action();
            }
            if (character.battlerStatus.action == actionType.kick && character.battlerStatus.isCanAction)
            {
                character.kick();
            }
            if (character.battlerStatus.action == actionType.protect && character.battlerStatus.isCanAction)
            {
                character.protect();
            }
            if (character.battlerStatus.action == actionType.heal && character.battlerStatus.isCanAction)
            {
                character.heal();
            }
        }
        //hp0の時にパネルを隠す
        foreach (Character character in CharactersList)
        {
            if (character.battlerStatus.hp <= 0)
            {
                character.battlerStatus.isAlive = false;
                character.battlerPanel.SetActive(false);
                character.battlerImg.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        //各陣営何人残っているかで判定
        int fCnt = 0;
        int eCnt = 0;
        foreach (Character character in CharactersList)
        {
            if (character.battlerStatus.isFriend && character.battlerStatus.isAlive)
            {
                fCnt++;
            }
            else if (!character.battlerStatus.isFriend && character.battlerStatus.isAlive)
            {
                eCnt++;
            }
        }
        //Debug.Log(fCnt + ":" + eCnt);

        Text resultText = resultPanel.GetComponentInChildren<Text>();

        if (fCnt == 0 && eCnt == 0)
        {
            resultPanel.SetActive(true);
            resultText.text = "DRAW";
        }
        else if(fCnt == 0)
        {
            resultPanel.SetActive(true);
            resultText.text = "LOSE";
        }
        else if(eCnt == 0)
        {
            resultPanel.SetActive(true);
            resultText.text = "WIN";
        }
        //ターン開始時の処理
        foreach (Character character in CharactersList)
        {
            character.battlerStatus.action = actionType.guard;
            character.battlerStatus.isCanAction = true;
        }
    }
}
