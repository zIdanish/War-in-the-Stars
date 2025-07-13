using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game
{
    //# Cursor: Cursor animations and control
    public class Cursor : MonoBehaviour
    {
        /* Init Variables */

        // InputAction objects to detect player input for movement, clicking and shifting
        [SerializeField] private InputAction Move;
        [SerializeField] private InputAction Click;
        [SerializeField] private InputAction Shift;

        // Variables for cursor position and delta
        private Vector2 position = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);

        private void OnEnable()
        {
            Move.Enable(); Click.Enable(); Shift.Enable(); // Enable inputs when this component is disabled
        }
        private void OnDisable()
        {
            Move.Disable(); Click.Disable(); Shift.Disable(); // Disable inputs when this component is disabled
        }
        private void Update()
        {
            MoveCursor();
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
            if (delta.x != 0 || delta.y != 0) {
                // If there was movement, add the delta to the current position, and clamp the position within boundaries
                position += delta;
                position = new Vector2(
                    Math.Clamp(position.x, -_settings.Boundaries.x, _settings.Boundaries.x), 
                    Math.Clamp(position.y, -_settings.Boundaries.y, _settings.Boundaries.y)
                );

                // set the transform's position to current cursor position
                transform.position = position;
            }
        }

    }
}