using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Big : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 2f;
    /*<-------------------------------------->*/
    public float DamageMultiplier = 10;
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
        bullet.transform.localScale *= 2;
        bullet.DMG = entity.DMG * DamageMultiplier;
    }
}
