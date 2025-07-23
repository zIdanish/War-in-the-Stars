using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Test : GameManager
{
    /* Init Variables */
    public GameObject Enemy_Placeholder;
    public void Start()
    {
        Init();
        StartCoroutine(Timeline());
    }

    /* Timeline */
    public IEnumerator Timeline()
    {
        SpawnEnemy(Enemy_Placeholder, new Vector2(0,70), new Vector2(0,50));

        yield return null;
    }
}
