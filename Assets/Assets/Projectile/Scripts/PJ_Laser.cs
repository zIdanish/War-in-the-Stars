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
    /*<------------------------------------->*/
    private Dictionary<Entity, bool> debounce = new Dictionary<Entity, bool>();
    private SpriteRenderer sprite = null!;
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
            yield return StartCoroutine(Game.Warn((float)WARN, transform));
        }

        // Laser
        if (DURATION > 0)
        {
            yield return StartCoroutine(Laser((float)DURATION));
        }
    }
    private IEnumerator Laser(float duration)
    {
        Laser Component = GetComponent<Laser>();
        Component.Begin(duration);
        float transition = Mathf.Min(.25f, duration / 2);

        yield return new WaitForSeconds(transition);

        GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(duration - transition);

        GetComponent<Collider2D>().enabled = false;
    }
}
