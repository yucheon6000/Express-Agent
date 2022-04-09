using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private Vector2 hotspot;

    private void Awake()
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
    }
}
