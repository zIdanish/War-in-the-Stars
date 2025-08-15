using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using Color = UnityEngine.Color;
#nullable enable

/// <summary>
/// Entity behaviour for both enemies and the player
/// Contains all the logic you would want for the entity: Stats, HealthBar link, Movement, etc.
/// No need to create a subclass for this
/// </summary>
public class Entity : MonoBehaviour
{
    /* Init Variables */

    /*<-----------------Stats---------------->*/
    // --> set these in the entity prefab
    // --> unused in actuality
    [SerializeField] private float hp = 100;
    [SerializeField] private float spd = 65;
    [SerializeField] private float dmg = 10;
    [SerializeField] private float inv = 0; // invulnerability window
    public int score = 0; // score dropped on death from player

    // Events
    public event Action<Entity?>? OnDeath;

    // Easings
    [SerializeField] private Ease Easing = Ease.OutQuad; // entity movement & stat easing

    // Entity Vector2 position info
    private SpriteRenderer sprite = null!;
    private Vector2 destination; // target position to move to
    private Vector2 last_destination; // previous target position moving to (detects any destination changes)
    private Vector2 position; // current position

    // Orientation/LookAt variables
    private float last_angle; // will be set automatically when the entity checks if it's a player or enemy
    private float? target_angle;
    private Vector2? target_destination;
    private Transform? target_lock;

    // other stats info
    private float last_spd;
    public Transform? HealthBar { get; private set; } // healthbar to display HP on (optional), mainly for bosses and the player
    private float invulnerable = 0; // 0 means vulnerable, anything above is invulnerable (in seconds)
    public Dictionary<string, float> Default { get; private set; } = new Dictionary<string, float> { }; // Default stats of the entity
    public Dictionary<string, Dictionary<Guid, float>> Offsets { get; private set; } = new Dictionary<string, Dictionary<Guid, float>> { }; // Offset stats of the entity from tweens
    /*<------------Public Stats----------->*/
    // get stat from Default + Offset
    public float HP => GetStat("HP");
    public float SPD => GetStat("SPD");
    public float DMG => GetStat("DMG");
    public float INV => GetStat("INV");
    public float TP { get; private set; } = 0;
    private float GetStat(string stat) {
        var value = Default.GetValueOrDefault(stat, 0);
        
        foreach (var change in Offsets[stat].Values)
        {
            value += change;
        }

        return value;
    } 

    // Some stats for public to access
    public Vector2 Position { get { return position; } }
    public Vector2 Destination { get { return destination; } }
    public bool Moving { get { return Mathf.Abs(position.x - destination.x) > .1f || Mathf.Abs(position.y - destination.y) > .1f; } }
    public float Angle { get; private set; }
    public bool Invulnerable { get { return invulnerable > 0; } }
    public bool Died { get; private set; }
    /*<-----------------Config--------------->*/
    private Tween? MOVING_TO_DESTINATION; // moving tween so that it can be killed
    private Tween? ROTATING_TO_ANGLE; // rotating tween so that it can be killed
    /*<-----------------Misc--------------->*/
    // Init default stats
    public Vector2 Direction { get; private set; } = new Vector2(0,1);
    private GameManager Game = null!; // mandatory GameManager
    /*<------------Init Functions----------->*/
    private void Awake()
    {
        // set default stats
        SetStat("HP");
        SetStat("SPD");
        SetStat("DMG");
        SetStat("INV");

        // init variables
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sprite = GetComponent<SpriteRenderer>();
        last_angle = defaultAngle();
        Angle = last_angle;
        last_spd = SPD;

        // ignore this
        // just to silence the stupid stupid unity warnings for not every accessing the stats directly >:(
        var PLEASE_STOP_GIVING_ME_WARNINGS = dmg; // so i dont get a  stupid warning from unity
        var HWEJOWRHGUYIGISFGJFKBUJFBEWKUIBJFUIBJWFIE = spd; // again

        // set default position and destination
        position = (Vector2)transform.position;
        if (destination == null)
        {
            destination = position;
        }
        last_destination = destination;
    }
    private void Update()
    {
        NewDestination();
        RefreshStats();
        InvulnerableSprite();
        UpdateAngle();
    }

