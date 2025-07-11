using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Debug : MonoBehaviour
{
    [SerializeField] private GameObject box;
    public GameObject debug_box(Vector2 Position, Vector2 Size, Color? Color)
    {
        GameObject d_box = Instantiate(box);
        d_box.transform.parent = transform;
        d_box.transform.position = Position;
        d_box.transform.localScale = Size;
        if (Color != null)
        {
            d_box.GetComponent<SpriteRenderer>().color = (Color)Color;
        }
        return d_box;
    }
}
