using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Laser : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 7.5f;
    public float ProjectileSpeed = 0;
    public float Duration = 3;
    public float Size = 3;
    public float DamageMultiplier = 2f;
    /*<-------------------------------------->*/
    public GameObject Laser;
    /*<-------------------------------------->*/
    protected override void Awake() { base.Awake(); }
    public override void Link()
    {
        Laser = Game.Assets.Laser;
        base.Link();
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
        var laser = (PJ_Laser)entity.Shoot(Laser, 0, 0);
        laser.DMG = entity.DMG * DamageMultiplier;
        laser.DURATION = Duration;
        laser.PIVOT = entity.transform;
        laser.SIZE = Size;
        laser.WARN = 0;
        laser.COOLDOWN = .1f;
    }
}
