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
    [SerializeField] private int score = 0;

    // Easings
    [SerializeField] private Ease Easing = Ease.OutQuad; // entity movement & stat easing

    // Entity Vector2 info, destination is the target position the entity is moving towards.
    private SpriteRenderer sprite = null!;
    private Vector2 destination;
    private Vector2 last_destination;
    private Vector2 position;

    // Orientation variables
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
    private float GetStat(string stat) {
        var value = Default.GetValueOrDefault(stat, 0);
        
        foreach (var change in Offsets[stat].Values)
        {
            value += change;
        }

        return value;
    } 
    public Vector2 Position { get { return position; } }
    public Vector2 Destination { get { return destination; } }
    public bool Moving { get { return Mathf.Abs(position.x - destination.x) > .1f || Mathf.Abs(position.y - destination.y) > .1f; } }
    public float Angle { get; private set; }
    public bool Invulnerable { get { return invulnerable > 0; } }
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

        var PLEASE_STOP_GIVING_ME_WARNINGS = dmg; // so i dont get a  stupid warning from unity
        var HWEJOWRHGUYIGISFGJFKBUJFBEWKUIBJFUIBJWFIE = spd; // again

        // set default position
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
        if (angle == last_angle) { return; }
        if (ROTATING_TO_ANGLE != null) { ROTATING_TO_ANGLE.Kill(); }

        float base_angle = last_angle;
        last_angle = angle;

        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(0, 0, angle);

        DOTween.To(
            () => 0f,
            x =>
            {
                transform.rotation = Quaternion.Slerp(start, end, x);
                Direction = new Vector2(transform.up.x, transform.up.y);
                Angle = Mathf.Lerp(base_angle, angle, x);
            },
            1f,
            1f
        ).SetEase(Ease.OutExpo).SetLink(gameObject);
    }
    private void RefreshStats() // Updates the entity's stats
    {
        if (invulnerable > 0) // Elapse invulnerable
        {
            invulnerable = MathF.Max(invulnerable - Time.deltaTime, 0);
        }
    }
    private void InvulnerableSprite()
    {
        if (inv <= 0) { return; }
        var c = sprite.color;
        sprite.color = new Color(c.r, c.g, c.b, MathF.Abs(.025f - (invulnerable%.05f)) * 40 );
    }
    private void NewDestination()
    {
        var spd = SPD;
        if (last_destination != destination || spd != last_spd)
        {
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
    }

    /* Movement Functions */
    public void MoveTo(Vector2 NewPosition) // Sets the entity's position to that Vector2 within the boundaries
    {
        destination = new Vector2( // clamp position in between boundaries
            Math.Clamp(NewPosition.x, -_settings.Boundaries.x, _settings.Boundaries.x),
            Math.Clamp(NewPosition.y, -_settings.Boundaries.y, _settings.Boundaries.y)
        );

        NewDestination();
    }
    public void MoveBy(Vector2 AddPosition) // Adds the Vector2 to the entity's position
    {
        MoveTo(destination + AddPosition);
    }
    public void MoveRandom(Vector2 BasePosition, float Magnitude)
    {
        Vector2 NewPosition = new Vector2(
            BasePosition.x + UnityEngine.Random.Range(-Magnitude, Magnitude),
            BasePosition.y + UnityEngine.Random.Range(-Magnitude, Magnitude)
        );

        MoveTo(NewPosition);
    }
    public void MoveExit(Vector2 NewPosition) // Sets the entity's position to that Vector2 past boundaries
    {
        destination = NewPosition;

        NewDestination();
    }
    
    public void SetPosition(Vector2 NewPosition) // Sets the entity's position to that Vector2 without taking into account SPD
    {
        position = NewPosition;
        transform.position = position;
    }
    private void unLook()
    {
        target_angle = null;
        target_destination = null;
        target_lock = null;
    }
    public void Look()
    {
        unLook();
        RefreshAngle(defaultAngle());
    }
    public void Look(Vector2 Position)
    {
        unLook();
        target_destination = Position;
    }
    public void Look(float Angle)
    {
        unLook();
        RefreshAngle(Angle + defaultAngle());
    }
    public void Look(Vector2 Position, float Angle)
    {
        unLook();
        target_angle = Angle;
        target_destination = Position;
    }
    public void Look(Transform Lock)
    {
        unLook();
        target_lock = Lock;
    }
    public void Look(Transform Lock, float Angle)
    {
        unLook();
        target_angle = Angle;
        target_lock = Lock;
    }

    /* Entity Functions */

    // Returns the default unchanged angle of the entity
    private float defaultAngle()
    {
        return transform.CompareTag("Player") ? 0 : 180;
    }

    // Player functions
    public Ability AddAbility(string component)
    {
        var type = Type.GetType(component);
        Ability ability = (Ability)gameObject.AddComponent(type);
        return ability;
    }
    public Ability AddAbility(string component, string keybind)
    {
        Ability ability = AddAbility(component);
        ability.input.AddBinding($"<Keyboard>/{keybind.ToLower()}");
        return ability;
    }

    // Stat functions
    public void Die(Entity? Caster) // Called when the entity is dead
    {
        if (Caster != null && Caster.CompareTag("Player")) // Add score when the entity is killed by player
        {
            Game.AddScore(score);
        }

        if (HealthBar != null)
        {
            HealthBar.gameObject.SetActive(false);
        }

        // kill tweens
        Destroy(gameObject);
    }
    public float Damage(float dmg, Entity? Caster) // Called when the entity is damaged by another, returns excess
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

        if (hp <= 0)
        {
            Die(Caster);
        }

        return Mathf.Clamp(dmg - lastHP + hp, 0, dmg);
    }
    public void Heal(float heal, Entity? Caster) // Called when the entity is damaged by another
    {
        if (hp <= 0) { return; } // Return if the entity is dead

        hp = Mathf.Clamp(hp + heal, 0, Default["HP"]);

        if (HealthBar != null)
        {
            Game.DisplayHP(HealthBar, hp, Default["HP"]);
        }
    }
    // Change the stat of the entity by float, return the function to reset the stat
    // Works by creating a new GUID index for the Offset Stat
    // Removes the index once reset
    public Action Stat(string stat, float change)
    {
        Guid index = Guid.NewGuid();
        stat = stat.ToUpper();
        Offsets[stat][index] = change; // my life

        return () => { Offsets[stat].Remove(index); };
    }
    // Change the stat of the entity for a limited time
    public void Stat(string stat, float change, float duration)
    {
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
        // --> so janky i hate thisv  i hate DICTIONARIES
        Coroutine reset(float? custom)
        {
            float time = (float)(custom != null ? custom : elapsed);

            add.Kill(); // --> I FOUND OUT NOT HAVING THIS WAS THE REASON WHY IT WONT SLOW BACK DOWN AAAAAAAAAAAAAAAAAAAAAAAAAAAA
            // --> UNITY SUCKSSSSS

            Tween undo = DOTween.To(
                () => Offsets[stat][index],
                x => {
                    Debug.Log(index);
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
    public void SetInvulnerable(float duration)
    {
        invulnerable = MathF.Max(invulnerable, duration);
    }
    public void SetInvulnerable(bool toggle)
    {
        invulnerable = toggle ? Mathf.Infinity : 0;
    }
    public void DisplayBar(Transform bar) {
        HealthBar = bar;
        Game.DisplayHP(bar, hp, Default["HP"]);
    }

    // Shoot Functions
    public Projectile Shoot(GameObject projectile, float spd, float angle) // Creates a projectile that moves at an angle
    {
        angle += Angle;
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, angle);
        Component.Caster = this;
        return Component;
    }
    public Projectile Shoot(GameObject projectile, float spd, Vector2 destination) // Creates a projectile that moves towards Vector2 direction
    {
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, destination);
        Component.Caster = this;
        return Component;
    }
    public Projectile Shoot(GameObject projectile, float spd, Vector2 destination, float angle) // Creates a projectile that moves towards Vector2 direction in an angle
    {
        string target = transform.tag == "Player" ? "Enemy" : "Player";
        Projectile Component = Game.Shoot(projectile, position, target, spd, destination, angle);
        Component.Caster = this;
        return Component;
    }

    // Entity static functions
    public static Entity? getPlayer() // Gets the player entity in game
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return null; }

        return player.GetComponent<Entity>();
    }
}