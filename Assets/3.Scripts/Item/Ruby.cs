using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruby : Coin
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // base.OnTriggerEnter2D(other);

        if (other.tag.Equals(PlayerCollision.TAG))
        {
            PlayAudio();
            Player.IncreaseRubyCount(1);
            gameObject.SetActive(false);
        }
    }
}
