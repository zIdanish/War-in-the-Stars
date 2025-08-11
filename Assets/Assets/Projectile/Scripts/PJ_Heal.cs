using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PJ_Heal : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float HEAL = 10;
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
        // Heals the entity and destroys this object
        entity.Heal(HEAL, Caster);
        Destroyed();
    }
}
