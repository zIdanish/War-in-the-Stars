using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PSV_Homing : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 1f;
    /*<-------------------------------------->*/
    public float DamageMultiplier = 2;
    public float ProjectileSpeed = 50;
    public float HomingDelay = .25f;
    public float HomingTime = .5f;
    public GameObject Projectile;
    /*<-------------------------------------->*/
    private void Start()    { Init(); }
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
        for (int i = 0; i < 5; i++)
        {
            var angle = UnityEngine.Random.Range(100, 260);

            var enemy = getClosest();

            var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, angle);
            bullet.DMG = entity.DMG * DamageMultiplier;
            if (enemy == null) { return; }

            StartCoroutine(LockOn(enemy, bullet, angle));
        }
    }
    private IEnumerator LockOn(Entity enemy, Projectile bullet, float angle)
    {
        bullet.DISABLE_DELETE = true;
        bullet.Accelerate(-ProjectileSpeed * .75f, HomingDelay);

        yield return new WaitForSeconds(HomingDelay);
        
        bullet.Accelerate(ProjectileSpeed * .75f, HomingTime);

        float elapsed = 0;
        var direction = bullet.Direction;

        while (elapsed < HomingTime && !bullet.IsDestroyed())
        {
            var diff = (enemy.Position - bullet.Position);
            diff.Normalize();
            elapsed += Time.deltaTime;

            bullet.MoveBy(Vector2.Lerp(direction, diff, elapsed/HomingTime));
            yield return null;
        }

        bullet.DISABLE_DELETE = false;
    }
}
