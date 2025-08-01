using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Ability : MonoBehaviour
{
    protected Transform ui = null!;
    protected Transform ability1 = null!;
    protected PlayerInputs input = null!;
    protected Entity entity = null!;
    protected Coroutine timeline = null!;
    public void Init()
    {
        ui = GameObject.FindGameObjectWithTag("UI").transform;
        ability1 = ui.Find("Ability1");
        input = gameObject.GetComponent<PlayerInputs>();
        entity = gameObject.GetComponent<Entity>();
        timeline = StartCoroutine(Timeline());
    }
    public virtual IEnumerator Timeline()
    {
        Debug.Log($"{this.GetType().FullName} does not have a timeline!!!!!!!!!!!!!!!!!!");
        yield return null;
    }
    protected IEnumerator AbilityPressed(InputAction input)
    {
        while (!input.IsPressed()) { yield return null; }
    }
    protected IEnumerator AbilityCooldown(float cooldown, Transform ui)
    {
        Image background = ui.Find("Cooldown").GetComponent<Image>();
        TextMeshPro text = ui.Find("Text").GetComponent<TextMeshPro>();
        float timer = cooldown;

        background.SetEnabled(true);
        text.enabled = true;

        while (timer > 0)
        {
            text.text = cooldown.ToString("F1") + "s";
            timer -= Time.deltaTime;
            yield return null;
        }

        background.SetEnabled(false);
        text.enabled = false;
    }
}
