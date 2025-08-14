using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
#nullable enable

/// <summary>
/// Ability core [FOR PLAYER ONLY]
/// This component creates a bullet pattern for the player that is either passive or active
/// Abilities are set by the GameManager, when the level begins
/// To create an active bullet pattern, set the icon and input variable in GameManager
/// Refer to other levels to figure out how to do it ig
/// </summary>
public class Ability : MonoBehaviour
{
    /*<----------------UI Variables---------------->*/
    //--> my bad for unhelpful naming scheme
    public Transform? icon; // ability icon UI (no need to set if the ability is automatic)

    // below are set automatically
    protected Image? background; // ui cooldown background image
    protected TextMeshProUGUI? text; // ui cooldown text display
    /*<----------------Stats---------------->*/
    public InputAction input = new InputAction();
    /*<-----------------Misc---------------->*/
    protected Transform entities = null!; // transform which stores all entities in its children
    protected Entity entity = null!; // main entity this ability is tied to
    protected Coroutine timeline = null!; // the ability timeline
    protected GameManager Game = null!;

    /*<------------Init Functions----------->*/

    // Refresh the input action to be activated and deactivated in tandem with the script
    private void OnEnable()
    {
        if (input != null) { input.Enable(); }
    }
    private void OnDisable()
    {
        if (input != null) { input.Disable(); };
    }
    protected virtual void Awake()
    {
        // init variables
        entities = GameObject.FindGameObjectWithTag("Entities").transform;
        entity = GetComponent<Entity>();
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    // Called outside the script to link the projectile to the player
    // And start the timeline in a coroutine
    public virtual void Link()
    {
        // init icon if not null
        if (icon != null)
        {
            background = icon.Find("Cooldown").GetComponent<Image>();
            text = icon.Find("Display").GetComponent<TextMeshProUGUI>();
            background.enabled = false; text.enabled = false;
        }

        timeline = StartCoroutine(Timeline());
    }

    /*<------------Essentials----------->*/

    // Ability timeline
    // --> This is just a placeholder, replace it with the actual bullet pattern
    public virtual IEnumerator Timeline()
    {
        Debug.LogWarning($"{this.GetType().FullName} does not have a timeline!!!!!!!!!!!!!!!!!!");
        yield return null;
    }

    // Yields until the ability button has been pressed
    // Used only for active abilities, passive abilities can just ignore
    protected IEnumerator AbilityPressed()
    {
        if (input == null) { yield break; }
        while (!input.IsPressed()) { yield return null; }
    }

    // Causes the Ability to go on a Cooldown
    // Called at the end of the ability, passive abilities ignore this
    protected IEnumerator AbilityCooldown(float cooldown)
    {
        float timer = cooldown;
        if (background == null || text == null) { yield break; }

        background.enabled = true;
        text.enabled = true;

        while (timer > 0)
        {
            text.text = timer.ToString("F1") + "s";
            timer -= Time.deltaTime;
            yield return null;
        }

        background.enabled = false;
        text.enabled = false;
    }

    /*<------------Ability Functions----------->*/
    // Compares the distance between each enemy entity
    // Returns the closest enemy to the player
    public Entity? getClosest(Vector2? position)
    {
        if (position == null) position = entity.Position;

        Entity? closest = null;
        float? distance = null;

        foreach (var enemy in entities.GetComponentsInChildren<Entity>())
        {
            if (enemy.CompareTag(transform.tag)) { continue; }

            var dist = (enemy.Position - (Vector2)position).magnitude;

            if (distance!=null && distance < dist) { continue; }
            closest = enemy;
            distance = dist;
        }

        return closest;
    }

    // Returns true or false depending on the chance value
    public bool Random(float chance)
    {
        return UnityEngine.Random.Range(0f, 100f) < chance;
    }
}
