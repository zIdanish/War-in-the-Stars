using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
#nullable enable

/// <summary>
/// Player Manager
/// Uses player inputs to control the Player Entity
/// Also manages TP (Tension Points), a player only stat in the entity class
/// </summary>
public class Player : MonoBehaviour
{
    /* Abilities */
    public Ability?[] Abilities = { null, null, null, null };
    /* Init Variables */
    private InputAction Pause = new InputAction();
    private Cursor Cursor = null!;
    private Entity Entity = null!;
    private GameManager Game = null!;

    private void Awake() // Setup objects and components
    {
        // --> REMEBMBER FIX THIS
        // --> REMEBMBER FIX THIS
        // --> excuse me what am i supposed to fix I DIDNT REMIND MYSELF WHAT IM SUPPOSED TO FIX

        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        Pause.AddBinding("<Keyboard>/escape");
        Pause.performed += ctx => Game.Pause();

        Cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        Entity = gameObject.GetComponent<Entity>();

        // init cursor follow
        Cursor.Follow = transform;

        // set sprite layer
        GetComponent<SpriteRenderer>().sortingOrder = _settings.zPlayer;
    }

    /* Collision Functions */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the tag is the same as the target
        if (collision.gameObject.tag != "Enemy") { return; }

        // Check if the collision has an entity component
        Entity? enemy = collision.gameObject.GetComponent<Entity>();
        if (enemy == null) { return; }

        Entity.Damage(enemy.DMG, enemy);
    }
    private void OnEnable()
    {
        Pause.Enable();
    }
    private void OnDisable()
    {
        Pause.Disable();
    }
    private void Update()
    {
        MoveEntity();
    }

    /* Player Input Controls */
    private void MoveEntity()
    {
        if (Cursor.Bounded) // Move entity by cursor delta if the player held click this frame
        {
            Entity.MoveTo(Cursor.Position);
        }
    }
    private void OnDestroy()
    {
        Game.End();
    }
}