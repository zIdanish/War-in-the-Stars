using System;
using UnityEngine;
using UnityEngine.InputSystem;

//# Player: Translating player inputs into Entity Functions
public class PlayerInputs : MonoBehaviour
{
    [SerializeField] public InputAction Ability1;

    /* Init Variables */
    private Cursor Cursor;
    private Entity Entity;
    private void Awake() // Setup objects and components
    {
        Cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        Entity = gameObject.GetComponent<Entity>();
    }
    private void OnEnable()
    {
        Ability1.Enable(); // Enable inputs when this component is disabled
    }
    private void OnDisable()
    {
        Ability1.Disable(); // Disable inputs when this component is disabled
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