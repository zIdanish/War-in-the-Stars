using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon5 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;
    private Coroutine Idle;
    private bool Attacking = false;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onAttack()
    {
        Idle = Call(BeginIdle());
    }
    protected override void onExit()
    {
        End(Idle);
    }
    private IEnumerator BeginIdle()
    {
        while (true) {
            if (!Attacking)
            {
                entity.MoveTo(new Vector2(entity.Position.x + UnityEngine.Random.Range(-10f, 10f), BasePosition.y));
                yield return StartCoroutine(WaitUntilStationary());
                yield return new WaitForSeconds(.5f);
            }
            else yield return null;
        }
    }
    protected override Vector2 exitPosition()
    {
        var player = Entity.getPlayer();
        if (player == null) { return new Vector2(entity.Position.x, _settings.Boundaries.y + 20); }

        var direction = entity.Position - player.Position;
        direction.Normalize();
        return player.Position + _settings.Height * 2 * direction;
    }
    protected override IEnumerator Attack()
    {
        Attacking = true;
        var player = Entity.getPlayer();
        if (player == null) { yield return null; yield break; }
        entity.Look(player.Position);
        entity.MoveTo(entity.Position);

        var laser = (PJ_Laser)entity.Shoot(Projectile, 0, player.Position);
        laser.WARN = 2;
        laser.DURATION = 1;
        laser.DMG = entity.DMG;
        laser.PIVOT = transform;
        laser.SIZE = 2.5f;

        Call(OnLaserEnd(laser.gameObject)); // disables attacking value when laser gets destroyed

        yield return new WaitForSeconds(Cooldown);
    }
    private IEnumerator OnLaserEnd(GameObject laser)
    {
        while (!laser.IsDestroyed()) { yield return null; }

        var player = Entity.getPlayer();
        if (player == null) { yield return null; yield break; }

        entity.Look(player.transform);
        Attacking = false;
    }
}
