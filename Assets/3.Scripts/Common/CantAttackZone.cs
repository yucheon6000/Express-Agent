using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantAttackZone : MonoBehaviour
{
    private bool cantAttack = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player Foot")) return;

        Player player = other.GetComponentInParent<Player>();

        if (!player) return;
        if (player != Player.Main) return;

        cantAttack = true;
        Player.CantAttack(true);
    }

    private void Update()
    {
        if (cantAttack)
            Player.CantAttack(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player Foot")) return;

        Player player = other.GetComponentInParent<Player>();

        if (!player) return;
        if (player != Player.Main) return;

        cantAttack = false;
        Player.CantAttack(false);
    }
}
