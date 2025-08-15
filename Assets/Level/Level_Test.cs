using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Test : GameManager
{
    public int EventHealthDropScore = 1500;

    /* Init Variables */
    public void Start()
    {
        Init();
    }

    /* Player Manager */
    protected override void DefaultAbilities()
    {
        SetAbility("PSV_Default", 0);
    }

    /* Wave Manager */
    protected override IEnumerator NewWave()
    {
        IEnumerator Wave = TestWave();

        // Wave Events
        RandomHealthDrop();


        // Create Wave
        yield return StartCoroutine(Wave);
    }

    /* Wave Events */
    private void RandomHealthDrop()
    {
        if (score < EventHealthDropScore) { return; }
        float chance = Mathf.Min((score - EventHealthDropScore) / 1000, 20);

        if (RandomChance(chance))
        {
            float x = Random.Range(-50.0f, 50.0f);
            Shoot(projectiles.HealOrb, new Vector2(x, _settings.Boundaries.y + 20), "Player", 5, new Vector2(x, -_settings.Boundaries.y - 20));
        }
    }
    private IEnumerator TestWave()
    {
        SpawnAbility(0);
        yield return new WaitForSeconds(2);
        SpawnAbility(1);
        yield return new WaitForSeconds(2);
        SpawnAbility(2);
        yield return new WaitForSeconds(2);
        SpawnAbility(3);
        yield return new WaitForSeconds(2);

        yield return new WaitForSeconds(999);
    }
}
