using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    /* Init Variables */
    /*<-----------------Game Variables---------------->*/
    private int score = 0;
    /*<----------------------------------------------->*/
    private Transform UI;
    private TextMeshProUGUI ScoreUI;
    private GameObject Entities;

    private void Awake() // Hide windows cursor when the game is loaded
    {
        HideCursor();
    }

    /* Public Variables */
    public int Score { get { return score; } }

    /* Init Functions */
    public void Init()
    {
        Entities = GameObject.FindGameObjectWithTag("Entities");
        UI = GameObject.FindGameObjectWithTag("UI").transform;
        ScoreUI = UI.Find("Score").GetComponent<TextMeshProUGUI>();
    }
    private void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    /* Game Functions */

    // Increases the score
    public void AddScore(int _score)
    {
        if (_score <= 0) return;
        score += _score;

        // refresh score text
        ScoreUI.SetText("Score : " + score.ToString());
    }

    // Spawns an enemy at the position, and moves towards target position
    public Entity SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        GameObject Enemy = Instantiate(enemy);
        Entity Component = Enemy.GetComponent<Entity>();
        Component.MoveTo(targetPosition);
        Enemy.transform.position = position;
        Enemy.transform.SetParent(Entities.transform);
        return Component;
    }
}