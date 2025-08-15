using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AB_Big : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 5f;
    public float DamageMultiplier = 20;
    public float ProjectileSpeed = 10;
    /*<-------------------------------------->*/
    public GameObject Projectile;
    /*<-------------------------------------->*/
    public override void Link()
    {
        /*-------------Stats-------------*/
        TP = 5;
        /*-------------------------------*/
        base.Link();
        Projectile = Game.abilities.Circle;
    }
    public override IEnumerator Timeline()
    {
        while (true)
        {
            yield return StartCoroutine(AbilityPressed());

            Attack();

            yield return AbilityCooldown(Cooldown);
        }
    }
    private void Attack()
    {
        var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, 0);
        bullet.transform.localScale *= 2;
        bullet.DMG = entity.DMG * DamageMultiplier;
    }
}
