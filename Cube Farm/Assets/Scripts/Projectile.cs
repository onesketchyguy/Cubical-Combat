using CubeFarm.Player;
using Mirror;
using UnityEngine;

namespace CubeFarm
{
    public class Projectile : NetworkBehaviour
    {
        internal float destroyAfter = 1;
        public Rigidbody rigidBody;
        internal float launchForce = 1000;

        internal float damage = 5;
        internal GameObject source;

        public AudioSource audioSource;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);

            if (audioSource != null)
            {
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, audioSource.volume);
            }
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        private void Start()
        {
            if (source != null) rigidBody.velocity = source.GetComponent<Rigidbody>().velocity;

            rigidBody.AddForce(transform.forward * launchForce);
        }

        // destroy for everyone on the server
        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        // ServerCallback because we don't want a warning if OnTriggerEnter is
        // called on the client
        [ServerCallback]
        private void OnTriggerEnter(Collider co)
        {
            //Hit another player
            if (co.tag.Equals("Player") && co.gameObject != source)
            {
                var pm = co.GetComponent<PlayerManager>();

                //Apply damage
                if (pm.GetHealth() - damage <= Mathf.Epsilon)
                {
                    // killed opponent

                    //update score on source
                    source.GetComponent<PlayerManager>().score += 1;
                }
                pm.ModifyHealth(-damage);
            }

            DestroySelf();
        }
    }
}