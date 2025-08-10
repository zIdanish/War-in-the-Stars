using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_GoonBoss : AI
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;
    /* Init Variables */
    protected Vector2 BasePosition;
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override IEnumerator Timeline() // Behaviour timeline
    {
        // Wait until the entity stops moving
        entity.SetInvulnerable(true);

        while (entity.Moving)
        {
            yield return null;
        }

        entity.SetInvulnerable(false);
        BasePosition = entity.Position;

        /*<-------------------------------------->*/
        // Start the Attack Behaviour

        Coroutine AttackBehaviour = Call(Intro());

        yield return AttackBehaviour;
    }
    private IEnumerator Intro() // Beginning Attack Pattern
    {
        for (int i = 0; i <= 50; i++) {
            var mult = Mathf.Abs(5 - i%9);
            for (int x = -5+i%2; x <= 5; x+=2)
            {
                var angle = x * 15 + mult * 2;
                var bullet = (PJ_Damage)entity.Shoot(Projectile, 7.5f, angle);
                bullet.transform.localScale *= 1.25f;
                bullet.DMG = entity.DMG;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
