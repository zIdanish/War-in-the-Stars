using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _settings : MonoBehaviour
{
    /* Setting Configuration */
    public static float Sensitivity = 1.0f; // cursor sensitivity
    public static float BoundaryShear = 5.0f; // cursor offset from the boundaries
        
    /* Automatic Variables */
    public static Vector2 Boundaries { get { float size = Camera.main.orthographicSize - BoundaryShear; return new Vector2(size, size); } }

    /* Debug */
    public static bool debug_placeholder = true;
}