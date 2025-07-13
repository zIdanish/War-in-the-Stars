using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
#nullable enable

namespace Game
{
    //# Entity: Entity behaviour (Stats Management, Movement, Projectile Creation etc. etc.)
    public class Entity : MonoBehaviour
    {
        /* Init Variables */

        /*<-----------------Stats---------------->*/
        [SerializeField] private float hp = 100;
        [SerializeField] private float spd = 65;
        public float placeholder_elapsed = 0;
        public GameObject placeholder_projectile;
        /*<-------------------------------------->*/

        // Init default stats
        Dictionary<string, float> Default = new Dictionary<string, float> { };

        // Entity Vector2 info, destination is the target position the entity is moving towards.
        private Transform projectile_folder;
        private Vector2 destination;
        private Vector2 position;
        private void Start()
        {
            projectile_folder = GameObject.FindGameObjectWithTag("Projectiles").transform;

            // set default stats
            Default["HP"] = hp;
            Default["SPD"] = spd;

            // set starting position
            position = (Vector2)transform.position;
            destination = position;
        }
        private void Update()
        {
            placeholderShoot();
            UpdatePosition();
        }

        /* Public Variables */
        public float HP { get { return hp; } }
        public float SPD { get { return spd; } }
        public Vector2 Position { get { return position; } }
        public Vector2 Destination { get { return destination; } }

        /* Movement Functions */
        public void MoveTo(Vector2 NewPosition) // Sets the entity's position to that Vector2
        {
            destination = new Vector2( // clamp position in between boundaries
                Math.Clamp(NewPosition.x, -_settings.Boundaries.x, _settings.Boundaries.x), 
                Math.Clamp(NewPosition.y, -_settings.Boundaries.y, _settings.Boundaries.y)
            );
        }
        public void MoveBy(Vector2 AddPosition) // Adds the Vector2 to the entity's position
        {
            MoveTo(destination + AddPosition);
        }

        /* Update Functions */
        private void UpdatePosition()
        {
            var delta = Time.deltaTime;
            if (position != destination) // Moves the entity position using Vector2 and lerping towards the target position
            {
                var diff = (destination - position).magnitude;
                var magnitude = Mathf.Clamp(spd * 5 * delta / diff, 0, Mathf.Min(1, diff));
                position = Vector2.Lerp(position, destination, magnitude);
            }

            transform.position = position; // Set the object position to the new position
        }

        /* Entity Functions */
        public void Damage(float dmg, Entity Caster)
        {
            hp = Mathf.Clamp(hp - dmg, 0, Default["HP"]);
            Debug.Log(hp);
            if (hp <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        public Entity? getPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) { return null; }

            return player.GetComponent<Entity>();
        }
        public Projectile Shoot(GameObject projectile, float dmg, float spd, float angle) // Creates a projectile that moves at an angle
        {
            float radians = (angle + transform.eulerAngles.z) * Mathf.Deg2Rad;
            Vector2 destination = position + new Vector2( // Calculate the destination vector (goes offscreen)
                Mathf.Sin(radians),
                Mathf.Cos(radians)
            );

            Projectile Component = Shoot(projectile, dmg, spd, destination);
            return Component;
        }
        public Projectile Shoot(GameObject projectile, float dmg, float spd, Vector2 destination) // Creates a projectile that moves towards Vector2 direction
        {
            GameObject Projectile = Instantiate(projectile);
            Projectile Component = Projectile.GetComponent<Projectile>();
            Component.TARGET = transform.tag == "Player" ? "Enemy" : "Player";
            Component.DMG = dmg;
            Component.SPD = spd;
            Component.Caster = this;
            Component.Position = position;
            Component.MoveTo(destination);
            Projectile.transform.parent = projectile_folder;
            return Component;
        }
        public Projectile Shoot(GameObject projectile, float dmg, float spd, Vector2 destination, float angle) // Creates a projectile that moves towards Vector2 direction in an angle
        {
            float radians = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            Vector2 dif = destination - position;
            Vector2 direction = new Vector2( // calculate the new direction of the vector
                dif.x * cos - dif.y * sin,
                dif.x * sin + dif.y * cos
            );

            destination = position + direction; // set the new destination

            Projectile Component = Shoot(projectile, dmg, spd, destination);
            return Component;
        }

        /* Placeholder Functions */
        public void placeholderShoot()
        {
            float cooldown = transform.tag == "Player" ? 0.1f : 0.5f;
            placeholder_elapsed += Time.deltaTime;
            if (placeholder_elapsed > cooldown)
            {
                placeholder_elapsed -= cooldown;

                if (transform.tag == "Player")
                {
                    for (int i = -30; i <= 30; i += 30)
                    {
                        Shoot(placeholder_projectile, 10, 75, i);
                    }
                } else
                {
                    var player = getPlayer();
                    if (player == null) { return; }
                    Shoot(placeholder_projectile, 20, 10, player.Position);
                }
            }
        }
    }
}