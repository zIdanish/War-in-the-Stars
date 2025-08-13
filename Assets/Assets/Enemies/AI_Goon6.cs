using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon6 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;
    public GameObject Projectile2;
    public float SpinSpeed = 5;
    private GameObject CurrentProjectile;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onAttack()
    {
        Lifetime = 0;
        StartCoroutine(SpinAndMove());
        StartCoroutine(CustomPattern());
    }
    private IEnumerator SpinAndMove()
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        entity.MoveTo(player.Position);
        float angle = 0;

        while (true)
        {
            angle += Time.deltaTime * SpinSpeed * 360;
            entity.Look(angle);

            yield return null;
        }
    }
    private IEnumerator CustomPattern()
    {
        while (true)
        {

            CurrentProjectile = CurrentProjectile == Projectile2 ? Projectile : Projectile2;

            Vector2 diff = entity.Destination - entity.Position;
            diff.Normalize();

            for (int angle = 0; angle < 360; angle += 90)
            {
                var bullet = (PJ_Damage)entity.Shoot(CurrentProjectile, ProjectileSpeed, angle);
                bullet.SetPosition(bullet.Position + bullet.Direction * 10);
                bullet.SPD += bullet.SPD * Mathf.Clamp(Vector2.Dot(diff, bullet.Direction), 0, 1);
                bullet.DMG = entity.DMG;
            }

            yield return new WaitForSeconds(Cooldown);
        }
    }
    protected override Vector2 exitPosition()
    {
        return new Vector2(entity.Position.x, -_settings.Height - 20);
    }
    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(Cooldown);
    }
}
