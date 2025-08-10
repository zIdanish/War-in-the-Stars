using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#nullable enable

//# Cursor: Cursor animations and control
public class Cursor : MonoBehaviour
{
    /* Init Variables */

    private SpriteRenderer Sprite = null!;
    private GameManager Game = null!;
    public Transform? Follow;
    public bool Bounded { get; private set; } = false;

    // InputAction objects to detect player input for movement, clicking and shifting
    [SerializeField] private InputAction Move = null!;
    [SerializeField] private InputAction Click = null!;
    [SerializeField] private InputAction Shift = null!;

    // Variables for cursor position and delta
    private Vector2 position = new Vector2(0, 0);
    private Vector2 delta = new Vector2(0, 0);

    private void OnEnable()
    {
        Move.Enable(); Click.Enable(); Shift.Enable(); // Enable inputs when this component is disabled
        Click.performed += OnClick;
        Click.canceled += OnRelease;
    }
    private void OnDisable()
    {
        Move.Disable(); Click.Disable(); Shift.Disable(); // Disable inputs when this component is disabled
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

    /* Public Variables */

    public Vector2 Position { get { return position; } }
    public bool Clicked { get { return Click.IsPressed(); } }
    public bool Shifted { get { return Shift.IsPressed(); } }
    public Vector2 Delta { get { return delta; } }

    /* Player Input Controls */
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

    /* Private Functions */
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
    private void OnRelease(InputAction.CallbackContext context)
    {
        // Unbound
        Bounded = false;
        Sprite.sortingOrder = 50;
    }
}