using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores all assets for player abilities
/// </summary>

[CreateAssetMenu]
public class AbilityAssets : ScriptableObject
{
    public GameObject Circle;
    public GameObject Bullet;
    public GameObject Laser;
    public GameObject Slash;
}
