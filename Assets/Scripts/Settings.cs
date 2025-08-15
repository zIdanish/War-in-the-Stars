using System.Drawing;
using UnityEngine;

// --> ScriptableObjects exist, thanks for teaching that after I've done most of the game...
// --> On the bright side, it feels like this is more convenient
public class _settings : MonoBehaviour
{
    /* Setting Configuration */
    public static float Sensitivity = 1.0f; // cursor sensitivity
    public static float BoundaryShear = 5.0f; // cursor offset from the boundaries
    public static float AspectRatio = 16f / 9f; // screen aspect ratio
    public static string ScoreDigits = "D9"; // number of digits the score has

    // Z Layers (sortingOrder)
    public static int zBgProjectile = 2;
    // leave zindex below Enemy for Boss Enemy
    public static int zEnemy = 4;
    public static int zPlayerProjectile = 5;
    // leave zindex below player, above zPlayerProjectile empty for cursor
    public static int zPlayer = 7;
    public static int zEnemyProjectile = 8;
    // leave zindex above zEnemyProjectile empty for warning projectiles
    public static int zFriendlyProjectile = 10;

    /* Keybinds */

    public static string[] AbilityKeybinds = { "q", "w", "e" }; // keybinds for abilities in string (will be formatted in scripts)

    /* Automatic Variables */
    public static float Height { get { return Camera.main.orthographicSize; } } // Screen Height from the camera size
    public static Vector2 Screen { get { return new Vector2(Height * AspectRatio, Height); } } // Full screen size using 16:9 aspect ratio
    public static Vector2 Boundaries { get { var size = Height - BoundaryShear; return new Vector2(size, size); } } // Boundary Size using 1:1 aspect ratio and adding the shears

    /* FX */

    public GameObject _ImpactPlayer;
    public GameObject _ImpactEnemy;
    public static GameObject ImpactPlayer;
    public static GameObject ImpactEnemy;

    // set the objects
    private void Awake()
    {
        ImpactPlayer = _ImpactPlayer;
        ImpactEnemy = _ImpactEnemy;
    }

    /* Debug */
    public static bool debug_placeholder = true; // --> unused, lazy to remove these three lines
}