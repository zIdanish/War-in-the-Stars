using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PJ_Damage : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float DMG = 10;
    /*<------------------------------------->*/
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    /* Projectile Functions */
    protected override void OnHit(Entity entity)
    {
        // Deals damage to the entity
        if (entity.Invulnerable) { return; }
        DMG = entity.Damage(DMG, Caster);

        // Destroy this object if no damage is left
        if (DMG > 0) { return; }
        Destroy(this.gameObject);
    }
}
