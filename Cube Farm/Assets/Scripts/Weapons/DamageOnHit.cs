using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using CubeFarm.Player;

public class DamageOnHit : NetworkBehaviour
{
    [Range(-100, 100)]
    public float damage = 5;

    public AudioSource audioSource;

    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    [ServerCallback]
    private void OnTriggerEnter(Collider co)
    {
        //Hit another player
        if (co.tag.Equals("Player"))
        {
            var pm = co.GetComponent<PlayerManager>();

            //Apply damage
            if (pm.GetHealth() - damage <= Mathf.Epsilon)
            {
                // killed a dumb player

                //update score on source
                pm.score -= 1;
            }

            pm.ModifyHealth(-damage);

            if (audioSource != null)
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, audioSource.volume);
        }
    }
}