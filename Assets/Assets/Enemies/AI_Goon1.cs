using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon1 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onExit()
    {
        entity.Look();
    }
    protected override IEnumerator Attack() // Behaviour when attacking the player
    {
        // Move randomly around the position
        entity.MoveRandom(BasePosition, 5.0f);

        /*<-------------------------------------->*/
        // Shoot at the player
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        entity.Look(player.transform);
        var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, player.Position);
        bullet.DMG = entity.DMG;

        yield return new WaitForSeconds(Cooldown);
    }
}
