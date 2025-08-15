using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores all projectiles (for gameManager)
/// </summary>

[CreateAssetMenu]
public class LevelInfo : ScriptableObject
{
    public string[] Passives = { };
    public string[] Abilities1 = { };
    public string[] Abilities2 = { };
    public string[] Abilities3 = { };
}
