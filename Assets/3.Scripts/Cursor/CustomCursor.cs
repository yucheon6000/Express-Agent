using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private bool hotspotIsCenter = true;

    [SerializeField]
    private Vector2 hotspot;

    private void Awake()
    {
        Vector2 hotspot =
            hotspotIsCenter
                ? new Vector2(cursorTexture.width / 2, cursorTexture.height / 2)
                : this.hotspot;

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
    }
}
