using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// AI core for enemies
/// This component creates a behavioural pattern for an enemy
/// Create a subclass of this script to create a new AI
/// Attach the AI to an entity
/// </summary>
public class AI : MonoBehaviour
{
    /*<------------Stats----------->*/
    protected Entity entity; // the entity this AI class is attached to

    /*<------------Init Functions----------->*/
    protected void Init()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(Timeline()); // Start the timeline
    }

    /*<----------------Timeline--------------->*/

    // Behaviour timeline
    // --> Placeholder, replace in the subclass
    protected virtual IEnumerator Timeline()
    {
        Debug.Log("No Timeline for this AI?!");
        yield return null;
    }

    // Safely call a coroutine
    protected Coroutine Call(IEnumerator routine)
    {
        if (routine != null)
        {
            return StartCoroutine(routine);
        }
        else
        {
            return StartCoroutine(__proxy());
        }
    }

    // Safely end a coroutine
    // --> Not sure why but this somehow fixed an error i keep having
    // --> Unity coroutines actually suck wth who made this garbage
    protected void End(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    // Yields the function until the entity stops moving
    protected IEnumerator WaitUntilStationary()
    {
        while (entity.Moving)
        {
            yield return null;
        }
    }

    // proxy for safely calling coroutines
    private IEnumerator __proxy()
    {
        yield return null;
    }
}
