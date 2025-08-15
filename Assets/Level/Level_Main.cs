using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// look at GameManager for comments
/// cause im not commenting all over this script...
/// </summary>
public class Level_Main : GameManager
{
    public int EventHealthDropScore = 1500;
    public int EventAbilityIntervalScore = 20000;
    public int EventEndlessBossScore = 7500;
    public int EndlessBosses = 0;
    private int EndlessBossScore = 0;

    /* Init Variables */
    public void Start()
    {
        Init();
    }
    protected override IEnumerator Timeline()
    {
        AudioManager.PlaySong(AudioManager.asset.Game);
        yield return new WaitForSeconds(2);

        /* Score Events */
        ScoreEvent(5000, AbilityDrop(1));
        ScoreEvent(20000, AbilityDrop(2));
        ScoreEvent(30000, AbilityDrop(0));
        ScoreEvent(41000, AbilityDrop(3));
        ScoreEvent(60000 + EventAbilityIntervalScore, AbilityDropRecur(0));
        EndlessBossScore = PlayerPrefs.GetInt("GoonBoss", 0);

        /*--------------*/
        StartCoroutine(base.Timeline());
    }

    /* Player Manager */
    protected override void DefaultAbilities()
    {
        SetAbility("PSV_Default", 0);
    }

    /* Wave Manager */
    protected override IEnumerator NewWave()
    {
        IEnumerator Wave = // Wave Logic
            score < 500 ? Wave1() :
            score < 1000 ? Wave2() :
            score < 2000 ? Wave3() :
            score < 4000 ? Wave4() :
            score < 6000 ? Wave5() :
            score < 10000 ? Wave6() :
            score < 20000 ? BossWave1() :
            score < 22500 ? Wave7() :
            score < 24500 ? Wave8() :
            score < 25500 ? Wave9() :
            score < 31000 ? Wave10() :
            score < 41000 ? BossWave2() :
            WaveInf();

        // Wavely Events
        RandomHealthDrop();

        // Create Wave
        yield return StartCoroutine(Wave);
    }

    /* Events */
    private void RandomHealthDrop()
    {
        if (score < EventHealthDropScore) { return; }
        float chance = Mathf.Min((score - EventHealthDropScore) / 500, 100);

        if (RandomChance(chance))
        {
            float x = Random.Range(-50.0f, 50.0f);
            Shoot(projectiles.HealOrb, new Vector2(x, _settings.Boundaries.y + 20), "Player", 5, new Vector2(x, -_settings.Boundaries.y - 20));
        }
    }
    private IEnumerator AbilityDrop(int slot)
    {
        SpawnAbility(slot);
        yield break;
    }
    private IEnumerator AbilityDropRecur(int slot)
    {
        SpawnAbility(slot);

        EventAbilityIntervalScore *= 2;
        slot = (slot + 1)%4;

        ScoreEvent(60000 + EventAbilityIntervalScore, AbilityDropRecur(slot));
        yield break;
    }

