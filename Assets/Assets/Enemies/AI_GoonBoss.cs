using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
#nullable enable
public class AI_GoonBoss : AI
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile = null!;
    public GameObject Projectile_Laser = null!;
    /* Init Variables */
    protected Vector2 BasePosition;
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override IEnumerator Timeline() // Behaviour timeline
    {
        // Wait until the entity stops moving
        entity.SetInvulnerable(true);

        while (entity.Moving)
        {
            yield return null;
        }

        entity.SetInvulnerable(false);
        BasePosition = entity.Position;

        /*<-------------------------------------->*/
        // Start the Attack Behaviour

        Coroutine AttackBehaviour = Call(Patterns());

        yield return AttackBehaviour;
    }
    private IEnumerator Intro() // Beginning Attack Pattern
    {
        // Opening Zigzags
        yield return StartCoroutine(ZigZags(25));

        yield return new WaitForSeconds(1);

        // Laser Barrage
        {
            // Laser Beam
            Laser(2, 2, 0);

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(3));

            // Laser Beams
            Laser(2, 2, -30);
            Laser(2, 2, 30);

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(3));
        }

        // Homing Laser
        var laser = Laser(3, 2, 0);
        if (laser != null)
        {
            StartCoroutine(LaserFollow(laser, 2));

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(7));

            while (!laser.IsDestroyed()) { yield return null; }
        }

        entity.Look();

        Call(Patterns());
    }

    private IEnumerator Patterns()
    {
        while (!entity.IsDestroyed())
        {

            yield return RandomTask(
                // attack 1: Lunge at the player, make a sphere pattern
                Lunge(1, 50, 1),
                // attack 2: Create a spiral and laser the player once
                SpiralLaser(),
                // attack 3: Fast circle borders and snipes at the player repeatedly
                BorderSnipe(),
                // attack 4: Middle of the screen the roulette around the screen, spray at the player
                LaserRouletteSpray()
            );
        }
    }

    /*<----------------Pattern Attacks--------------->*/
    private IEnumerator LaserRouletteSpray()
    {
        yield return null;
    }
    private IEnumerator BorderSnipe()
    {
        yield return null;
    }
    private IEnumerator SpiralLaser()
    {
        yield return null;
    }
    private IEnumerator Lunge(float foreswing, float speed, float backswing)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        // locks at the player, send a warning
        entity.Look(player.transform);
        entity.MoveTo(entity.Position + (entity.Position - player.Position) * 5f);
        StartCoroutine(Game.Warn(foreswing, transform, 100));
        var undo_1 = entity.TweenStat("spd", 1 - entity.SPD, foreswing);

        yield return new WaitForSeconds(foreswing);

        // Lunge
        var undo_2 = entity.TweenStat("spd", speed, backswing);
        var direction = (player.Position - entity.Position);
        direction.Normalize();

        entity.Look(player.Position);
        entity.MoveExit(player.Position + _settings.Height * 2 * direction);

        yield return new WaitForSeconds(.1f);

        // Sphere bullet pattern x3 when moving
        for (int i = 0; i < 3; i++)
        {
            Sphere(i * 15);
            yield return new WaitForSeconds(.1f);
        }

        yield return StartCoroutine(WaitUntilStationary());

        undo_1(0); undo_2(0);

        entity.Look();
        entity.SetPosition(new Vector2(BasePosition.x, _settings.Height + 30));
        entity.MoveTo(BasePosition);

        yield return StartCoroutine(WaitUntilStationary());
    }
    private IEnumerator ZigZags(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var mult = Mathf.Abs(5 - i % 9);
            for (int x = -5 + i % 2; x <= 5; x += 2)
            {
                var angle = x * 15 + mult * 2;
                var bullet = (PJ_Damage)entity.Shoot(Projectile, 7.5f, angle);
                bullet.transform.localScale *= 1.25f;
                bullet.DMG = entity.DMG;
            }
            yield return new WaitForSeconds(.15f);
        }
    }
    private IEnumerator MultiSpray(int count)
    {
        entity.MoveRandom(BasePosition, 10);
        for (int i = 0; i < count; i++)
        {
            Spray((i % 2) * 17.5f);
            yield return new WaitForSeconds(.333f);
        }
    }

    /*<----------------Projectile Presets--------------->*/
    private void Sphere(float angle)
    {
        // create a sphere of bullets
        for (float x = 0; x < 360; x += 30)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, x + angle);
            bullet.transform.localScale *= 1.25f;
            bullet.DMG = entity.DMG;
        }
    }
    private void Spiral()
    {
        // 90 degrees bullet pattern
        // rotate CW
    }
    private void Snipe()
    {
        // x3 long bullet to the player
    }
    private void Spray(float angle)
    {
        for (float x = -90; x < 90; x += 17.5f)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, x + angle);
            bullet.transform.localScale *= 1.25f;
            bullet.DMG = entity.DMG;
        }
    }
    private PJ_Laser? Laser(float warn, float life, float angle)
    {
        var player = Entity.getPlayer();
        if (player == null) { return null; }
        entity.Look(player.transform.position);

        var laser = (PJ_Laser)entity.Shoot(Projectile_Laser, 0, player.transform.position, angle);
        laser.WARN = warn;
        laser.DURATION = life;
        laser.DMG = entity.DMG;
        laser.PIVOT = transform;

        return laser;
    }
    private IEnumerator LaserFollow(PJ_Laser laser, float duration)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // refresh orientation
            entity.Look(player.transform.position);
            laser.MoveTo(player.transform.position, entity.Position);
            laser.RefreshPosition();

            yield return null;
        }
    }
}
