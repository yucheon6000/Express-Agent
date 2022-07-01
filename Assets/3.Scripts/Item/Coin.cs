using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Coin : Item
{
    protected static readonly int coinAmount = 1;
    public static int CointAmount => coinAmount;

    [SerializeField]
    protected float minMoveSpeed = 5;
    [SerializeField]
    protected float maxMoveSpeed = 5;
    protected float currentMoveSpeed = 0;

    [SerializeField]
    protected float scatterTime = 0.5f;
    [SerializeField]
    protected float minScatterPower = 5;
    [SerializeField]
    protected float maxScatterPower = 5;
    protected float currentScatterPower;
    protected float scatterTimer = 0;

    protected bool scattered = false;
    protected bool targeting = false;

    protected Vector3 startPosition;

    protected Movement movement;

    protected virtual void Awake()
    {
        movement = GetComponent<Movement>();
    }

    protected virtual void Update()
    {
        if (targeting) UpdateMove();
        else if (!scattered) UpdateScatter();
    }

    protected virtual void UpdateScatter()
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

    protected virtual void UpdateMove()
    {
        Vector2 moveDirection = Player.Main.TargetPosition - transform.position;

        currentMoveSpeed += 1f * Time.deltaTime;

        float speed = Mathf.Clamp(currentMoveSpeed, minMoveSpeed, maxMoveSpeed);
        movement.SetMoveSpeed(speed);
        movement.SetMoveDirection(moveDirection);
    }

    [ContextMenu("Start Targeting")]
    public virtual void StartTargeting()
    {
        targeting = true;
    }

    protected virtual void OnEnable()
    {
        scattered = false;
        targeting = false;
        scatterTimer = 0;
        currentMoveSpeed = minMoveSpeed;
        startPosition = transform.position;

        currentScatterPower = Random.Range(minScatterPower, maxScatterPower);
        movement.SetMoveDirection(Random.insideUnitCircle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(PlayerCollision.TAG))
        {
            PlayAudio();
            Player.IncreaseCoinCount(coinAmount);
            gameObject.SetActive(false);
        }
    }
}
