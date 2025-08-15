using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores all projectiles (for gameManager)
/// </summary>

[CreateAssetMenu]
public class AudioAssets : ScriptableObject
{
    public AudioClip Boss;
    public AudioClip Game;
    public AudioClip End;

    public AudioClip SND_Ability;
    public AudioClip SND_Asteroid;
    public AudioClip SND_Damaged;
    public AudioClip SND_Death;
    public AudioClip SND_Error;
    public AudioClip SND_Explode;
    public AudioClip SND_Get;
    public AudioClip SND_Graze;
    public AudioClip SND_Heal;
    public AudioClip SND_Hurt;
    public AudioClip SND_Laser;
}
