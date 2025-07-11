using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    //# Entity: Entity behaviour (Stats Management, Movement, Projectile Creation etc. etc.)
    public class Entity : MonoBehaviour
    {
        /* Init Variables */
        [SerializeField]
        private float hp = 100;
        private float spd = 65;

        private Vector2 destination = new Vector2(0, 0);
        private Vector2 position = new Vector2(0, 0);

        private void Update()
        {
            UpdatePosition();
        }

        /* Public Variables */
        public float HP { get { return hp; } }
        public float SPD { get { return spd; } }
        public Vector2 Position { get { return position; } }
        public Vector2 Destination { get { return destination; } }

        /* Movement Functions */
        public void MoveTo(Vector2 NewPosition)
        {
            // clamp position in between boundaries
            destination = new Vector2(
                Math.Clamp(NewPosition.x, -_settings.Boundaries.x, _settings.Boundaries.x), 
                Math.Clamp(NewPosition.y, -_settings.Boundaries.y, _settings.Boundaries.y)
            );
        }
        public void MoveBy(Vector2 AddPosition)
        {
            MoveTo(destination + AddPosition);
        }

        /* Update Functions */
        private void UpdatePosition()
        {
            // Moves the entity position using Vector2
            var delta = Time.deltaTime;
            if (position != destination)
            {
                var diff = (destination - position).magnitude;
                var magnitude = Mathf.Clamp(spd * 5 * delta / diff, 0, Mathf.Min(1, diff));
                position = Vector2.Lerp(position, destination, magnitude);
            }

            // Set the object position to the new position
            transform.position = position;
        }
    }
}