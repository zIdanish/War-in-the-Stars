using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Default : Ability
{
    /*<-----------------Stats---------------->*/
    [SerializeField] private float Cooldown = .1f;
    /*<-------------------------------------->*/
    [SerializeField] private float DamageMultiplier = 1f;
    [SerializeField] private float ProjectileSpeed = 75;
    [SerializeField] private GameObject Projectile;
    /*<-------------------------------------->*/
    private void Start()    {        Init();    }
    public override IEnumerator Timeline()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Cooldown);
        }
    }
    private void Attack()
    {
        for (int angle = -30; angle <= 30; angle += 15)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, angle);
            bullet.DMG = entity.DMG * DamageMultiplier;
        }
    }
}
