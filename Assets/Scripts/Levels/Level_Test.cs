using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Test : GameManager
{
    /*<-------------------Enemies------------------>*/
    public GameObject Goon1;
    /*<-----------------Projectiles---------------->*/
    public GameObject HealOrb;

    /* Init Variables */
    public void Start()
    {
        Init();
    }

    /* Timeline */
    protected override IEnumerator Timeline()
    {
        yield return new WaitForSeconds(1f);

        SpawnEnemy(Goon1, new Vector2(0, bounds.y+20), new Vector2(0, bounds.y-10));

        yield return new WaitForSeconds(2f);
    }
}
