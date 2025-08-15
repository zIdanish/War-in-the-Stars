using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// AudioManager Core for playing songs and sounds
/// It's mostly static so any script can call it from its class (very convenient, how i wish i did this for GameManager)
/// </summary>
public class AudioManager : MonoBehaviour
{
    public AudioAssets library; // used to transfer audioclips ScriptableObject from the gameObject to the static class
    public static AudioManager self; // The gameObject this script is attached to, accessible from static
    public static AudioSource player; // AudioSource Player, not to be confused with the player
    public static AudioAssets asset; // All AudioClips accessed from a ScriptableObject
    private void Awake()
    {
        self = this;
        player = GetComponent<AudioSource>();
        asset = library;
    }
    // Same as StartCoroutine
    // --> Added cause StartCoroutine doesn't work in static :pensive:
    public Coroutine Call(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
    // Plays a song in the main AudioSource
    public static void PlaySong(AudioClip song)
    {
        if (player.clip == song) return;

        self.Call(_PlaySong(song));
    }
    // Tweening to make the previous song fade, then play the new one
    private static IEnumerator _PlaySong(AudioClip song)
    {
        var tween = DOTween.To(() => 1f, x => player.volume = x, 0f, 1);
        yield return new WaitForSeconds(1f);
        tween.Kill();

        player.clip = song;
        player.time = 0;
        player.volume = 1f;
        player.Play();
    }
    // Plays a temporary sound in a temporary AudioSource
    // AudioSource gets destroyed after sound is finished
    public static AudioSource PlaySound(AudioClip sound)
    {
        var source = player.AddComponent<AudioSource>();
        source.clip = sound;
        source.priority = 125; // arbitrary value
        source.Play();
        Destroy(source, sound.length);

        return source;
    }
    // with volume
    public static AudioSource PlaySound(AudioClip sound, float volume)
    {
        var source = PlaySound(sound);
        source.volume = volume;
        source.priority = Mathf.Clamp((int)(125 / volume), 0, 255); // arbitrary value
        return source;
    }
}
