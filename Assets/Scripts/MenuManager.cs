using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Main Menu functions
/// </summary>

public class MenuManager : MonoBehaviour
{
    // Attached to the menu ui
    void Start()
    {
        /*---------Variable Init----------*/
        var data = DataManager.LoadData();
        TextMeshProUGUI highscore = transform.Find("HighScore").GetComponent<TextMeshProUGUI>();
        GameObject continueGame = transform.Find("Continue").gameObject;

        MenuButtons.Score = data.Score; // init menubuttons score for resetting

        /*--------Init UI Elements--------*/
        // set highscore
        if (data.HighScore != null) highscore.SetText($"HIGHSCORE: {((int)data.HighScore).ToString(_settings.ScoreDigits)}");
        else highscore.SetText("HIGHSCORE: NONE");

        // enable continue game button if data is found
        if (data.New) continueGame.SetActive(false);
        else continueGame.SetActive(true);
    }
}