    /*<------------Update Functions----------->*/
    
    // Calculates the angle using the target_lock, target_destination and target_angle variables
    // Update the angle if the new angle is different from the old one
    private void UpdateAngle()
    {
        // Check if there's any angle lock
        // Calculates the angle from the difference in position between the target destination and the current position
        // Calls the function to refresh the angle of the entity to the new updated angle

        if (target_lock != null) // set the target destination to the locked transform
        {
            target_destination = target_lock.position;
        }

        if (target_destination == null && target_angle == null) { Direction = new Vector2(0, transform.CompareTag("Player") ? 1 : -1); return; }

        float angle = target_angle!=null ? (float)target_angle : defaultAngle();

        if (target_destination != null)
        {
            var direction = (Vector2)target_destination - position;
            angle += Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        angle += 90;

        RefreshAngle(angle);
    }

    // Tweens to the new angle based on a default value
    // Gets the current euler Angle and tweens it to the new one with Slerp
    // Sets the Direction every tick
    // --> lazy to add dynamic rotation speed
    private void RefreshAngle(float angle)
    {
        // Breaks this function if the angle is the same as the last angle
        // Kills any previous angle tweens
        // Sets the last angle to the current angle
        // Tweens the angle exponentially to face that direction
            // --> lazy to add a variable to change the turn speed ajniaegr[jiaegroijegrjio[aegrkop

        if (angle == last_angle) { return; }
        if (ROTATING_TO_ANGLE != null) { ROTATING_TO_ANGLE.Kill(); }

        float base_angle = last_angle; // --> cause we're setting the previous angle to the current one
        // --> and we need the previous angle to lerp properly
        last_angle = angle;

        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(0, 0, angle);

        ROTATING_TO_ANGLE = DOTween.To(
            () => 0f,
            x =>
            {
                transform.rotation = Quaternion.Slerp(start, end, x);
                Direction = new Vector2(transform.right.x, transform.right.y);
                Angle = Mathf.Lerp(base_angle, angle, x);
            },
            1f,
            1f
        ).SetEase(Ease.OutExpo).SetLink(gameObject);
    }

    // Supposedly supposed to refresh entity stats but in the end
    // it was only ever used for invulnerability to elapse
    private void RefreshStats()
    {
        if (invulnerable > 0)
        {
            invulnerable = MathF.Max(invulnerable - Time.deltaTime, 0);
        }
    }
    // Sprite effects that fade the entity in and out when in an invulnerable state
    // --> TODO: make this look more visible and better
    private void InvulnerableSprite()
    {
        if (inv <= 0) { return; }
        var c = sprite.color;
        sprite.color = new Color(c.r, c.g, c.b, MathF.Abs(.025f - (invulnerable%.05f)) * 40 );
    }

    // Triggers when the destination or spd of the player has changed since the last frame
    // Then stores the current destination and spd, kills the previoous tween
    // And start tweening the entity's position to the new position
    private void NewDestination()
    {
        // Breaks the function if no difference in speed or destination

        var spd = SPD; // --> im scared of unity's optimisation with the {get {return;}; function thing
        if (last_destination == destination && spd == last_spd) { return; }

        // Set previous variables to current
        last_destination = destination;
        last_spd = spd;

        // Cancels any previous tweens
        if (MOVING_TO_DESTINATION != null)
        {
            MOVING_TO_DESTINATION.Kill();
        }


        // Set the (non-tweened) position to the current position
        // And tweens the position towards the destination based on the time = distance/speed formula
        // Distance is the difference between the destination and current position of the object
        var currentPos = position;
        var diff = (destination - currentPos).magnitude;
        var time = diff / (spd * 5);
        MOVING_TO_DESTINATION = DOTween.To(
            () => currentPos,
            x => { transform.position = x; position = x; }, destination, time
        ).SetEase(Easing).SetLink(gameObject);
    }

    /*<-----------------Movement Functions--------------->*/

    // Sets the entity's destination to that Vector2 within the boundaries of the game
    public void MoveTo(Vector2 NewPosition)
    {
        destination = new Vector2( // clamp position in between boundaries
            Math.Clamp(NewPosition.x, -_settings.Boundaries.x, _settings.Boundaries.x),
            Math.Clamp(NewPosition.y, -_settings.Boundaries.y, _settings.Boundaries.y)
        );

        NewDestination();
    }

    // Adds the Vector2 to the entity's current destination
    public void MoveBy(Vector2 AddPosition)
    {
        MoveTo(destination + AddPosition);
    }
    // Sets the destination to random position within the magnitude of the BasePosition
    public void MoveRandom(Vector2 BasePosition, float Magnitude)
    {
        Vector2 NewPosition = new Vector2( // --> lazy to make range accurate so this is good enough
            BasePosition.x + UnityEngine.Random.Range(-Magnitude, Magnitude),
            BasePosition.y + UnityEngine.Random.Range(-Magnitude, Magnitude)
        );

        MoveTo(NewPosition);
    }

    // Sets the entity's destination to that Vector2 past boundaries of the game
    public void MoveExit(Vector2 NewPosition)
    {
        destination = NewPosition;

        NewDestination();
    }
    
    // Teleports the entity to the position
    // --> does not set destination btw
    public void SetPosition(Vector2 NewPosition)
    {
        position = NewPosition;
        transform.position = position;
    }

    // Reset the entity's orientation lookat values
    private void unLook()
    {
        target_angle = null;
        target_destination = null;
        target_lock = null;
    }
    // Resets the entity's orientation to the default orientation
    public void Look()
    {
        unLook();
        RefreshAngle(defaultAngle());
    }
    // Sets the entity's orientation to look at the target position
    public void Look(Vector2 Position)
    {
        unLook();
        target_destination = Position;
    }
    // Sets the entity's orientation to the angle
    // --> For enemy entities, add 180 degrees to the angle
    public void Look(float Angle)
    {
        unLook();
        RefreshAngle(Angle + defaultAngle());
    }
    // Sets the entity's orientation to look at the target position
    // Displaced by the angle
    public void Look(Vector2 Position, float Angle)
    {
        unLook();
        target_angle = Angle;
        target_destination = Position;
    }
    // Sets the entity's orientation to always look at the transform position
    public void Look(Transform Lock)
    {
        unLook();
        target_lock = Lock;
    }
    // Sets the entity's orientation to always look at the transform position
    // Displaced by the angle
    public void Look(Transform Lock, float Angle)
    {
        unLook();
        target_angle = Angle;
        target_lock = Lock;
    }

    /*<-----------------Entity Functions--------------->*/

    // Returns the default angle of the entity
    private float defaultAngle()
    {
        return transform.CompareTag("Player") ? 0 : 180;
    }

    /*<-----------------Stat Functions--------------->*/
    // Kills the entity,
    // Adding a score if the attacker was the player
    // Sets Healthbar to disable
    // Triggers any event functions OnDeath
    // Destroys the gameObject of this component
    // Breaks if the entity has already died as a form of debounce
    public void Die(Entity? Caster)
    {
        if (Died) return;
        Died = true;

        if (Caster != null && Caster.CompareTag("Player")) // Add score when the entity is killed by player
        {
            Game.AddScore(score);
            score = 0; // --> prevent double score from happeninging (although it shouldnt happen)
        }
        
        if (HealthBar != null && CompareTag("Enemy")) // disable HealthBar if not player
        {
            HealthBar.gameObject.SetActive(false);
        }

        AudioManager.PlaySound(AudioManager.asset.SND_Death, .5f);
        OnDeath?.Invoke(Caster);

        Destroy(gameObject);
    }
    // Deals damage to the entity (if the entity isn't invulnerable)
    // Checks if the entity has an invulnerable stat and becomes invulnerable
    // Set the HP (clamping between 0 and the current HP to prevent dealing negative damage or excess damage)
    // If the entity is linked to a display, update the display to the entity's current HP
    // If HP less than 0, entiy DIES...
    // Returns the excess damage
    public float Damage(float dmg, Entity? Caster)
    {
        if (invulnerable > 0) { return 0; } // Return if the entity is invulnerable

        if (inv > 0) // Entity becomes invulnerable when taking damage (if inv stat is above 0)
        {
            invulnerable = inv;
        }

        float lastHP = hp;
        hp = Mathf.Clamp(hp - dmg, 0, Default["HP"]);

        if (HealthBar != null)
        {
            Game.DisplayHP(HealthBar, hp, Default["HP"]);
        }

        if (this.CompareTag("Player")) AudioManager.PlaySound(AudioManager.asset.SND_Hurt);
        else AudioManager.PlaySound(AudioManager.asset.SND_Damaged, .25f);

        if (hp <= 0)
        {
            Die(Caster);
        }

        return Mathf.Clamp(dmg - lastHP + hp, 0, dmg);
    }

    // Uses up player tp to use ability
    // --> Function accessed for other scripts cause i wanna add effects
    public void UseTP(float tp)
    {
        TP = Math.Clamp(TP - tp, 0, 100);
        Game.DisplayTP(TP);
    }

    // To add player tp on graze by the 
    // --> Function accessed for other scripts cause i wanna add effects
    public void AddTP(float tp)
    {
        TP = Math.Clamp(TP + tp, 0, 100);
        Game.DisplayTP(TP);
    }

    // just adds plr hp
    // --> Updates the displayHP additionalyl!!
    public void AddHP(float addHP)
    {
        hp = Math.Max(hp + addHP, 0);
        Default["HP"] = Math.Max(hp, Default["HP"]);

        if (HealthBar == null) { return; }
        Game.DisplayHP(HealthBar, hp, Default["HP"]);
    }

    // sets the hp and tp (should only be used when loading data)
    public void SetHP(float newHP)
    {
        hp = newHP;

        if (HealthBar == null) { return; }
        Game.DisplayHP(HealthBar, hp, Default["HP"]);
    }
    public void SetTP(float newTP)
    {
        TP = newTP;
        Game.DisplayTP(TP);
    }

    // Heals the entity by the current HP (as long as its not dead)
    // If the entity is linked to a display, update the display to the entity's current HP
    // --> excuse me why is negative healing viable
    // --> if only someone was here to fix that wink wink
    public void Heal(float heal, Entity? Caster)
    {
        if (hp <= 0) { return; } // Return if the entity is dead

        AudioManager.PlaySound(AudioManager.asset.SND_Heal);
        hp = Mathf.Clamp(hp + heal, 0, Default["HP"]);

        if (HealthBar != null)
        {
            Game.DisplayHP(HealthBar, hp, Default["HP"]);
        }
    }
    // Change the stat of the entity by float, return the function to reset the stat
    public Action Stat(string stat, float change)
    {
        // Works by creating a new GUID index for the Offset Stat
        // Removes the index once reset
        // Returns the reset function
        Guid index = Guid.NewGuid();
        stat = stat.ToUpper();
        Offsets[stat][index] = change; // my life

        return () => { Offsets[stat].Remove(index); };
    }
    // Change the stat of the entity for a limited time
    public void Stat(string stat, float change, float duration)
    {
        // Uses a Coroutine to reset the stat (stolen from the previous function) after the duration ends
        var reset = Stat(stat, change);

        IEnumerator ResetStat() {
            yield return new WaitForSeconds(duration);
            reset();
        }

        StartCoroutine(ResetStat());
    }
    // Tweens stat change the stat by number, return the function to reset the stat
    public Func<float?, Coroutine> TweenStat(string stat, float change, float time)
    {
        Guid index = Guid.NewGuid();
        stat = stat.ToUpper();
        Offsets[stat][index] = 0;

        float elapsed = 0f;

        // tween by adding/substracting to the original stat instead of setting
        Tween add = DOTween.To(
            () => 0f,
            x => {
                Offsets[stat][index] = x;
                elapsed += Time.deltaTime;
            }, change, time
        ).SetEase(Easing).SetLink(gameObject);

        // function to reset the stats
        // KILLS THE PREVIOUS TWEEN
        // TWEEN AGAIN TO REMOVE THE ADDED VALUES
        // returns coroutine that WAITs for the duration of the undo tween (an argument)
        // and automatically sets the stat back to normal
        // --> so janky i hate thisv  i hate DICTIONARIES I HATE UNTIY
        Coroutine reset(float? custom)
        {
            float time = (float)(custom != null ? custom : elapsed);

            add.Kill(); // --> I FOUND OUT NOT HAVING THIS WAS THE REASON WHY IT WONT SLOW BACK DOWN AAAAAAAAAAAAAAAAAAAAAAAAAAAA
            // --> UNITY SUCKSSSSS

            Tween undo = DOTween.To(
                () => Offsets[stat][index],
                x => {
                    Offsets[stat][index] = x;
                }, 0f, time
            ).SetEase(Easing).SetLink(gameObject);

            IEnumerator remove()
            {
                yield return new WaitForSeconds(time);
                undo.Kill();

                if (!Offsets[stat].ContainsKey(index)) { yield break; }
                yield return null;

                Offsets[stat].Remove(index);
            }

            return StartCoroutine(remove());
        }

        return reset;
    }

    // Set the stat as a default stat (used only in Start/Awake)
    private void SetStat(string stat)
    {
        // init string values since entity stat uses lower, dictionary stats use upper (why did i do this?)
        // gets the field value, then sets the default value to the field value in this entity
        // create an offset dictionary for value offset setting
        var index = stat.ToUpper();
        stat = stat.ToLower();

        FieldInfo field = GetType().GetField(stat, BindingFlags.Instance | BindingFlags.NonPublic);

        // check if the field is not null and is a float
        if (field == null) { Debug.Log($"Field - {stat} - does not exist"); return; }
        if (field.FieldType != typeof(float)) { Debug.Log($"Stat - {stat} - is not a float"); return; }

        var value = (float)field.GetValue(this);
        Default[index] = value;
        Offsets[index] = new Dictionary<Guid, float> { };
    }
    // Sets the invulnerable value to the duration
    public void SetInvulnerable(float duration)
    {
        invulnerable = MathF.Max(invulnerable, duration);
    }
    // Toggles the invulnerable on or off
    public void SetInvulnerable(bool toggle)
    {
        // Set invulnerable to infinity cause we dont want the invulnerability to expire
        invulnerable = toggle ? Mathf.Infinity : 0;
    }
    // Link a Display UI (transform) to the entity
    // Refreshes the display instantly
    public void DisplayHPBar(Transform bar)
    {
        HealthBar = bar;
        Game.DisplayHP(bar, hp, Default["HP"]);
    }

    /*<-----------------Shoot Functions--------------->*/
    // Creates a projectile that moves towards an angle
    // --> is affected by the entity look direction
    public Projectile Shoot(GameObject projectile, float spd, float angle)
    {
        angle += Angle;
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, angle);
        Component.Caster = this;
        return Component;
    }
    // Creates a projectile that moves towards Vector2 direction (calculated instantly from destination and projectile position)
    public Projectile Shoot(GameObject projectile, float spd, Vector2 destination)
    {
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, destination);
        Component.Caster = this;
        return Component;
    }
    // Creates a projectile that moves towards Vector2 direction (calculated instantly from destination and projectile position)
    // Displaced by the angle
    public Projectile Shoot(GameObject projectile, float spd, Vector2 destination, float angle)
    {
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, destination, angle);
        Component.Caster = this;
        return Component;
    }

    /*<-----------------Static Entity Functions--------------->*/
    // Gets the player entity in game
    // --> might not exist cause die
    public static Entity? getPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return null; }

        return player.GetComponent<Entity>();
    }
}