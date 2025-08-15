using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#nullable enable

/// <summary>
/// Custom cursor class
/// Contains inputs that affect the cursor such as Move, Click, Shift
/// Attach this to the cursor
/// Custom cursor is used since it's easier to make the entity move relative to the cursor that way
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
    private RectTransform PauseButton = null!;
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
        HideDefaultCursor();
    }
    private void OnDisable()
    {
        Move.Disable(); Click.Disable(); Shift.Disable();
        Click.performed -= OnClick;
        Click.canceled -= OnRelease;
        ShowDefaultCursor();
    }
    private void Update()
    {
        MoveCursor();
    }
    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        PauseButton = GameObject.FindGameObjectWithTag("PauseButton").GetComponent<RectTransform>();
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

    /*<------------Cursor Functions----------->*/
    // Called when the player clicks
    private void OnClick(InputAction.CallbackContext context)
    {
        BindToEntity();
        CheckPause();
    }

    // Checks if the player entity transform exists, is within the boundaries and the game isn't paused
    // And if yes, set the cursor position to the player position
    // Additionally set the cursor ZIndex to below the player so you can see the hitbox
    private void BindToEntity()
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
    // Checks if the cursor is clicking on the pause button
    // Breaks the function if not within range of the pause button
    // Cause the player will be using the custom cursor instead of the player cursor for the pause menu
    private void CheckPause()
    {
        var dist = ((Vector2)PauseButton.position - position).magnitude;
        if (dist > 5) { return; }
        GameButtons.Pause();
    }
    
    // Called when the player releases the click
    // Sets the cursor ZIndex to the top and disables the variable Bounded to the player
    private void OnRelease(InputAction.CallbackContext context)
    {
        // Unbound
        Bounded = false;
        Sprite.sortingOrder = 999;
    }
    // Hide the default windows cursor
    // So it uses the custom cursor in game
    private void HideDefaultCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        Sprite.enabled = true; // Show Custom Cursor

        // Clamp the cursor within game boundaries
        position = new Vector2(
            Math.Clamp(position.x, -_settings.Boundaries.x, _settings.Boundaries.x),
            Math.Clamp(position.y, -_settings.Boundaries.y, _settings.Boundaries.y)
        );
    }

    // Show back the default windows cursor for menus and such
    private void ShowDefaultCursor()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        Sprite.enabled = false;// Hide Custom Cursor
    }
}