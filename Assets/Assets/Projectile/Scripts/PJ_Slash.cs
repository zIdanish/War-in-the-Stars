using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

public class PJ_Slash : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float DMG = 10;
    /*<------------------------------------->*/
    protected override void Start()
    {
        VALUE = DMG;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile? projectile = collision.gameObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            if (projectile.SameTarget(TARGET) || projectile.VALUE > VALUE) { return; }

            projectile.Destroyed();
            return;
        }

        base.OnTriggerEnter2D(collision);
    }

    /* Projectile Functions */
    protected override void OnHit(Entity entity)
    {
        // Deals damage to the entity
        if (entity.Invulnerable) { return; }
        DMG = entity.Damage(DMG, Caster);
        ImpactFX(entity.Position);

    }
}
