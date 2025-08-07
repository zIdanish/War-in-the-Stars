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

public class Ability : MonoBehaviour
{
    public Transform? icon;
    protected Image? background;
    protected TextMeshProUGUI? text;
    public InputAction input = new InputAction();
    protected Entity entity = null!;
    protected Coroutine timeline = null!;
    private void OnEnable()
    {
        if (input != null) { input.Enable(); } // Enable inputs when this component is disabled
    }
    private void OnDisable()
    {
        if (input != null) { input.Disable(); }; // Disable inputs when this component is disabled
    }
    public void Init()
    {
        entity = gameObject.GetComponent<Entity>();
        timeline = StartCoroutine(Timeline());

        // icon stuff
        if (icon == null) { return; }
        background = icon.Find("Cooldown").GetComponent<Image>();
        text = icon.Find("Display").GetComponent<TextMeshProUGUI>();
        background.enabled = false; text.enabled = false;
    }
    public virtual IEnumerator Timeline()
    {
        Debug.Log($"{this.GetType().FullName} does not have a timeline!!!!!!!!!!!!!!!!!!");
        yield return null;
    }
    protected IEnumerator AbilityPressed()
    {
        if (input == null) { yield break; }
        while (!input.IsPressed()) { yield return null; }
    }
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
}
