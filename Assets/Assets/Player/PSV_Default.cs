using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSV_Default : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = .1f;
    /*<-------------------------------------->*/
    public float DamageMultiplier = 1f;
    public float ProjectileSpeed = 75;
    public GameObject Projectile;
    /*<-------------------------------------->*/
    protected override void Awake() { base.Awake(); }
    public override void Link()
    {
        Projectile = Game.abilities.Circle;
        base.Link();
    }
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
