using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

/// <summary>
/// Level core class
/// All the game events, enemy spawning, projectile spawning, game stats calculation and storage is in this class
/// Although i wished i kept the stats and functions in separate scripts
/// but its too late now
/// </summary>

// --> this script is a giant mess that piled up as i added more and got lazy to fix cause its like 2 days left before submission :(
// --> pulau semakau got nothing on this
public class GameManager : MonoBehaviour
{
    /* Init Variables */
    /*<-----------------Game Variables---------------->*/
    public bool Ended { get; private set; } = false;
    public bool Paused { get; private set; } = false;
    public Dictionary<IEnumerator, int> WaveEvents { get; protected set; } = new Dictionary<IEnumerator, int>();
    public int EntityCount { get { return Entities.childCount; } }
    public int EntityLimit = 50;
    [NonSerialized] public Entity player = null!;
    [NonSerialized] public Player playerManager = null!;
    public int score { get; private set; } = 0;
    protected Vector2 bounds = new Vector2(); // Easier to access _settings.Boundaries
    [NonSerialized] public Transform PlayerHealth = null!;
    [NonSerialized] public Transform BossHealth = null!;

    // Prefabs
    public Abilities abilities = null!;
    public Projectiles projectiles = null!;
    public Enemies enemies = null!;
    public LevelInfo levelInfo = null!;

    /*<----------------------------------------------->*/
    public Transform UI { get; protected set; } = null!;
    public TextMeshProUGUI ScoreUI { get; protected set; } = null!;
    public Transform TPUI { get; protected set; } = null!;
    public Transform Entities { get; protected set; } = null!;
    public Transform Projectiles { get; protected set; } = null!;

    /* Public Variables */
    public int Score { get { return score; } } // --> idk why this is here but i'll just keep it

    /* Init Functions */
    protected virtual void Init()
    {
        // In the event i forget to disable a gameManager, it deletes all of them except for the one of the highest priority
        if (GetComponent<GameManager>()!=this) { enabled = false; Destroy(this); return; }

        Entities = GameObject.FindGameObjectWithTag("Entities").transform;
        UI = GameObject.FindGameObjectWithTag("UI").transform;
        ScoreUI = UI.Find("Score").GetComponent<TextMeshProUGUI>();
        TPUI = UI.Find("PlayerTension");
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
        StartCoroutine(InitPlayer());

        // start the timeline
        StartCoroutine(Timeline());

        // DEBUG
        CheckPlayerAbilities();
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

        player = getPlayer;
        playerManager = getPlayer.GetComponent<Player>();

        // Get Player Data
        var data = DataManager.LoadData();

        // end the game on player killed
        player.DisplayHPBar(PlayerHealth);
        player.OnDeath += End;

        // Set game & player values from data
        SetScore(data.Score);
        player.SetHP(data.HP);
        player.SetTP(data.TP);

        if (data.New)
            // No data: Activate player abilities
            DefaultAbilities();
        else
            // Has - Activate player abilities
            SetAbilities(data.Abilities);

        // --> Unpause game in case
        GameButtons.UnPause();

        // --> nothing beats two unpauses
        yield return new WaitForSeconds(1);
        GameButtons.UnPause();
    }
    protected virtual void DefaultAbilities()
    {
        Debug.Log("No Default Abilities?!?");
    }
    private void SetAbilities(string?[] abilities)
    {
        for (int i = 0; i < abilities.Length; i++) {
            var ability = abilities[i];
            if (ability == null || ability == "") continue;

            SetAbility(ability, i);
        }
    }
    protected virtual IEnumerator Timeline()
    {
        // yield until player exists
        while (player == null) {
            yield return null;
        }

        // begin the wave loop
        while (true)
        {
            PlayWaveEvents();
            SaveGame();

            yield return StartCoroutine(NewWave());
        }
    }
    private void PlayWaveEvents()
    {
        foreach (var waveEvent in WaveEvents.Keys.ToList())
        {
            if (WaveEvents[waveEvent] >= score) { continue; }

            StartCoroutine(waveEvent);
            WaveEvents.Remove(waveEvent);
        }
    }
    private void SaveGame()
    {
        if (Ended) return;

        string?[] abilities = {null,null,null,null};
        for (int i = 0; i < playerManager.Abilities.Length; i++) {
            var ability = playerManager.Abilities[i];
            if (ability == null) continue;

            abilities[i] = ability.GetType().Name;
        }

        DataManager.SaveData(score, player.HP, player.TP, abilities);
    }
    protected virtual IEnumerator NewWave()
    {
        yield return null;
    }
    protected void ScoreEvent(int score, IEnumerator waveEvent)
    {
        WaveEvents[waveEvent] = score;
    }

