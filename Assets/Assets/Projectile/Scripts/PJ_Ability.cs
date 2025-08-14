using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PJ_Ability : Projectile
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public int SLOT = 1; // 0 for passive, 1-3 for active abilities
    [NonSerialized] public string NAME = ""; // ability name
    /*<------------------------------------->*/
    public GameObject[] Icons = new GameObject[3]; // Ability image assets (there are 3)
    protected override void Start()
    {
        FRIENDLY = true; // --> friend inside me
        VALUE = Mathf.Infinity;

        // check if the ability type exists in the first place
        // if not then warn
        Type type = Type.GetType(NAME);
        if (type == null)
        {
            Debug.LogWarning($"Type {NAME} does not EXIST.");
        }

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    /* Projectile Functions */
    protected override void OnHit(Entity entity)
    {
        if (!entity.transform.CompareTag("Player")) return;

        Game.SetAbility(NAME, SLOT);

        Destroyed();
    }
}
