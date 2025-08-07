using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Big : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 0f;
    /*<-------------------------------------->*/
    public float DamageMultiplier = 5000f;
    public float ProjectileSpeed = 25;
    public GameObject Projectile;
    /*<-------------------------------------->*/
    private void Start()    {        Init();    }
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
        bullet.DMG = entity.DMG * DamageMultiplier;
    }
}