    /* Game Functions */

    // Game behaviour
    public void Pause()
    {
        if (Ended) return;
        Pause(!Paused);
    }
    public void Pause(bool paused)
    {
        if (Ended) return;
        Paused = paused;
        Time.timeScale = Paused ? 0f : 1f;
    }
    public void End(Entity? _)
    {
        if (Ended) return;
        Ended = true;
        Time.timeScale = 1f;
        if (gameObject.activeInHierarchy) StartCoroutine(OnGameEnd()); // prevent any errors
    }

    // General Functions

    // Selects a random coroutine in the arguments to activate
    public Coroutine RandomPattern(params IEnumerator[] patterns)
    {
        return StartCoroutine(patterns[UnityEngine.Random.Range(0, patterns.Length)]);
    }

    // Returns true or false depending on the chance value
    public bool RandomChance(float chance)
    {
        return UnityEngine.Random.Range(0f, 100f) < chance;
    }

    // Wait Functions
    public IEnumerator WaitUntilDied()
    {
        bool has_enemy = true;
        while (has_enemy)
        {
            has_enemy = false;
            foreach (var value in Entities.GetComponentsInChildren<Entity>())
            {
                if (value.CompareTag("Player")) { continue; }

                has_enemy = true;
                yield return null; 
                break; 
            }
        }
    }
    public IEnumerator WaitUntilDied(Entity entity)
    {
        while (!Ended && !entity.IsDestroyed())
        {
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
        // delete any prvious abilities
        if (playerManager.Abilities[slot] != null)
        {
            Destroy(playerManager.Abilities[slot]);
        }

        // create new ability component
        var type = Type.GetType(abilityName);
        Ability ability = (Ability)player.gameObject.AddComponent(type);

        // store ability in playerManager so it can be deleted when adding a new ability in that slot
        playerManager.Abilities[slot] = ability;

        if (slot > 0)
        {
            UILinkAbility(ability, slot);
        } else {
            ability.Link();
        }

        return ability;
    }
    private void UILinkAbility(Ability ability, int slot)
    {
        var keybind = _settings.AbilityKeybinds[slot - 1];
        var icon = UI.Find($"Ability{slot}");
        ability.icon = icon;
        ability.input.AddBinding($"<Keyboard>/{keybind}");

        // init icon
        icon.Find("Keybind").GetComponent<TextMeshProUGUI>().SetText(keybind.ToUpper());
        icon.gameObject.SetActive(true);

        ability.Link();

        // --> tpp
        if (ability.TP != null)
        {
            icon.Find("TP").GetComponent<TextMeshProUGUI>().SetText($"{ability.TP} TP");
            icon.Find("TP").gameObject.SetActive(true);
        }
        else
        {
            icon.Find("TP").gameObject.SetActive(false);
        }
    }

    // Increases the score
    public void AddScore(int _score)
    {
        SetScore(score + _score);
    }
    protected void SetScore(int _score)
    {
        if (Ended || _score < score) return;
        score = _score;

        // refresh score text
        ScoreUI.SetText(score.ToString(_settings.ScoreDigits));
    }

    // Display Stat on the ui statbar
    public void DisplaySTAT(Transform ui, float stat, float maxStat)
    {
        if (Ended) return;

        Image StatBar = ui.Find("Bar").GetComponent<Image>();
        TextMeshProUGUI StatDisplay = ui.Find("Display").GetComponent<TextMeshProUGUI>();

        RectTransform rect = StatBar.rectTransform;
        RectTransform? frame = rect.parent as RectTransform;
        if (frame == null) { Debug.Log("STAT Frame not found!"); return; }

        float width = frame.rect.width * (1 - (stat / maxStat));

        int display = (int)stat;
        StatDisplay.SetText(display.ToString());

        StatBar.rectTransform.offsetMax = new Vector2(-(1.0f + width), -1.0f);
    }

    // Display TP & HP on UI
    public void DisplayHP(Transform ui, float hp, float maxHP)
    {
        DisplaySTAT(ui, hp, maxHP);
    }
    public void DisplayTP(float tp)
    {
        DisplaySTAT(TPUI, tp, 100);
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
        Component.DisplayHPBar(BossHealth);
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
        GameObject Alert = Instantiate(projectiles.Warning);
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

    // Loops through each ability to see if the component exists or not
    // Then spawns an ability orb projectile
    public PJ_Ability? SpawnAbility(int slot)
    {
        string[] abilities = (string[])(
            // theres probably a better way than to do something like this
            slot == 1 ? levelInfo.Abilities1 :
            slot == 2 ? levelInfo.Abilities2 :
            slot == 3 ? levelInfo.Abilities3 :
            levelInfo.Passives).Clone();

        Ability? currentAbility = playerManager.Abilities[slot];

        // Filters currently existing abilities
        if (currentAbility != null)
        {
            var abilityName = currentAbility.GetType().Name;
            if (abilities.Contains(abilityName))
            {
                abilities = abilities.Where(v => v != abilityName).ToArray();
            }
        }

        // Does not spawn an orb if there's no abilities
        if (abilities.Length == 0) { return null; }

        var x = UnityEngine.Random.Range(-50f, 50f);
        var Ability = (PJ_Ability)Shoot(projectiles.AbilityOrb, new Vector2(x, _settings.Height + 20), "Player", 10f, -180);

        Ability.NAME = abilities[UnityEngine.Random.Range(0, abilities.Length)];
        Ability.SLOT = slot;

        Ability.Accelerate(-8, 4f);
        return Ability;
    }

    // Plays on player death
    // Waits a bit before showing the game over screen
    private IEnumerator OnGameEnd()
    {
        GameButtons.CanPause = false;
        GameButtons.UnPause();
        DataManager.ResetData(score);
        playerManager.Cursor.enabled = false;

        yield return new WaitForSeconds(1);

        AudioManager.PlaySong(AudioManager.asset.End);

        yield return new WaitForSeconds(1);

        var data = DataManager.LoadData();
        var deathScreen = GameButtons.DeathScreen;
        var parent = deathScreen.transform;
        deathScreen.SetActive(true);

        // display score and highscor
        var uiscore = parent.Find("Score").GetComponent<TextMeshProUGUI>();
        var uihighscore = parent.Find("HighScore").GetComponent<TextMeshProUGUI>();
        // --> made 6 hours before deadline :D
        uiscore.SetText($"SCORE: {score.ToString(_settings.ScoreDigits)}");
        if (data.HighScore != null) uihighscore.SetText($"HIGHSCORE: {((int)data.HighScore).ToString(_settings.ScoreDigits)}");
        else uihighscore.SetText("HIGHSCORE: NONE"); // shouldn't be possible but just in case
    }

    // Debug Functions

    // Checks if the ability from an ability string in LevelInfo exists
    // --> In case a typo happens and i waste a few hours finding it (happened for many other things in this codebase)
    // --> Why cant i just use the Ability class instead??
    // --> ...
    private void CheckPlayerAbilities()
    {
        void SearchThrough(string[] array)
        {
            foreach (string name in array)
            {

                // check if the ability type exists in the first place
                // if not then warn
                Type type = Type.GetType(name);
                if (type == null)
                {
                    Debug.LogWarning($"Type {name} does not EXIST.");
                }
            }
        }

        SearchThrough(levelInfo.Passives);
        SearchThrough(levelInfo.Abilities1);
        SearchThrough(levelInfo.Abilities2);
        SearchThrough(levelInfo.Abilities3);
    }
}