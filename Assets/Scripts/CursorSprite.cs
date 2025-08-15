using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSprite : MonoBehaviour
{
    // this script is to just set the cursor sprite to the custom one
    public Texture2D texture;
    void Start()
    {
        UnityEngine.Cursor.SetCursor(texture, new Vector2(texture.width / 2, texture.height / 2), CursorMode.Auto);
    }
}
