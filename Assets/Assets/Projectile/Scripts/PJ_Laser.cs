using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#nullable enable

public class PJ_Laser : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float DMG = 10;
    public Transform? PIVOT; // Laser Anchor
    public Vector2? DISP; // Displacement from the anchor
    public float? WARN = 1f; // Warning laser
    public float DURATION = 1f; // Laser lifetime
    public float SIZE = 10f;
    /*<------------------------------------->*/
    private Dictionary<Entity, bool> debounce = new Dictionary<Entity, bool>();
    private SpriteRenderer sprite = null!;
    private GameObject? warn;
    protected override void Start()
    {
        // init projectile properties
        GetComponent<Collider2D>().enabled = false;
        DISABLE_DELETE = true;
        DISABLE_MOVE = true;
        SPD = 0; // default SPD to 0
        
        // init sprites
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = _settings.zBgProjectile;

        // start coroutine
        base.Start();
        StartCoroutine(Begin());
    }

    protected override void Update()
    {
        // add pivot position
        RefreshPosition();
        base.Update();
    }
    protected override void Destroyed()
    {
        base.Destroyed();
    }

    /* Projectile Functions */
    protected override void OnHit(Entity entity)
    {
        // Deals damage to the entity
        if (entity.Invulnerable || debounce.ContainsKey(entity)) { return; }
        debounce[entity] = true;
        entity.Damage(DMG, Caster);
    }
    public void RefreshPosition()
    {
        if (PIVOT != null)
        {
            var newPosition = (Vector2)PIVOT.position + Direction * 105.02f;
            if (DISP != null)
            {
                newPosition += (Vector2)DISP;
            }

            SetPosition(newPosition);
        }
    }
    private IEnumerator Begin()
    {
        // Warning laser
        if (WARN != null && WARN > 0)
        {
            yield return StartCoroutine(Warn((float)WARN));
        }

        // Laser
        if (DURATION > 0)
        {
            yield return StartCoroutine(Laser(DURATION));
        }
    }
    private IEnumerator Warn(float duration)
    {
        if (Caster != null && Caster.IsDestroyed()) { yield break; }
        warn = Game.Warn(duration, SIZE, transform);

        while (!warn.IsDestroyed()) { yield return null; }
    }
    private IEnumerator Laser(float duration)
    {
        if (Caster != null && Caster.IsDestroyed()) { yield break; }
        transform.localScale = new Vector3(SIZE, 10, SIZE);
        Laser Component = GetComponent<Laser>();
        Component.Begin(duration);

        float transition = Mathf.Min(.25f, duration / 2);

        yield return new WaitForSeconds(transition / 2);

        GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(duration - transition);

        GetComponent<Collider2D>().enabled = false;
    }
}
