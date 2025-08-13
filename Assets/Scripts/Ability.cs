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
    protected Image? background; // cooldown background image
    protected TextMeshProUGUI? text; // cooldown text display
    /*<----------------Stats---------------->*/
    public InputAction input = new InputAction(); // keybinds
    /*<-----------------Misc---------------->*/
    protected Transform entities = null!;
    protected Entity entity = null!;
    protected Coroutine timeline = null!;

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

    // Called outside the script
    public void Init()
    {
        // init variables
        entities = GameObject.FindGameObjectWithTag("Entities").transform;
        entity = GetComponent<Entity>();
        timeline = StartCoroutine(Timeline());

        // init icon if not null
        if (icon == null) { return; }
        background = icon.Find("Cooldown").GetComponent<Image>();
        text = icon.Find("Display").GetComponent<TextMeshProUGUI>();
        background.enabled = false; text.enabled = false;
    }

    /*<------------Timeline----------->*/

    // Ability timeline
    // --> This is just a placeholder, replace it with the actual bullet pattern
    public virtual IEnumerator Timeline()
    {
        Debug.Log($"{this.GetType().FullName} does not have a timeline!!!!!!!!!!!!!!!!!!");
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
    public Entity? getClosest()
    {
        Entity? closest = null;
        float? distance = null;

        foreach (var enemy in entities.GetComponentsInChildren<Entity>())
        {
            if (enemy.CompareTag(transform.tag)) { continue; }

            var dist = (enemy.Position - entity.Position).magnitude;

            if (distance!=null && distance > dist) { continue; }
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
