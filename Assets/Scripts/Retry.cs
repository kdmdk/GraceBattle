using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public void OnClickRetryButton()
    {
        //SelectManager.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameObject battleManager = GameObject.Find("BattleManager");
        battleManager.GetComponent<BattleManager>().Reset();
    }
}
