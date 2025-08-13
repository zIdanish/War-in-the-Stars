using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PJ_Asteroid : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float DMG = 10;
    [NonSerialized] public float WARN = 1;
    /*<------------------------------------->*/
    private SpriteRenderer sprite;
    protected override void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(_Start());
    }
    private IEnumerator _Start()
    {
        sprite.enabled = false;

        Game.Warn(WARN, 10, transform.position);
        yield return new WaitForSeconds(WARN);

        sprite.enabled = true;
        SetPosition(new Vector2(Position.x, _settings.Height + 20));
        MoveTo(new Vector2(Position.x, -(_settings.Height + 20)));

        base.Start();
    }

    protected override void Update()
    {
        if (!sprite.enabled) { return; }
        base.Update();
    }

    /* Projectile Functions */
    protected override void OnHit(Entity entity)
    {
        // Deals damage to the entity
        if (entity.Invulnerable) { return; }
        entity.Damage(DMG, Caster);
    }
}
