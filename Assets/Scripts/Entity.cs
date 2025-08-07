using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using Color = UnityEngine.Color;
#nullable enable

//# Entity: Entity behaviour (Stats Management, Movement, Projectile Creation etc. etc.)
public class Entity : MonoBehaviour
{
    /* Init Variables */

    /*<-----------------Stats---------------->*/
    [SerializeField] private float hp = 100;
    [SerializeField] private float spd = 65;
    [SerializeField] private float dmg = 10;
    [SerializeField] private float inv = 0; // invulnerability window
    [SerializeField] private int score = 0;

    // Init default stats
    public bool IsPlayer = false;
    private Dictionary<string, float> Default = new Dictionary<string, float> { };
    private float invulnerable = 0; // 0 means vulnerable, anything above is invulnerable (in seconds)

    // Entity Vector2 info, destination is the target position the entity is moving towards.
    private SpriteRenderer sprite = null!;
    private Vector2 destination;
    private Vector2 position;
    /*<-------------------------------------->*/
    private GameManager Game = null!;
    private void Awake()
    {
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sprite = GetComponent<SpriteRenderer>();

        // set default stats
        Default["HP"] = hp;
        Default["SPD"] = spd;

        // set default position
        position = (Vector2)transform.position;
        if (destination == null)
        {
            destination = position;
        }
    }
    private void Update()
    {
        UpdatePosition();
        RefreshStats();
        InvulnerableSprite();
    }

    /* Public Variables */
    public float HP { get { return hp; } }
    public float SPD { get { return spd; } }
    public float DMG { get { return dmg; } }
    public Vector2 Position { get { return position; } }
    public Vector2 Destination { get { return destination; } }
    public bool Moving { get { return Mathf.Abs(position.x-destination.x)>.1f || Mathf.Abs(position.y - destination.y)> .1f; } }
    public bool Invulnerable { get { return invulnerable > 0; } }

    /* Update Functions */
    private void UpdatePosition() // Updates entity position
    {
        var delta = Time.deltaTime;
        if (position != destination) // Moves the entity position using Vector2 and lerping towards the target position
        {
            var diff = (destination - position).magnitude;
            var magnitude = Mathf.Clamp(spd * 5 * delta / diff, 0, Mathf.Min(1, diff));
            position = Vector2.Lerp(position, destination, magnitude);
        }

        transform.position = position; // Set the object position to the new position
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

    /* Movement Functions */
    public void MoveTo(Vector2 NewPosition) // Sets the entity's position to that Vector2 within the boundaries
    {
        destination = new Vector2( // clamp position in between boundaries
            Math.Clamp(NewPosition.x, -_settings.Boundaries.x, _settings.Boundaries.x),
            Math.Clamp(NewPosition.y, -_settings.Boundaries.y, _settings.Boundaries.y)
        );
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
    }
    
    public void SetPosition(Vector2 NewPosition) // Sets the entity's position to that Vector2 without taking into account SPD
    {
        position = NewPosition;
    }

    /* Entity Functions */

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
        if (Caster != null && Caster.tag == "Player") // Add score when the entity is killed by player
        {
            Game.AddScore(score);
        }
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

        if (IsPlayer)
        {
            Game.DisplayHP(hp, Default["HP"]);
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

        if (IsPlayer)
        {
            Game.DisplayHP(hp, Default["HP"]);
        }
    }

    // Shoot Functions
    public Projectile Shoot(GameObject projectile, float spd, float angle) // Creates a projectile that moves at an angle
    {
        angle += transform.eulerAngles.z;
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