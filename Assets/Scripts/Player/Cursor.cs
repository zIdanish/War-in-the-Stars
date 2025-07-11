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

        [SerializeField] private InputAction Move;
        [SerializeField] private InputAction Click;
        [SerializeField] private InputAction Shift;

        private Vector2? locked;
        private Vector2 position = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);

        private void OnEnable()
        {
            Move.Enable(); Click.Enable(); Shift.Enable();
        }
        private void OnDisable()
        {
            Move.Disable(); Click.Disable(); Shift.Disable();
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
            delta = Move.ReadValue<Vector2>() * _settings.Sensitivity / 8;
            if (delta.x != 0 || delta.y != 0) {
                position += delta;
                position = new Vector2(Math.Clamp(position.x, -_settings.Boundaries.x, _settings.Boundaries.x), Math.Clamp(position.y, -_settings.Boundaries.y, _settings.Boundaries.y)); ;
                transform.position = position;
            }
        }

    }
}