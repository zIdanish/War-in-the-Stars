using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Test : GameManager
{
    /*<-------------------Enemies------------------>*/
    public GameObject Goon1;
    public GameObject Goon2;
    public GameObject Goon4;
    public GameObject Goon5;
    public GameObject GoonBoss;
    /*<-----------------Projectiles---------------->*/
    public GameObject HealOrb;
    public GameObject PlayerBullet;

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
        AB_1.Projectile = PlayerBullet;
        AB_1.icon = UI.Find("Ability1");
    }

    /* Wave Manager */
    protected override IEnumerator NewWave()
    {
        IEnumerator Wave = TestWave();

        // Create Wave
        yield return StartCoroutine(Wave);
    }
    private IEnumerator TestWave()
    {
        // Goon 2
        float x = Random.Range(-60.0f, 60.0f);
        SpawnEnemy(Goon5, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));

        yield return new WaitForSeconds(2f);
    }
    private IEnumerator BossWave()
    {
        yield return new WaitForSeconds(1f);

        // Goon Boss
        var boss = SpawnBoss(GoonBoss, new Vector2(0, bounds.y + 30), new Vector2(0, bounds.y - 30));

        // Goon 1 Barrage
        for (int i = 0; i < 15; i++)
        {
            float x = Random.Range(-60.0f, 60.0f);
            SpawnEnemy(Goon1, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));
            yield return new WaitForSeconds(.25f);
        }

        yield return WaitUntilDied(boss);
    }
    private IEnumerator FunnyWave()
    {
        // Goon 2
        float x = Random.Range(-60.0f, 60.0f);
        SpawnEnemy(Goon2, new Vector2(x, bounds.y + 20), new Vector2(x, bounds.y - Random.Range(5.0f, 30.0f)));

        yield return new WaitForSeconds(.05f);
    }
}