    /* Waves */
    private IEnumerator Wave1()
    {
        yield return new WaitForSeconds(1f);

        // 2 Forward Goons
        for (int i = -20; i <= 20; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 20));
        }

        yield return new WaitForSeconds(.25f);

        // 3 Back Goons
        for (int i = -40; i <= 40; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 10));
        }

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Wave2()
    {
        yield return new WaitForSeconds(1f);

        // 3 Forward Goons
        for (int i = -20; i <= 20; i += 20)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 20));
        }

        yield return new WaitForSeconds(.25f);

        // 3 Back Goons
        for (int i = -40; i <= 40; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 10));
        }

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Wave3()
    {
        yield return new WaitForSeconds(1f);

        // 3 Forward Goons
        for (int i = -20; i <= 20; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 30));
        }

        yield return new WaitForSeconds(.25f);

        // 2 Middle Goons
        for (int i = -20; i <= 20; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 20));
        }

        yield return new WaitForSeconds(1f);

        // 1 Goon_2
        float x2 = Random.Range(-10.0f, 10.0f);
        SpawnEnemy(enemies.Goon2, new Vector2(x2, bounds.y + 20), new Vector2(x2, bounds.y - 10));

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Wave4()
    {
        yield return new WaitForSeconds(1f);

        // 2x
        for (int i = -20; i <= 20; i += 40)
        {
            // 1 Forward Goon_2
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon2, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - 30));

            yield return new WaitForSeconds(1f);

            // 3 Back Goons
            for (int z = -45; z <= 45; z += 45)
            {
                float x2 = x + Random.Range(-10.0f, 10.0f);
                float y = Random.Range(10.0f, 20.0f);
                SpawnEnemy(enemies.Goon1, new Vector2(x2, bounds.y + 20), new Vector2(x2, bounds.y - y));

                yield return new WaitForSeconds(.2f);
            }
        }

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Wave5()
    {
        // Goon_3s
        for (int i = 0; i < 3; i++)
        {
            float x = Random.Range(-50.0f, 50.0f);
            SpawnEnemy(enemies.Goon3, new Vector2(x, bounds.y + 30));
            yield return new WaitForSeconds(1.25f);
        }

        float x2 = Random.Range(-50.0f, 50.0f);
        SpawnEnemy(enemies.Goon2, new Vector2(x2, bounds.y + 30), new Vector2(x2, bounds.y - 30));
        yield return new WaitForSeconds(1.5f);
    }
    private IEnumerator Wave6()
    {
        // 3 Goon_2s
        for (int i = 0; i < 3; i++)
        {
            float x = Random.Range(-50.0f, 50.0f);
            float y = Random.Range(10.0f, 30.0f);
            SpawnEnemy(enemies.Goon2, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - y));

            yield return new WaitForSeconds(1.5f);
        }

        // Lone Goon_3
        float x2 = Random.Range(-50.0f, 50.0f);
        SpawnEnemy(enemies.Goon3, new Vector2(x2, bounds.y + 30));
        yield return new WaitForSeconds(2f);

        // 25% chance for Asteroid
        if (RandomChance(25))
        {
            float x = Random.Range(-40.0f, 40.0f);
            var asteroid = (PJ_Asteroid)Shoot(projectiles.Asteroid, new Vector2(x, 0), "Player", 50, 180);
            asteroid.DMG = 15;
            yield return new WaitForSeconds(2);
        }

        yield return new WaitForSeconds(.25f);
    }
    private IEnumerator Wave7()
    {
        // 5 Back Goons
        for (int i = 0; i < 5; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            float y = Random.Range(10.0f, 50.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - y));

            yield return new WaitForSeconds(.21f);
        }

        // 2 Goon_4
        for (int i = 0; i < 2; i++)
        {
            float x = Random.Range(-50.0f, 50.0f);
            float y = Random.Range(10.0f, 30.0f);
            SpawnEnemy(enemies.Goon4, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - y));

            yield return new WaitForSeconds(.5f);
        }
        
        // 50% chance for Goon_2
        if (RandomChance(50))
        {
            float x = Random.Range(-50.0f, 50.0f);
            float y = Random.Range(40.0f, 50.0f);
            SpawnEnemy(enemies.Goon2, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - y));
            yield return new WaitForSeconds(.5f);
        }

        // Asteroid
        float x2 = Random.Range(-40.0f, 40.0f);
        var asteroid = (PJ_Asteroid)Shoot(projectiles.Asteroid, new Vector2(x2, 0), "Player", 50, 180);
        asteroid.DMG = 15;
        yield return new WaitForSeconds(2f);
    }
    private IEnumerator Wave8()
    {
        // 3 Back Goons
        for (int i = 0; i < 3; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            float y = Random.Range(10.0f, 50.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - y));

            yield return new WaitForSeconds(.21f);
        }

        // Goon_4 & Goon_3 Barrage
        float x2 = Random.Range(-50.0f, 50.0f);
        float y2 = Random.Range(10.0f, 30.0f);
        SpawnEnemy(enemies.Goon4, new Vector2(x2, bounds.y + 30), new Vector2(x2, bounds.y - y2));

        float x3 = Random.Range(-50.0f, 50.0f);
        SpawnEnemy(enemies.Goon3, new Vector2(x3, bounds.y + 30));

        yield return new WaitForSeconds(.75f);

        // 3 Forward Goons
        for (int i = -20; i <= 20; i += 20)
        {
            float x4 = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x4, bounds.y + 20), new Vector2(x4, bounds.y - 20));
        }

        yield return new WaitForSeconds(2f);

        // Asteroid
        float x5 = Random.Range(-40.0f, 40.0f);
        var asteroid = (PJ_Asteroid)Shoot(projectiles.Asteroid, new Vector2(x5, 0), "Player", 50, 180);
        asteroid.DMG = 15;
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Wave9()
    {
        // Goon_5
        float x = Random.Range(-60.0f, 60.0f);
        SpawnEnemy(enemies.Goon5, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - 10));

        // 3 Bombs x3
        for (int i = 0; i < 3; i++)
        {
            for (int x2 = -30; x2 <= 30; x2 += 60)
            {
                SpawnEnemy(enemies.Bomb, new Vector2(x2, bounds.y + 20)).MoveExit(new Vector2(x2, -bounds.y - 20));
            }

            yield return new WaitForSeconds(1f);

            SpawnEnemy(enemies.Bomb, new Vector2(0, bounds.y + 20)).MoveExit(new Vector2(0, -bounds.y - 20));

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f);
    }
    private IEnumerator Wave10()
    {
        // Goon_5
        float x = Random.Range(-50.0f, 50.0f);
        SpawnEnemy(enemies.Goon5, new Vector2(x, bounds.y + 30), new Vector2(x, bounds.y - 10));

        yield return new WaitForSeconds(.75f);

        // 5 Goon_1
        for (int x2 = -50; x2 <= 50; x2 += 25)
        {
            SpawnEnemy(enemies.Goon1, new Vector2(x2, bounds.y + 30), new Vector2(x2, bounds.y - 30));

            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(2f);

        // 3 Goon_4
        for (int i = 0; i < 3; i++)
        {
            float x2 = Random.Range(-50.0f, 50.0f);
            float y = Random.Range(40.0f, 50.0f);
            SpawnEnemy(enemies.Goon4, new Vector2(x2, bounds.y + 30), new Vector2(x2, bounds.y - y));

            yield return new WaitForSeconds(.5f);
        }

        // 3 Goon_3
        for (int i = 0; i < 3; i++)
        {
            float x2 = Random.Range(-50.0f, 50.0f);
            SpawnEnemy(enemies.Goon3, new Vector2(x2, bounds.y + 30));

            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(1f);

        // Goon_2
        float x3 = Random.Range(-50.0f, 50.0f);
        SpawnEnemy(enemies.Goon2, new Vector2(x3, bounds.y + 30), new Vector2(x3, bounds.y - 20));

        yield return new WaitForSeconds(2f);
    }
    private IEnumerator BossWave1()
    {
        yield return WaitUntilDied();

        AudioManager.PlaySong(AudioManager.asset.Boss);
        yield return new WaitForSeconds(1f);

        // Goon Boss
        var boss = SpawnBoss(enemies.GoonBoss, new Vector2(0, bounds.y + 30), new Vector2(0, bounds.y - 30));

        // Goon 1 Barrage
        for (int i = 0; i < 15; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));
            yield return new WaitForSeconds(.25f);
        }

        yield return WaitUntilDied(boss);

        AudioManager.PlaySong(AudioManager.asset.Game);

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator BossWave2()
    {
        yield return WaitUntilDied();

        AudioManager.PlaySong(AudioManager.asset.Boss);
        yield return new WaitForSeconds(1f);

        // Goon Boss
        var boss = SpawnBoss(enemies.GoonBoss, new Vector2(0, bounds.y + 30), new Vector2(0, bounds.y - 30));
        boss.AddHP(5000);

        // 7 Goon_3s
        for (int i = 0; i < 7; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));

            // bomb every 3 goons
            if (i % 3 == 0)
            {
                float x2 = Random.Range(-60.0f, 60.0f);
                SpawnEnemy(enemies.Bomb, new Vector2(x2, bounds.y + 20)).MoveExit(new Vector2(x2, -bounds.y - 20));
            }

            yield return new WaitForSeconds(.55f);
        }

        // Enemy Barrage Until the boss dies
        IEnumerator SpawnEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);

                // Goon_4
                float x2 = Random.Range(40.0f, 60.0f);
                if (RandomChance(50)) x2 *= -1;
                SpawnEnemy(enemies.Goon4, new Vector2(x2, bounds.y + 20), new Vector2(x2, bounds.y - Random.Range(5.0f, 10.0f)));

                yield return new WaitForSeconds(15f);

                // Goon_2
                float x3 = Random.Range(40.0f, 60.0f);
                if (RandomChance(50)) x3 *= -1;
                SpawnEnemy(enemies.Goon2, new Vector2(x3, bounds.y + 20), new Vector2(x3, bounds.y - Random.Range(15.0f, 30.0f)));

                yield return new WaitForSeconds(5f);

                // Goon_3
                float x = Random.Range(-60.0f, 60.0f);
                SpawnEnemy(enemies.Goon3, new Vector2(x, bounds.y + 20));
            }
        }

        var loop = StartCoroutine(SpawnEnemies());

        yield return WaitUntilDied(boss);

        StopCoroutine(loop);
        AudioManager.PlaySong(AudioManager.asset.Game);

        yield return WaitUntilDied();

        SetEndlessBossScore(score + EventEndlessBossScore);

        yield return new WaitForSeconds(1f);
    }
    private Coroutine SpawnRandom(GameObject entity, float chance, int count, float interval, float? height, float delay)
    {
        return StartCoroutine(_SpawnRandom(entity, chance, count, interval, height, delay));
    }
    private IEnumerator WaveInf()
    {
        if (score > EndlessBossScore)
        {
            yield return StartCoroutine(BossWaveInf());
        }

        yield return SpawnRandom(enemies.Goon1, 25, Random.Range(5, 15), .05f, 40, .75f);
        yield return SpawnRandom(enemies.Goon2, 50, Random.Range(1, 3), .5f, 30, 1f);
        yield return SpawnRandom(enemies.Goon6, 100, Random.Range(1, 2), 1.5f, null, 1.5f);
        yield return SpawnRandom(enemies.Goon3, 33, Random.Range(2, 5), .15f, null, 1f);
        yield return SpawnRandom(enemies.Goon1, 25, Random.Range(5, 15), .05f, 40, .75f);
        yield return SpawnRandom(enemies.Goon4, 40, Random.Range(3, 6), .2f, 20, 1f);
        yield return SpawnRandom(enemies.Goon5, 66, Random.Range(1, 2), .7f, 10, 1.5f);
        yield return SpawnRandom(enemies.Goon3, 33, Random.Range(2, 5), .15f, null, 1f);
        yield return SpawnRandom(enemies.Goon1, 25, Random.Range(5, 15), .05f, 40, 1.75f);

        // mandatory 3-8 bombs
        for (int i = 0; i < Random.Range(3, 8); i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            SpawnEnemy(enemies.Bomb, new Vector2(x, bounds.y + 20)).MoveExit(new Vector2(x, -bounds.y - 20));

            yield return new WaitForSeconds(.33f);
        }
    }

    private IEnumerator _SpawnRandom(GameObject entity, float chance, int count, float interval, float? height, float delay)
    {
        if (!RandomChance(chance)) { yield break; }

        // spawn based on the randomcount and height
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            if (height != null)
            {
                float y = bounds.y - (float)height - Random.Range(0.0f, 10.0f);
                SpawnEnemy(entity, new Vector2(x, bounds.y + 20), new Vector2(x, y));
            } else
            {
                SpawnEnemy(entity, new Vector2(x, bounds.y + 20));
            }
            yield return new WaitForSeconds(interval);
        }

        // random meteor 20%
        if (RandomChance(20))
        {
            float x = Random.Range(-40.0f, 40.0f);
            var asteroid = (PJ_Asteroid)Shoot(projectiles.Asteroid, new Vector2(x, 0), "Player", 50, 180);
            asteroid.DMG = 15;
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(delay);
    }
    private IEnumerator BossWaveInf()
    {
        yield return WaitUntilDied();

        AudioManager.PlaySong(AudioManager.asset.Boss);
        yield return new WaitForSeconds(1f);
        EndlessBosses += 1;

        // Goon Boss
        var boss = SpawnBoss(enemies.GoonBoss, new Vector2(0, bounds.y + 30), new Vector2(0, bounds.y - 30));
        boss.AddHP(5000 + EndlessBosses * 1000);
        boss.score += EndlessBosses * 1000;

        // Goon_1 and Bomb Barrage
        for (int i = 0; i < 15; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            SpawnEnemy(enemies.Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));

            // bomb every 2 goons
            if (i % 2 == 0)
            {
                float x2 = Random.Range(-60.0f, 60.0f);
                SpawnEnemy(enemies.Bomb, new Vector2(x2, bounds.y + 20), new Vector2(x2, - bounds.y - 20));
            }

            yield return new WaitForSeconds(.25f);
        }

        // Enemy Barrage Until the boss dies
        IEnumerator SpawnEnemies()
        {
            yield return new WaitForSeconds(3.5f);

            while (true)
            {
                // Goon_5
                float x = Random.Range(-60.0f, 60.0f);
                SpawnEnemy(enemies.Goon5, new Vector2(x, bounds.y + 10));
                yield return new WaitForSeconds(3f);

                // scary asteroid
                var player = Entity.getPlayer();
                if (player == null) { continue; }

                var asteroid = (PJ_Asteroid)Shoot(projectiles.Asteroid, new Vector2(player.Position.x, 0), "Player", 50, 180);
                asteroid.DMG = 15;

                yield return new WaitForSeconds(7f);

                // 2 Bombs
                for (int i = 0; i < 2; i++)
                {
                    float x2 = Random.Range(-60.0f, 60.0f);
                    SpawnEnemy(enemies.Bomb, new Vector2(x2, bounds.y + 20), new Vector2(x2, -bounds.y - 20));

                    yield return new WaitForSeconds(1f);
                }

                yield return new WaitForSeconds(6f);
            }
        }

        var loop = StartCoroutine(SpawnEnemies());

        yield return WaitUntilDied(boss);

        StopCoroutine(loop);
        AudioManager.PlaySong(AudioManager.asset.Game);

        yield return WaitUntilDied();

        SetEndlessBossScore(score + EventEndlessBossScore);

        yield return new WaitForSeconds(1f);
    }
    private void SetEndlessBossScore(int newScore)
    {
        EndlessBossScore = newScore;
        PlayerPrefs.SetInt("GoonBoss", EndlessBossScore);
        PlayerPrefs.Save();
    }
}
