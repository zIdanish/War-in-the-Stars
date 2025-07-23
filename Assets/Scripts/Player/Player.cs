using System;
using UnityEngine;
using UnityEngine.InputSystem;

//# Player: Translating player inputs into Entity Functions
public class PlayerInputs : MonoBehaviour
{
    /* Init Variables */
    private Cursor Cursor;
    private Entity Entity;
    private void Awake() // Setup objects and components
    {
        Cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        Entity = this.GetComponent<Entity>();
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
            Entity.MoveBy(Cursor.Delta);
        }
    }
}