using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using Image = UnityEngine.UI.Image;
#nullable enable

// --> mess that piled up as i added more and got lazy to fix cause its like 2 days left before submission :(
public class GameManager : MonoBehaviour
{
    /* Init Variables */
    /*<-----------------Game Variables---------------->*/
    public bool Ended { get; private set; } = false;
    public bool Paused { get; private set; } = false;
    public AbilityAssets Assets = null!;
    public int EntityCount { get { return Entities.childCount; } }
    public int EntityLimit = 50;
    [NonSerialized] public Entity player = null!;
    public int score { get; protected set; } = 0;
    protected Vector2 bounds = new Vector2();
    [NonSerialized] public Transform PlayerHealth = null!;
    [NonSerialized] public Transform BossHealth = null!;

    // Prefabs
    public GameObject Warning = null!;
    /*<----------------------------------------------->*/
    public Transform UI { get; protected set; } = null!;
    public TextMeshProUGUI ScoreUI { get; protected set; } = null!;
    public Transform Entities { get; protected set; } = null!;
    public Transform Projectiles { get; protected set; } = null!;

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

        PlayerHealth = UI.Find("PlayerHealth");
        BossHealth = UI.Find("BossHealth");

        // disable boss healthbar & ability uis
        BossHealth.gameObject.SetActive(false);
        UI.Find("Ability1").gameObject.SetActive(false);
        UI.Find("Ability2").gameObject.SetActive(false);
        UI.Find("Ability3").gameObject.SetActive(false);

