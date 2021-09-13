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
    public static GameObject resultPanel;
    public GameObject readyButton;
    float waitTime = 5;
    void Awake()
    {
        resultPanel = GameObject.Find("ResultPanel");
    }

    void Update()
    {
        if (!SelectManager.isStart || onLoaded)
        {
            SortPanel();
            return;
        }
        LoadCharacterData();
    }
    //リセット処理
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
    //キャラクターパネルの生成処理
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

        SortPanel();
        
        onLoaded = true;
    }

    //キャラクターパネルの並び替え
    void SortPanel()
    {
        //battlePanelにインデックスをつけてヒエラルキー上のオブジェクトを並び替える
        int panelCount = 0;
        foreach (Character character in CharactersList)
        {
            character.battlerPanel.transform.SetSiblingIndex(panelCount);
            if (!onLoaded)
            {
                character.target = character;
            }
            panelCount++;
        }
    }
    //readyボタンをクリックしたとき
    public void OnClickReady()
    {
        //条件に従って行動をとる
        StartCoroutine("Action");
    }
    IEnumerator Action()
    {
        readyButton.SetActive(false);
        for (int i = 0; i < CharactersList.Count; i++)
        {
            Character character = CharactersList[i];
            //生きている場合、行動をする
            if (character.battlerStatus.isAlive)
            {
                switch (character.battlerStatus.action)
                {
                    case actionType.guard:
                        character.Guard();
                        break;
                    case actionType.punch:
                        character.Punch();
                        break;
                    case actionType.kick:
                        character.Kick();
                        break;
                    case actionType.protect:
                        character.Protect();
                        break;
                    case actionType.heal:
                        character.Heal();
                        break;
                    default:
                        break;
                }
                DeadOrArive();
                yield return new WaitForSeconds(waitTime);
                Debug.Log("end " + i);
            }
        }
        OtherProcess();
        readyButton.SetActive(true);
    }

    void DeadOrArive()
    {
        //hp0の時にパネルを隠す
        //foreach (Character character in CharactersList)
        for (int i = 0; i < CharactersList.Count; i++)
        {
            Character character = CharactersList[i];
            if (character.battlerStatus.hp <= 0)
            {
                character.battlerStatus.isAlive = false;
                character.battlerPanel.SetActive(false);
                character.battlerImg.GetComponent<SpriteRenderer>().enabled = false;
                Debug.Log(character.battlerStatus.characterName + "は倒れた！");
                //CharactersList.Remove(character);
            }
        }
    }

    void OtherProcess()
    {
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

        //リザルト画面
        Text resultText = resultPanel.GetComponentInChildren<Text>();

        if (fCnt == 0 && eCnt == 0)
        {
            resultPanel.SetActive(true);
            resultText.text = "DRAW";
        }
        else if (fCnt == 0)
        {
            resultPanel.SetActive(true);
            resultText.text = "LOSE";
        }
        else if (eCnt == 0)
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
