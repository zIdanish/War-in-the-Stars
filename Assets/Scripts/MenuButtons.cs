using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#nullable enable

/// <summary>
/// Button functions for the game menu
/// </summary>
public class MenuButtons : MonoBehaviour
{
    /* Public variables (ADD TO MENU CANVAS) */
    public GameObject? instructions;
    public GameObject[]? pages;

    /* Static Variables */
    public static int CurrentPage { get; private set; } = 1;
    public static GameObject[] Pages { get; private set; } = new GameObject[0]; // --> why does setting object to disabled not let you find it
    public static GameObject Instructions = null!; // --> why does setting object to disabled not let you find it
    public static int? Score;
    private void Awake()
    {
        if (instructions != null) { 
            Instructions = instructions;
            Instructions.SetActive(false);
        }

        if (pages != null && pages.Length > 0) {
            Pages = pages;
            SetPage(1);
        }
    }

    /* Main Menu */
    public static void ContinueGame()
    {
        SceneManager.LoadScene("Level");
    }
    public static void NewGame()
    {
        // The highscore should already be saved in game
        DataManager.ResetData();
        SceneManager.LoadScene("Level");
    }
    public static void QuitGame()
    {
        Application.Quit();
    }
    public static void ToggleInstructions()
    {
        Instructions.SetActive(!Instructions.activeSelf);
    }
    public static void NextPage()
    {
        SetPage(Mathf.Min(CurrentPage+1, Pages.Length));
    }
    public static void BackPage()
    {
        SetPage(Mathf.Max(CurrentPage - 1, 1));
    }
    public static void SetPage(int page) {
        CurrentPage = page; // set the current page for next/back functions

        foreach (var Page in Pages)
        {
            // check if the index is the same as the current page (by getting the number from the name)
            // sets to inactive if not
            // --> did this before realising i could just get the index of the list lol
            var index = int.Parse(Page.name[4..]);
            Page.SetActive(index == page);
        }
    }
}
