using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#nullable enable

/// <summary>
/// Button functions for GameUI
/// </summary>
public class GameButtons : MonoBehaviour
{
    /* Public variables (ADD TO CANVAS) */
    public GameObject? pauseScreen;
    public GameObject? deathScreen;

    /* Static Variables */
    public static bool CanPause = true;
    public static GameObject PauseScreen = null!; // --> why does setting object to disabled not let you find it
    public static GameObject DeathScreen = null!; // --> why does setting object to disabled not let you find it
    public static GameManager Game { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); } }
    public static Cursor CustomCursor { get { return GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>(); } }
    private void Awake()
    {
        if (pauseScreen!=null) PauseScreen = pauseScreen;
        if (deathScreen != null) { DeathScreen = deathScreen; deathScreen.SetActive(false); }
    }

    /* Game Menu */
    public static void TogglePause()
    {
        if (Game.Paused) UnPause();
        else Pause();
    }
    public static void Pause()
    {
        if (!CanPause) { return; }
        PauseScreen.SetActive(true);
        Game.Pause(true);
        CustomCursor.enabled = false; // disable custom cursor on pause
    }
    public static void UnPause()
    {
        PauseScreen.SetActive(false);
        Game.Pause(false);
        CustomCursor.enabled = true; // enable custom cursor on pause
    }
    public static void Retry()
    {
        CanPause = true;
        SceneManager.LoadScene("Level");
    }
    public static void Leave()
    {
        // Save HighScore in case the player resets in the middle of a wave
        DataManager.SetHighScore(Game.Score);
        SceneManager.LoadScene("Menu");
    }
}