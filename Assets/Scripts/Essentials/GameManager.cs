using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    /* Init Variables */
    /*<-----------------Game Variables---------------->*/
    protected int score = 0;
    protected Vector2 bounds = new Vector2();
    /*<----------------------------------------------->*/
    private Transform UI;
    private TextMeshProUGUI ScoreUI;
    private Transform Health;
    private Image HealthBar;
    private TextMeshProUGUI HealthDisplay;
    private GameObject Entities;
    private Transform projectile_folder;

    private void Awake() // Hide windows cursor when the game is loaded
    {
        HideCursor();
    }

    /* Public Variables */
    public int Score { get { return score; } }

    /* Init Functions */
    protected virtual void Init()
    {
        Entities = GameObject.FindGameObjectWithTag("Entities");
        UI = GameObject.FindGameObjectWithTag("UI").transform;
        ScoreUI = UI.Find("Score").GetComponent<TextMeshProUGUI>();
        projectile_folder = GameObject.FindGameObjectWithTag("Projectiles").transform;
        bounds = _settings.Boundaries;

        Health = UI.Find("Health");
        HealthBar = Health.Find("Bar").GetComponent<Image>();
        HealthDisplay = Health.Find("Display").GetComponent<TextMeshProUGUI>();

        StartCoroutine(Timeline());
    }
    protected virtual IEnumerator Timeline()
    {
        yield return null;
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
        ScoreUI.SetText(score.ToString("D7"));
    }
    public void DisplayHP(float hp, float maxHP)
    {
        RectTransform hpbar = HealthBar.rectTransform;
        RectTransform frame = hpbar.parent as RectTransform;
        float width = frame.rect.width * (1 - (hp / maxHP));

        int display = (int)hp;
        HealthDisplay.SetText(display.ToString());

        HealthBar.rectTransform.offsetMax = new Vector2(-(1.0f + width), -1.0f);
    }

    // Spawns an enemy at the position, and moves towards target position
    public Entity SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        GameObject Enemy = Instantiate(enemy);
        Entity Component = Enemy.GetComponent<Entity>();
        Component.SetPosition(position);
        Component.MoveTo(targetPosition);
        Enemy.transform.SetParent(Entities.transform);
        return Component;
    }

    // Spawn Projectiles
    public Projectile Shoot(GameObject projectile, Vector2 position, string target, float spd, float angle) // Creates a projectile that moves at an angle
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector2 destination = position + new Vector2( // Calculate the destination vector (goes offscreen)
            Mathf.Sin(radians),
            Mathf.Cos(radians)
        );

        Projectile Component = Shoot(projectile, position, target, spd, destination);
        return Component;
    }
    public Projectile Shoot(GameObject projectile, Vector2 position, string target, float spd, Vector2 destination) // Creates a projectile that moves towards Vector2 direction
    {
        GameObject Projectile = Instantiate(projectile);
        Projectile Component = Projectile.GetComponent<Projectile>();
        Component.TARGET = target;
        Component.SPD = spd;
        Component.Position = position;
        Component.MoveTo(destination);
        Projectile.transform.parent = projectile_folder;
        return Component;
    }
    public Projectile Shoot(GameObject projectile, Vector2 position, string target, float spd, Vector2 destination, float angle) // Creates a projectile that moves towards Vector2 direction in an angle
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        Vector2 dif = destination - position;
        Vector2 direction = new Vector2( // calculate the new direction of the vector
            dif.x * cos - dif.y * sin,
            dif.x * sin + dif.y * cos
        );

        destination = position + direction; // set the new destination

        Projectile Component = Shoot(projectile, position, target, spd, destination);
        return Component;
    }
}