        // start the other init functions
        StartCoroutine(Timeline());
        HideCursor();
        StartCoroutine(InitPlayer());
    }
    private IEnumerator InitPlayer()
    {
        // Get Player
        Entity? getPlayer = Entity.getPlayer();
        while (getPlayer == null)
        {
            getPlayer = Entity.getPlayer();
            yield return null;
        }
        player = getPlayer.GetComponent<Entity>();

        // Set Healthbar
        player.DisplayBar(PlayerHealth);

        // Activate player abilities
        PlayerAbilities();
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
        if (Ended) { return; }
        Pause(!Paused);
    }
    public void Pause(bool paused)
    {
        Paused = paused;
        Time.timeScale = Paused ? 0f : 1f;
    }
    public void End()
    {
        if (Ended) { return; }
        Ended = true;
        Time.timeScale = 1f;
    }

    // General Functions

    // Selects a random coroutine in the arguments to activate
    public Coroutine RandomPattern(params IEnumerator[] patterns)
    {
        return StartCoroutine(patterns[UnityEngine.Random.Range(0, patterns.Length)]);
    }

    // Wait Functions
    public IEnumerator WaitUntilDied(Entity entity)
    {
        while (!Ended && !entity.IsDestroyed()) { 
            yield return null;
        }
    }

    // Game Functions

    // Sets the ability slot of the player entity to a new ability (of type abilityName)
    // Gets the type of the abilityName, adds the component to the player gameobject
    // Checks if the slot is an active slot and sets the icon Transform and Keybinding of the ability respectively
    // Links the ability to begin the timeline
    public Ability SetAbility(string abilityName, int slot)
    {
        var type = Type.GetType(abilityName);
        Ability ability = (Ability)player.gameObject.AddComponent(type);

        if (slot > 0)
        {
            var keybind = _settings.AbilityKeybinds[slot - 1];
            var icon = UI.Find($"Ability{slot}");
            ability.icon = icon;
            ability.input.AddBinding($"<Keyboard>/{keybind}");

            // init icon
            icon.Find("Keybind").GetComponent<TextMeshProUGUI>().SetText(keybind.ToUpper());
            icon.gameObject.SetActive(true);
        }

        ability.Link();
        return ability;
    }

    // Increases the score
    public void AddScore(int _score)
    {
        if (Ended || _score <= 0) return;
        score += _score;

        // refresh score text
        ScoreUI.SetText(score.ToString("D9"));
    }

    // Display HP on the UI bar from the Transform
    public void DisplayHP(Transform health, float hp, float maxHP)
    {
        if (Ended) { return; }

        Image HealthBar = health.Find("Bar").GetComponent<Image>();
        TextMeshProUGUI HealthDisplay = health.Find("Display").GetComponent<TextMeshProUGUI>();

        RectTransform hpbar = HealthBar.rectTransform;
        RectTransform? frame = hpbar.parent as RectTransform;
        if (frame == null) { Debug.Log("HP Frame not found!"); return; }

        float width = frame.rect.width * (1 - (hp / maxHP));

        int display = (int)hp;
        HealthDisplay.SetText(display.ToString());

        HealthBar.rectTransform.offsetMax = new Vector2(-(1.0f + width), -1.0f);
    }

    // Spawns an enemy at the position, and moves towards target position
    public Entity? SpawnEnemy(GameObject enemy, Vector2 position)
    {
        if (Ended || EntityCount > EntityLimit) { return null; }
        GameObject Enemy = Instantiate(enemy);
        Entity Component = Enemy.GetComponent<Entity>();
        Component.SetPosition(position);
        Component.MoveExit(position);
        Enemy.transform.SetParent(Entities);
        Enemy.GetComponent<SpriteRenderer>().sortingOrder = _settings.zEnemy;
        return Component;
    }
    public Entity? SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        if (Ended) { return null; }

        Entity? Component = SpawnEnemy(enemy, position);
        if (Component != null)
        {
            Component.MoveTo(targetPosition);
        }

        return Component;
    }
    public Entity? SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition, Action<Entity> action)
    {
        if (Ended) { return null; }

        Entity? Component = SpawnEnemy(enemy, position, targetPosition);
        if (Component != null)
        {
            action(Component);
        }

        return Component;
    }
    public Entity? SpawnBoss(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        if (Ended) { return null; }
        BossHealth.gameObject.SetActive(true);
        GameObject Enemy = Instantiate(enemy);
        Entity Component = Enemy.GetComponent<Entity>();
        Component.SetPosition(position);
        Component.MoveTo(targetPosition);
        BossHealth.Find("Name").GetComponent<TextMeshProUGUI>().SetText(enemy.name);
        Component.DisplayBar(BossHealth);
        BossHealth.gameObject.SetActive(true);
        Enemy.transform.SetParent(Entities);
        Enemy.GetComponent<SpriteRenderer>().sortingOrder = _settings.zEnemy-1;
        return Component;
    }
    public Entity? SpawnBoss(GameObject enemy, Vector2 position, Vector2 targetPosition, Action<Entity> action)
    {
        if (Ended) { return null; }

        Entity? Component = SpawnBoss(enemy, position, targetPosition);
        if (Component != null)
        {
            action(Component);
        }

        return Component;
    }

    // Projectiles
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
        Projectile.GetComponent<SpriteRenderer>().sortingOrder = target != "Player" ? _settings.zPlayerProjectile : _settings.zEnemyProjectile;
        Projectile Component = Projectile.GetComponent<Projectile>();
        Component.TARGET = target;
        Component.SPD = spd;
        Component.SetPosition(position);
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
    public GameObject Warn(float duration, float size, Transform transform)
    {
        GameObject Alert = _Warn(duration, size);
        StartCoroutine(_Snap(Alert, transform, 0));
        return Alert;
    }
    public GameObject Warn(float duration, float size, Transform transform, float dist)
    {
        GameObject Alert = _Warn(duration, size);
        StartCoroutine(_Snap(Alert, transform, dist));
        return Alert;
    }
    public GameObject Warn(float duration, float size, Vector2 position)
    {
        GameObject Alert = _Warn(duration, size);

        Alert.transform.position = position;
        return Alert;
    }
    public GameObject Warn(float duration, float size, Vector2 position, float angle)
    {
        GameObject Alert = _Warn(duration, size);

        Alert.transform.position = position;
        Alert.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        return Alert;
    }
    private GameObject _Warn(float duration, float size)
    {
        GameObject Alert = Instantiate(Warning);
        Alert.transform.SetParent(Projectiles);
        Alert.transform.localScale = new Vector3(size, 10, size);
        Laser Component = Alert.GetComponent<Laser>();
        Component.Begin(duration);
        return Alert;
    }
    private IEnumerator _Snap(GameObject Alert, Transform transform, float dist)
    {
        while (!Alert.IsDestroyed())
        {
            Alert.transform.localPosition = transform.localPosition + transform.up * dist;
            Alert.transform.localRotation = transform.localRotation;
            yield return null;
        }
    }
}