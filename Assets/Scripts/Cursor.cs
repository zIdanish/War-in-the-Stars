using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#nullable enable

/// <summary>
/// Custom cursor class
/// Contains inputs that affect the cursor such as Move, Click, Shift
/// Attach this to the cursor
/// </summary>
public class Cursor : MonoBehaviour
{
    /*<----------------Stats---------------->*/
    public bool Bounded { get; private set; } = false; // set to true if the player entity is bounded to the cursor
    public Vector2 Position { get { return position; } } // Position of the cursor
    public bool Clicked { get { return Click.IsPressed(); } } // Boolean for cursor click
    public bool Shifted { get { return Shift.IsPressed(); } } // Boolean for precise cursor movement (LShift)
    public Vector2 Delta { get { return delta; } } // Cursor Movement delta
    public Transform? Follow; // Set the transform to the player entity
    /*<-----------------Misc---------------->*/
    private SpriteRenderer Sprite = null!; // Cursor sprite
    private GameManager Game = null!; // GameManager

    // InputAction objects to detect player input for movement, clicking and shifting
    [SerializeField] private InputAction Move = null!;
    [SerializeField] private InputAction Click = null!;
    [SerializeField] private InputAction Shift = null!;

    // Variables for cursor position and delta
    private Vector2 position = new Vector2(0, 0);
    private Vector2 delta = new Vector2(0, 0);

    /*<------------Init Functions----------->*/
    // Bind functions to the cursor
    private void OnEnable()
    {
        Move.Enable(); Click.Enable(); Shift.Enable();
        Click.performed += OnClick;
        Click.canceled += OnRelease;
    }
    private void OnDisable()
    {
        Move.Disable(); Click.Disable(); Shift.Disable();
        Click.performed -= OnClick;
        Click.canceled += OnRelease;
    }
    private void Update()
    {
        MoveCursor();
    }
    private void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    /*<------------Update Functions----------->*/

    // Checks if the cursor has moved and saves the delta for this frame
    // Also puts the cursor within boundaries if it's moving the player entity
    public void MoveCursor()
    {
        delta = Move.ReadValue<Vector2>() * _settings.Sensitivity / (Shifted ? 16 : 8); // Gets the cursor delta translated into unity Vector2
        var Boundaries = Bounded ? _settings.Boundaries : _settings.Screen;

        if (delta.x != 0 || delta.y != 0) // Check if the mouse moved
        {
            // Add the delta to the current position, and clamp the position within boundaries
            position += delta;
            position = new Vector2(
                Math.Clamp(position.x, -Boundaries.x, Boundaries.x),
                Math.Clamp(position.y, -Boundaries.y, Boundaries.y)
            );

            // set the transform's position to current cursor position
            transform.position = position;
        }
    }

    /*<------------Private Functions----------->*/
    // Called when the player clicks
    // Checks if the player entity transform exists, is within the boundaries and the game isn't paused
    // And if yes, set the cursor position to the player position
    // Additionally set the cursor ZIndex to below the player so you can see the hitbox
    private void OnClick(InputAction.CallbackContext context)
    {
        if (
            Follow == null || 
            Game.Paused || 
            MathF.Abs(position.x) > _settings.Boundaries.x || 
            MathF.Abs(position.y) > _settings.Boundaries.y) 
        {
            return; 
        }
        position = (Vector2)Follow.position;
        transform.position = position;

        // Bound
        Bounded = true;
        Sprite.sortingOrder = _settings.zPlayer - 1;
    }
    
    // Called when the player releases the click
    // Sets the cursor ZIndex to the top and disables the variable Bounded to the player
    private void OnRelease(InputAction.CallbackContext context)
    {
        // Unbound
        Bounded = false;
        Sprite.sortingOrder = 50;
    }
}