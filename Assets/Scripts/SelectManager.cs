using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public static int characterNum = 9;
    public Text[] texts;
    public BattleManager battleManager;
    int count = 0;
    public GameObject startButton;
    public GameObject[] selectPanel;
    public Text infoText;
    public static bool isStart;
    public GameObject selectScreen;
    //public int MAX = 3;
    public static int maxNum = 3;
    public static bool[] onSelect = new bool[characterNum];

    List<int> selectedCharacters = new List<int>();

    //public static bool isReset = false;

    void Start()
    {
        GameObject battleManager = GameObject.Find("BattleManager");
        battleManager.GetComponent<BattleManager>().Reset();
        //maxNum = MAX;
        //Reset();
        Info();
    }

    void Update()
    {
        //maxNum = MAX;

        Info();

        if (count == maxNum)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }
    /*
    public static void Reset()
    {
        for(int i = 0;i < onSelect.Length; i++)
        {
            onSelect[i] = false;
        }
        isStart = false;

        foreach(Character c in BattleManager.CharactersList)
        {
            c.target = c;
            c.battlerStatus.isFriend = true;
            c.battlerStatus.isCanAction = true;
            c.battlerStatus.action = actionType.guard;
            c.battlerStatus.hp = c.battlerStatus.maxHp;
            c.battlerStatus.atk = c.battlerStatus.defaultAtk;
            c.battlerStatus.spd = c.battlerStatus.defaultSpd;
        }
        isStart = false;
        BattleManager.CharactersList.Clear();
        BattleManager.resultPanel.SetActive(false);
    }
    */
    public void OnClickCharacter(int num)
    {
        if (!onSelect[num])
        {
            selectPanel[num].gameObject.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.8f, 1.0f);
            count++;
            onSelect[num] = true;
            selectedCharacters.Add(num);
        }
        else
        {
            selectPanel[num].gameObject.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            count--;
            onSelect[num] = false;
            selectedCharacters.Remove(num);
        }
        /*
        foreach (int x in selectedCharacters)
        {
           Debug.Log(x);
        }
        */

        if (count > maxNum)
        {
            selectPanel[selectedCharacters.First()].gameObject.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            onSelect[selectedCharacters.First()] = false;
            count--;
            selectedCharacters.RemoveAt(0);
        }

        
        texts[0].text = battleManager.statuses[num].characterName;
        texts[1].text = battleManager.statuses[num].hp.ToString();
        texts[2].text = battleManager.statuses[num].atk.ToString();
        texts[3].text = battleManager.statuses[num].spd.ToString();

        Info();
    }
    public void OnStartButton()
    {
        isStart = true;
        selectScreen.SetActive(false);
    }
    void Info()
    {
        if (count < maxNum)
        {
            infoText.text = $"あと{maxNum - count}人選んでください";
        }
        else if (count == maxNum)
        {
            infoText.text = "ゲームを開始できます！";
        }
        else
        {
            infoText.text = $"あと{(count - maxNum)}人減らしてください";
        }
    }
}
