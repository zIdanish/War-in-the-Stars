using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class AI_Placeholder : MonoBehaviour
{
    /* Init Variables */
    public GameObject projectile;
    private Entity entity;
    private void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(Timeline());
    }

    /* Timeline */
    private void Update()
    {
        // pls work on this idk what to work on just work on something
    }
    public IEnumerator Timeline()
    {
        while (entity.Moving)
        {
            yield return null;
        }

        for (int i = -30; i <= 30; i += 15)
        {
            var player = entity.getPlayer();
            if (player == null) { yield return null; }
            entity.Shoot(projectile, 10, 75, i);
        }

        yield return null;
    }
}
