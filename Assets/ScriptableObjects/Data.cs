using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

/// <summary>
/// ScriptableObject that stores data info
/// </summary>

[CreateAssetMenu]
public class Data : ScriptableObject
{
    public int Score = 0;
    public int? HighScore;
    public float HP = 100;
    public float TP = 0;
    public string?[] Abilities = {null, null, null, null};
    public bool New = true;
}
