using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
#nullable enable

public class PlayerGraze : MonoBehaviour
{
    private GameManager Game = null!; // mandatory GameManager
    private Entity entity = null!;
    private void Awake()
    {
        // init variables
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        entity = transform.parent.GetComponent<Entity>();

        StartCoroutine(GameInit());
    }
    private IEnumerator GameInit()
    {
        while (Game.TPUI == null) { yield return null; }
        Game.DisplayTP(entity.TP);
    }

    /* Stat Variables*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (entity.Invulnerable) { return; }
        Projectile? projectile = collision.gameObject.GetComponent<Projectile>();

        // --> cant be bothered to make this work with enemy collisions tbh
        if (projectile == null || !projectile.SameTarget("Player")) { return; }

        entity.AddTP(2);
    }
}
