using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
#nullable enable

/// <summary>
/// Extra class for detecting player grazing
/// </summary>
public class PlayerGraze : MonoBehaviour
{
    private SpriteRenderer sprite = null!;
    private GameManager Game = null!; // mandatory GameManager
    private Entity entity = null!;
    private void Awake()
    {
        // init variables
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        entity = transform.parent.GetComponent<Entity>();
        sprite = GetComponent<SpriteRenderer>();

        StartCoroutine(GameInit());
    }

    // Init displayTP on GameManager
    private IEnumerator GameInit()
    {
        while (Game.TPUI == null) { yield return null; }
        Game.DisplayTP(entity.TP);
    }

    /* Stat Variables*/

    // On collision, checks if the projectile is targeting the player
    // Then adds TP to the player entity
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (entity.Invulnerable) { return; }
        Projectile? projectile = collision.gameObject.GetComponent<Projectile>();

        // --> cant be bothered to make this work with enemy collisions tbh
        if (projectile == null || !projectile.SameTarget("Player")) { return; }

        AudioManager.PlaySound(AudioManager.asset.SND_Graze);
        grazeRoutine = StartCoroutine(Animate());
        entity.AddTP(2);
    }

    // Enables the sprite
    // tween down the sprite to 0
    // If another tween is playing, stop it
    private Tween? grazeAnimation;
    private Coroutine? grazeRoutine;
    public Vector2 SpriteSize = new Vector2(17, 17);
    private IEnumerator Animate()
    {
        if (grazeAnimation != null) grazeAnimation.Kill();
        if (grazeRoutine != null) StopCoroutine(grazeRoutine);

        sprite.enabled = true;
        sprite.color = new Color(1,1,1);
        sprite.size = SpriteSize;
        grazeAnimation = DOTween.To(() => 1f, x => {
            sprite.size = SpriteSize * x;
            sprite.color = new Color(1, 1, 1, x);
        }, 0, 1).SetEase(Ease.OutSine).SetLink(gameObject);
        yield return new WaitForSeconds(1);

        sprite.enabled = false;
        grazeRoutine = null;
    }
}
