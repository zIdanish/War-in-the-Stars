using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable

public class AI_Bomb : AI
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile = null!;

    /* Init Variables */
    private void Start()
    {
        Init();
        entity.OnDeath += (Attacker) => { StartCoroutine(Explode(Attacker)); };
    }

    /*<----------------Timeline--------------->*/
    protected override IEnumerator Timeline()
    {
        entity.Look(-entity.Angle);
        while (entity.Moving) { yield return null; }
        entity.Die(null);
    }
    private IEnumerator Explode(Entity? Attacker) // Behaviour when attacking the player
    {
        if (Attacker == null) { yield break; }
        AudioManager.PlaySound(AudioManager.asset.SND_Explode);

        for (int angle = 0; angle < 90; angle += 30)
        {
            for (float x = 0; x < 360; x += 30)
            {
                var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, x + angle);
                bullet.transform.localScale *= 1.25f;
                bullet.DMG = entity.DMG;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
