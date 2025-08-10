using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class _settings : MonoBehaviour
{
    /* Setting Configuration */
    public static float Sensitivity = 1.0f; // cursor sensitivity
    public static float BoundaryShear = 5.0f; // cursor offset from the boundaries

    // Z Layers
    // leave zindex below Enemy for Boss Enemy
    public static int zEnemy = 4;
    public static int zPlayerProjectile = 5;
    // leave zindex below player, above zPlayerProjectile empty for cursor
    public static int zPlayer = 7;
    public static int zEnemyProjectile = 8;

    /* Automatic Variables */
    public static Vector2 Screen { get { float y = Camera.main.orthographicSize; return new Vector2(y/9*16, y); } }
    public static Vector2 Boundaries { get { float size = Camera.main.orthographicSize - BoundaryShear; return new Vector2(size, size); } }

    /* Debug */
    public static bool debug_placeholder = true;
}