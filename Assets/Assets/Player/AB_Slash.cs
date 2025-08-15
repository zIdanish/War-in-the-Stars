using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Slash : Ability
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 7.5f;
    public float ProjectileSpeed = 25f;
    public float DamageMultiplier = 2f;
    /*<-------------------------------------->*/
    public GameObject Slash;
    /*<-------------------------------------->*/
    protected override void Awake() { base.Awake(); }
    public override void Link()
    {
        /*-------------Stats-------------*/
        TP = 10;
        /*-------------------------------*/
        Slash = Game.abilities.Slash;
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
        var bullet = (PJ_Slash)entity.Shoot(Slash, ProjectileSpeed, 0);
        bullet.SetPosition(bullet.Position + bullet.Direction*10);
        bullet.DMG = entity.DMG * DamageMultiplier;
        bullet.transform.localScale *= .5f;

        var scale = bullet.transform.localScale;
        DOTween.To(() => scale, x => bullet.transform.localScale = x, scale * 4, 1).SetLink(bullet.gameObject);
    }
}
