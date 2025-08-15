using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
#nullable enable
public class AI_GoonBoss : AI
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile = null!;
    public GameObject Projectile_Bullet = null!;
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

        Coroutine AttackBehaviour = Call(Intro());

        yield return AttackBehaviour;
    }
    private IEnumerator Intro() // Beginning Attack Pattern
    {
        // Opening Zigzags
        yield return Call(ZigZags(25));

        yield return new WaitForSeconds(1);

        // Laser Barrage
        {
            // Laser Beam
            Laser(2, 2, 0);

            yield return new WaitForSeconds(2);

            // Spray
            yield return Call(MultiSpray(3));

            // Laser Beams
            Laser(2, 2, -30);
            Laser(2, 2, 30);

            yield return new WaitForSeconds(2);

            // Spray
            yield return Call(MultiSpray(3));
        }

        // Homing Laser
        var laser = Laser(3, 2, 0);
        if (laser != null)
        {
            Call(LaserFollow(laser, 2));

            yield return new WaitForSeconds(2);

            // Spray
            yield return Call(MultiSpray(7));

            while (!laser.IsDestroyed()) { yield return null; }
        }

        entity.Look();

        Call(Patterns());
    }

    private IEnumerator Patterns()
    {
        while (!entity.IsDestroyed())
        {
            yield return RandomTask( // --> too much effort to remove magic numbers in all of these pattern scripts
                // attack 1: Lunge at the player, make a sphere pattern
                Lunge(1, 50, 1),
                // attack 2: Create a spiral and laser the player multiple times
                SpiralLaser(3, .175f),
                // attack 3: Fast circle borders and snipes at the player repeatedly
                BorderSnipe(5),
                // attack 4: Move to Middle of the screen and just spam snipe at the player
                LaserRouletteSpray(4.5f)
            );
        }
    }

    /*<----------------Pattern Attacks--------------->*/
    private IEnumerator LaserRouletteSpray(float duration)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        entity.Look(player.transform);

        var undo = entity.Stat("SPD", 10);

        // move to middle
        entity.MoveTo(Vector2.zero);

        // yield a bit
        yield return Call(WaitUntilStationary());

        // warn of attack
        Game.Warn(1.25f, 10, transform, 100);
        yield return new WaitForSeconds(1.75f);

        // snipe a lot
        for (float i = 0; i < duration; i+=.15f) {
            Snipe(25);
            yield return new WaitForSeconds(.15f);
        }

        // choice
        if (RandomChance(25)) // 25% Chance to do a lunge instantly from that position
        {
            undo();
            yield return Call(Lunge(1.5f, 50, 1));
        } else { // 75% chance to just return back to normal position
            entity.MoveTo(BasePosition);
            entity.Look();
            yield return Call(WaitUntilStationary());
            undo();
        }
    }
    private IEnumerator BorderSnipe(float duration)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        entity.Look(player.transform);

        StartCoroutine(Border(duration, 25, 15));
        yield return new WaitForSeconds(.075f);
        StartCoroutine(Border(duration-.15f, 35, 15));

        for (float i = 0; i < duration-1; i += 2f)
        {
            yield return new WaitForSeconds(1f);
            for (int j = 0; j < 3; j++) {
                Snipe(15);
                yield return new WaitForSeconds(.33f);
            }
        }

        entity.Look();
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator SpiralLaser(float laser_count, float interval)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        // rotate the angle
        float angle = 0;
        DOTween.To(
            () => 0f,
            x => angle = x % 360,
            360f,
            laser_count*2+1
        );

        // spiral loop
        IEnumerator _SpiralLoop()
        {
            while (true)
            {
                Spiral(angle);
                yield return new WaitForSeconds(interval);
            }
        }

        var SpiralLoop = Call(_SpiralLoop());

        for (int i = 0; i < laser_count; i++) {
            entity.Look(player.Position);
            Laser(2, 1, 0);
            yield return new WaitForSeconds(2.5f);
        }

        StopCoroutine(SpiralLoop);

        yield return new WaitForSeconds(.25f);
        Sphere(0);

        yield return new WaitForSeconds(.1f);
        Sphere(15);

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Lunge(float foreswing, float speed, float backswing)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        // locks at the player, send a warning
        entity.Look(player.transform);
        entity.MoveTo(entity.Position + (entity.Position - player.Position) * 5f);
        Game.Warn(foreswing + .25f, 10, transform, 100);

        var undo_1 = entity.TweenStat("spd", 1 - entity.SPD, foreswing);

        yield return new WaitForSeconds(foreswing + .25f);

        // Lunge
        var undo_2 = entity.TweenStat("spd", speed, backswing);
        var direction = (player.Position - entity.Position);
        direction.Normalize();

        entity.Look(entity.Angle - 180);
        entity.MoveExit(player.Position + _settings.Height * 2 * direction);

        yield return new WaitForSeconds(.1f);

        // Sphere bullet pattern x3 when moving
        for (int i = 0; i < 3; i++)
        {
            Sphere(i * 15);
            yield return new WaitForSeconds(.1f);
        }

        yield return Call(WaitUntilStationary());

        entity.Look();
        entity.SetPosition(new Vector2(BasePosition.x, _settings.Height + 30));
        entity.MoveTo(BasePosition);

        // Undos the speed
        undo_1(0); undo_2(null);

        yield return Call(WaitUntilStationary());

        yield return new WaitForSeconds(1f);
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
    private void Spiral(float angle)
    {
        // 90 degrees bullet pattern
        for (int i = 0; i < 360; i+= 90)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, i + angle - entity.Angle);
            bullet.transform.localScale *= 1.25f;
            bullet.DMG = entity.DMG;
        }
    }
    private IEnumerator Border(float duration, float width, float add_angle)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        // border stuff
        for (float z = 0; z < duration; z += .15f)
        {
            for (float i = -1; i <= 1; i += 2)
            {
                var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, player.Position, add_angle * -i);
                bullet.SetPosition(
                    entity.Position + entity.Direction * 5 
                    + i * width * (Vector2)entity.transform.right / 2
                );

                bullet.transform.localScale *= 1.25f;
                bullet.DMG = entity.DMG;
            }

            yield return new WaitForSeconds(.1f);
        }
    }
    private void Snipe(float speed)
    {
        var player = Entity.getPlayer();
        if (player == null) { return; }

        // x3 long bullet to the player
        for (int i = -15; i <= 15; i += 15)
        {
            var bullet = (PJ_Damage)entity.Shoot(Projectile_Bullet, speed, player.Position);
            bullet.SetPosition(entity.Position + (Vector2)entity.transform.right * i);
            bullet.transform.localScale *= 1.25f;
            bullet.DMG = entity.DMG;
        }
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

        var laser = (PJ_Laser)entity.Shoot(Projectile_Laser, 0, player.Position, angle);
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
