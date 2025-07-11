using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    //# Player: Translating player inputs into Entity Functions
    public class PlayerInputs : MonoBehaviour
    {
        /* Init Variables */
        private Cursor Cursor;
        private Entity Entity;
        private void Awake()
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
            if (Cursor.Clicked)
            {
                Entity.MoveBy(Cursor.Delta);
            }
        }
    }
}