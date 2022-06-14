using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Stamina : Item
{
    private static readonly int staminaAmount = 5;
    public static int StaminaAmount => staminaAmount;

    [SerializeField]
    private float minMoveSpeed = 5;
    [SerializeField]
    private float maxMoveSpeed = 5;
    private float currentMoveSpeed = 0;

    [SerializeField]
    private float scatterTime = 0.5f;
    [SerializeField]
    private float scatterPower = 5;
    private float currentScatterPower;
    private float scatterTimer = 0;

    [SerializeField]
    private float minCollisionDistance = 0.5f;

    private bool scattered = false;

    private Vector3 startPosition;

    private Movement movement;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        UpdateCollision();

        if (!scattered) UpdateScatter();
        else UpdateMove();
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

    private void UpdateCollision()
    {
        float distance = ((Vector2)Player.Main.TargetPosition - (Vector2)transform.position).sqrMagnitude;
        if (distance > Mathf.Pow(minCollisionDistance, 2)) return;

        Player.IncreaseStaminaCount(staminaAmount);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        scattered = false;
        scatterTimer = 0;
        currentMoveSpeed = minMoveSpeed;
        startPosition = transform.position;

        currentScatterPower = Random.Range(0, scatterPower);
        movement.SetMoveDirection(Random.insideUnitCircle);
    }
}
