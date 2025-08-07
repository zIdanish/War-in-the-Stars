using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
#nullable enable

public class GameManager : MonoBehaviour
{
    /* Init Variables */
    /*<-----------------Game Variables---------------->*/
    public bool Ended { get; private set; } = false;
    public bool Paused { get; private set; } = false;
    public int EntityCount { get { return Entities.childCount; } }
    public int EntityLimit = 50;
    [NonSerialized] public Entity player = null!;
    public int score { get; protected set; } = 0;
    protected Vector2 bounds = new Vector2();
    /*<----------------------------------------------->*/
    protected Transform UI = null!;
    private TextMeshProUGUI ScoreUI = null!;
    private Transform Health = null!;
    private Image HealthBar = null!;
    private TextMeshProUGUI HealthDisplay = null!;
    private Transform Entities = null!;
    private Transform Projectiles = null!;

    private void Awake() // Hide windows cursor when the game is loaded
    {
        HideCursor();

        // Get Player
        Entity? getPlayer = Entity.getPlayer();
        if (getPlayer == null) { Debug.Log("No player found!"); return; }
        player = getPlayer.GetComponent<Entity>();
    }

    /* Public Variables */
    public int Score { get { return score; } }

    /* Init Functions */
    protected virtual void Init()
    {
        Entities = GameObject.FindGameObjectWithTag("Entities").transform;
        UI = GameObject.FindGameObjectWithTag("UI").transform;
        ScoreUI = UI.Find("Score").GetComponent<TextMeshProUGUI>();
        Projectiles = GameObject.FindGameObjectWithTag("Projectiles").transform;
        bounds = _settings.Boundaries;

        Health = UI.Find("Health");
        HealthBar = Health.Find("Bar").GetComponent<Image>();
        HealthDisplay = Health.Find("Display").GetComponent<TextMeshProUGUI>();

        PlayerAbilities();
        StartCoroutine(Timeline());
    }
    protected virtual void PlayerAbilities()
    {
        Debug.Log("No Player Abilities?!?");
    }
    protected virtual IEnumerator Timeline()
    {
        while (true)
        {
            yield return StartCoroutine(NewWave());
        }
    }
    protected virtual IEnumerator NewWave()
    {
        yield return null;
    }
    private void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    /* Game Functions */

    // Game behaviour
    public void Pause()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0f : 1f;
    }
    public void Pause(bool paused)
    {
        Paused = paused;
        Time.timeScale = Paused ? 0f : 1f;
    }

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
        RectTransform? frame = hpbar.parent as RectTransform;
        if (frame == null) { Debug.Log("HP Frame not found!"); return; }

        float width = frame.rect.width * (1 - (hp / maxHP));

        int display = (int)hp;
        HealthDisplay.SetText(display.ToString());

        HealthBar.rectTransform.offsetMax = new Vector2(-(1.0f + width), -1.0f);
    }

    // Spawns an enemy at the position, and moves towards target position
    public Entity? SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        if (EntityCount > EntityLimit) { return null; }
        GameObject Enemy = Instantiate(enemy);
        Enemy.transform.position = position;
        Entity Component = Enemy.GetComponent<Entity>();
        Component.SetPosition(position);
        Component.MoveTo(targetPosition);
        Enemy.transform.SetParent(Entities);
        return Component;
    }
    public Entity? SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition, Action<Entity> action)
    {
        Entity? Component = SpawnEnemy(enemy, position, targetPosition);
        if (Component != null) {
            action(Component);
        }

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
        Projectile.transform.parent = Projectiles;
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