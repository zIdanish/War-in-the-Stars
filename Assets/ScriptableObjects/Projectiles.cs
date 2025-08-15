using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores all projectiles (for gameManager)
/// </summary>

[CreateAssetMenu]
public class Projectiles : ScriptableObject
{
    public GameObject AbilityOrb;
    public GameObject Asteroid;
    public GameObject HealOrb;
    public GameObject Warning;
}
