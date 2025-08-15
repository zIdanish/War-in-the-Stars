using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
#nullable enable

public class PJ_Laser : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float DMG = 10;
    public Transform? PIVOT; // Laser Anchor
    public Vector2? DISP; // Displacement from the anchor
    public float? WARN = 1f; // Warning laser
    public float DURATION = 1f; // Laser lifetime
    public float? COOLDOWN = null;
    public float SIZE = 10f;
    public bool HAD_CASTER = false; // Becomes true if caster is real
    /*<------------------------------------->*/
    private Dictionary<Entity, bool> collided = new Dictionary<Entity, bool>();
    private Dictionary<Entity, float> hit = new Dictionary<Entity, float>();
    private GameObject? warn;
    protected override void Start()
    {
        // init projectile properties
        GetComponent<Collider2D>().enabled = false;
        DISABLE_DELETE = true;
        DISABLE_MOVE = true;
        BG = true;
        VALUE = DMG;
        SPD = 0; // default SPD to 0
        HAD_CASTER = Caster != null;

        // start coroutine
        base.Start();
        StartCoroutine(Begin());
    }

    protected override void Update()
    {
        // add pivot position
        RefreshPosition();

        // refresh hit
        Check();

        base.Update();
    }

    /* Projectile Functions */
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != TARGET) { return; }

        Entity? entity = collision.gameObject.GetComponent<Entity>();
        if (entity == null) { return; }

        OnExit(entity);
    }
    protected override void OnHit(Entity entity)
    {
        // Add entity to collision table
        if (COOLDOWN!=null && !collided.ContainsKey(entity))
        {
            collided[entity] = true;
        }

        if (hit.ContainsKey(entity)) { return; }
        hit[entity] = 0;
        Hit(entity);
    }
    private void OnExit(Entity entity)
    {
        // Remove entity from collision table
        if (!collided.ContainsKey(entity)) { return; }
        collided.Remove(entity);
    }
    private void Check()
    {
        if (COOLDOWN == null) { return; }

        foreach (var enemy in hit.Keys.ToList())
        {
            hit[enemy] -= Math.Min((float)COOLDOWN, Time.deltaTime);
            if (!collided.ContainsKey(enemy)) { continue; }
            Hit(enemy);
        }
    }
    private void Hit(Entity entity)
    {
        if (hit[entity] > 0 || entity.IsDestroyed()) { return; }
        hit[entity] += (float)(COOLDOWN!=null ? COOLDOWN : 1);

        entity.Damage(DMG, Caster);
        ImpactFX(entity.Position);
        if (entity.IsDestroyed())
        {
            hit.Remove(entity);
            collided.Remove(entity);
        }
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
        if (Caster == null && HAD_CASTER) { yield break; }
        warn = Game.Warn(duration, SIZE, transform);

        while (!warn.IsDestroyed()) { yield return null; }
    }
    private IEnumerator Laser(float duration)
    {
        if (Caster == null && HAD_CASTER) { yield break; }
        transform.localScale = new Vector3(SIZE, 10, SIZE);
        Laser Component = GetComponent<Laser>();
        Component.Begin(duration);

        AudioManager.PlaySound(AudioManager.asset.SND_Laser);
        float transition = Mathf.Min(.25f, duration / 2);

        yield return new WaitForSeconds(transition / 2);

        GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(duration - transition);

        GetComponent<Collider2D>().enabled = false;
    }
}
