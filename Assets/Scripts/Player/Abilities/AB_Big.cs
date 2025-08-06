using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Big : Ability
{
    /*<-----------------Stats---------------->*/
    [SerializeField] private float Cooldown = 2f;
    /*<-------------------------------------->*/
    [SerializeField] private float DamageMultiplier = 2f;
    [SerializeField] private float ProjectileSpeed = 25;
    [SerializeField] private GameObject Projectile;
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
