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
            yield return StartCoroutine(AbilityPressed(input.Ability1));

            Attack();

            yield return AbilityCooldown(Cooldown, ability1);
        }
    }
    private void Attack()
    {
        entity.Shoot(Projectile, entity.DMG * DamageMultiplier, ProjectileSpeed, 0);
    }
}
