using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Test : GameManager
{
    /*<-------------------Enemies------------------>*/
    public GameObject Goon1;
    /*<-----------------Projectiles---------------->*/
    public GameObject HealOrb;
    public GameObject PlayerBullet;
    public GameObject PlayerBulletBig;

    /* Init Variables */
    public void Start()
    {
        Init();
    }

    /* Player Manager */
    protected override void PlayerAbilities()
    {
        AB_Default AB_Base = (AB_Default)player.AddAbility("AB_Default");
        AB_Base.Projectile = PlayerBullet;

        AB_Big AB_1 = (AB_Big)player.AddAbility("AB_Big", "q");
        AB_1.Projectile = PlayerBulletBig;
        AB_1.icon = UI.Find("Ability1");
    }

    /* Wave Manager */
    protected override IEnumerator NewWave()
    {
       IEnumerator Wave = // Wave Logic
            score < 2500 ? Wave1()
            : Wave2();

       // Random Health Drop
       if (Random.Range(score<2500 ? 75 : 1, 101) > 99)
       {
            float x = Random.Range(-50.0f, 50.0f);
            Shoot(HealOrb, new Vector2(x, _settings.Boundaries.y + 20), "Player", 5, new Vector2(x, -_settings.Boundaries.y - 20));
       }


       // Create Wave
       yield return StartCoroutine(Wave);
    }
    private IEnumerator Wave1()
    {
        yield return new WaitForSeconds(1f);

        // 2 Forward Goons
        for (int i = -20; i <= 20; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 20));
        }

        yield return new WaitForSeconds(.25f);

        // 3 Back Goons
        for (int i = -40; i <= 40; i += 40)
        {
            float x = i + Random.Range(-10.0f, 10.0f);
            SpawnEnemy(Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 10));
        }

        yield return new WaitForSeconds(2f);
    }
    private IEnumerator Wave2()
    {
        // 2 Forward Goons
        for (int x = -20; x <= 20; x += 40)
        {
            SpawnEnemy(Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 20));
        }

        yield return new WaitForSeconds(0f);

        // 3 Back Goons
        for (int x = -40; x <= 40; x += 40)
        {
            SpawnEnemy(Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - 10));
        }

        yield return new WaitForSeconds(0f);
    }
}
