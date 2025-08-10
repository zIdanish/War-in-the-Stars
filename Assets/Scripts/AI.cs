using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI : MonoBehaviour
{
    /* Init Variables */
    protected Entity entity;
    protected void Init()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(Timeline()); // Start the timeline
    }

    /*<----------------Timeline--------------->*/
    protected virtual IEnumerator Timeline() // Behaviour timeline
    {
        Debug.Log("No Timeline for this AI?!");
        yield return null;
    }
    protected Coroutine Call(IEnumerator routine) // Safely call a coroutine
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
    protected void End(Coroutine routine) // Safely end a coroutine
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }
    protected IEnumerator WaitUntilStationary()
    {
        while (entity.Moving)
        {
            yield return null;
        }
    }
    private IEnumerator __proxy()
    {
        yield return null;
    }
}
