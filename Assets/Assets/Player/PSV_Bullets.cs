using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PSV_Bullets : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = .12f;
    public float DamageMultiplier = 1f;
    public float ProjectileSpeed = 50;
    /*<-------------------------------------->*/
    public GameObject Projectile;
    /*<-------------------------------------->*/
    protected override void Awake() { base.Awake(); }
    public override void Link()
    {
        Projectile = Game.abilities.Bullet;
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
        for (int i = -2; i <= 2; i++)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, i*3);
            bullet.SetPosition(bullet.Position + new Vector2(i*7.5f, 0));
            bullet.DMG = entity.DMG * DamageMultiplier;
        }
    }
}
