using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer topSpriteRenderer;
    [SerializeField]
    private SpriteRenderer bottomSpriteRenderer;
    [SerializeField]
    private float deltaTime;
    private float timer;
    [SerializeField]
    private GameObject ghostPart;

    private void Awake()
    {
        timer = deltaTime;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < deltaTime) return;

        timer = 0;

        SpriteRenderer spriteRenderer = ObjectPooler.SpawnFromPool<SpriteRenderer>(ghostPart.name, topSpriteRenderer.transform.position, Quaternion.identity);
        spriteRenderer.sprite = topSpriteRenderer.sprite;

        spriteRenderer = ObjectPooler.SpawnFromPool<SpriteRenderer>(ghostPart.name, bottomSpriteRenderer.transform.position, Quaternion.identity);
        spriteRenderer.sprite = bottomSpriteRenderer.sprite;
    }
}
