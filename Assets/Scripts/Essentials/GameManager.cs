using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* Init Variables */
    private GameObject Entities;

    private void Awake()
    {
        HideCursor();
    }

    /* Init Functions */
    public void Init()
    {
        Entities = GameObject.FindGameObjectWithTag("Entities");
    }
    private void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    /* Game Functions */

    // Spawns an enemy at the position, and moves towards target position
    public Entity SpawnEnemy(GameObject enemy, Vector2 position, Vector2 targetPosition)
    {
        GameObject Enemy = Instantiate(enemy);
        Entity Component = Enemy.GetComponent<Entity>();
        Component.MoveTo(targetPosition);
        Enemy.transform.position = position;
        Enemy.transform.SetParent(Entities.transform);
        return Component;
    }
}