using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Coin : Item
{
    private static readonly int coinAmount = 10;
    public static int CointAmount => coinAmount;

    [SerializeField]
    private float minMoveSpeed = 5;
    [SerializeField]
    private float maxMoveSpeed = 5;
    private float currentMoveSpeed = 0;

    [SerializeField]
    private float scatterTime = 0.5f;
    [SerializeField]
    private float minScatterPower = 5;
    [SerializeField]
    private float maxScatterPower = 5;
    private float currentScatterPower;
    private float scatterTimer = 0;

    private bool scattered = false;
    private bool targeting = false;

    private Vector3 startPosition;

    private Movement movement;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (targeting) UpdateMove();
        else if (!scattered) UpdateScatter();
    }

    private void UpdateScatter()
    {
        scatterTimer += Time.deltaTime;
        float percent = scatterTimer / scatterTime;

        float speed = (currentScatterPower / 2) * (1 + Mathf.Cos(Mathf.Lerp(0, Mathf.PI, percent)));
        movement.SetMoveSpeed(speed);

        if (percent > 1f)
        {
            scattered = true;
        }
    }

    private void UpdateMove()
    {
        Vector2 moveDirection = Player.Main.TargetPosition - transform.position;

        currentMoveSpeed += 1f * Time.deltaTime;

        float speed = Mathf.Clamp(currentMoveSpeed, minMoveSpeed, maxMoveSpeed);
        movement.SetMoveSpeed(speed);
        movement.SetMoveDirection(moveDirection);
    }

    [ContextMenu("Start Targeting")]
    public void StartTargeting()
    {
        targeting = true;
    }

    private void OnEnable()
    {
        scattered = false;
        targeting = false;
        scatterTimer = 0;
        currentMoveSpeed = minMoveSpeed;
        startPosition = transform.position;

        currentScatterPower = Random.Range(minScatterPower, maxScatterPower);
        movement.SetMoveDirection(Random.insideUnitCircle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(PlayerCollision.TAG))
        {
            Player.IncreaseCoinCount(coinAmount);
            gameObject.SetActive(false);
        }
    }
}
