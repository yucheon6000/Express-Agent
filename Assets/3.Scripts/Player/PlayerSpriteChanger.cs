using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        if (!spriteRenderer) return;
        if ((int)playerAngle >= sprites.Length) return;

        Sprite sprite = sprites[(int)playerAngle];

        spriteRenderer.sprite = sprite;
    }
}
