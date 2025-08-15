using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
#nullable enable

/// <summary>
/// DataManager static class that saves and loads data
/// </summary>
public class DataManager : MonoBehaviour
{
    // --> Used to conveniently load data without having to call PlayerPrefs all the time
    public static Data LoadData()
    {
        // Convert data into a ScriptableObject for easy access
        var data = ScriptableObject.CreateInstance<Data>();
        data.HP = PlayerPrefs.GetFloat("HP", 100);
        data.TP = PlayerPrefs.GetFloat("TP", 0);
        data.Score = PlayerPrefs.GetInt("Score", 0);
        data.HighScore = PlayerPrefs.GetInt("HighScore", data.Score);
        data.Abilities = new string?[] {
            PlayerPrefs.GetString("Passive", null),
            PlayerPrefs.GetString("Ability1", null),
            PlayerPrefs.GetString("Ability2", null),
            PlayerPrefs.GetString("Ability3", null)
        };
        data.New = !PlayerPrefs.HasKey("Saved");

        return data;
    }
    // --> Reset data when dying in game or playing a new game
    public static void ResetData()
    {
        // Deletes all data keys except for HighScore
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("HP");
        PlayerPrefs.DeleteKey("TP");
        PlayerPrefs.DeleteKey("Passive");
        PlayerPrefs.DeleteKey("Ability1");
        PlayerPrefs.DeleteKey("Ability2");
        PlayerPrefs.DeleteKey("Ability3");
        PlayerPrefs.DeleteKey("Saved");
        PlayerPrefs.DeleteKey("GoonBoss"); // wave data
        PlayerPrefs.Save();
    }
    // Also sets the high score
    public static void ResetData(int score)
    {
        // Set the current score to highscore before resetting
        SetHighScore(score);
        ResetData();
    }
    // --> Save all game data to be used in the next play session
    public static void SaveData(int score, float hp, float tp, string?[] abilities)
    {
        // game stats
        PlayerPrefs.SetFloat("HP", hp);
        PlayerPrefs.SetFloat("TP", tp);

        // score
        PlayerPrefs.SetInt("Score", score);
        SetHighScore(score);

        // save abilities
        SaveAbility("Passive", abilities[0]);
        SaveAbility("Ability1", abilities[1]);
        SaveAbility("Ability2", abilities[2]);
        SaveAbility("Ability3", abilities[3]);

        // indicate that this is not a new game
        // --> no boolean save so this is that i came up with
        PlayerPrefs.SetInt("Saved", 1);
        PlayerPrefs.Save();
    }
    // Check if the new score is higher than the highscore,
    // Then set the highscore
    public static void SetHighScore(int score)
    {
        int HighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > HighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }
    // --> Used for conveniently saving the ability
    // Saves the ability to the slot if not null
    // Deletes the slot if null
    private static void SaveAbility(string key, string? ability)
    {
        if (ability == null) PlayerPrefs.DeleteKey(key);
        else PlayerPrefs.SetString(key, ability);
    }
}
