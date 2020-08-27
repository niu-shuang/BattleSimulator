using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleResult : MonoBehaviour
{
    public static string battleResult = "Result";

    public Text _resultText;

    private void Start()
    {
        _resultText.text = battleResult;
    }

    public void BackToPrepare()
    {
        SceneManager.LoadScene("BattlePrepare");
    }
}
