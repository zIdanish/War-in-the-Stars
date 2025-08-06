using System;
using UnityEngine;
using UnityEngine.InputSystem;

//# Player: Translating player inputs into Entity Functions
public class Player : MonoBehaviour
{
    /* Init Variables */
    private Cursor Cursor;
    private Entity Entity;
    private void Awake() // Setup objects and components
    {
        Cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        Entity = gameObject.GetComponent<Entity>();
        Entity.IsPlayer = true;

        // init cursor follow
        Cursor.follow = transform;
    }
    private void Update()
    {
        MoveEntity();
    }

    /* Player Input Controls */
    private void MoveEntity()
    {
        if (Cursor.Clicked) // Move entity by cursor delta if the player held click this frame
        {
            Entity.MoveTo(Cursor.Position);
        }
    }
}