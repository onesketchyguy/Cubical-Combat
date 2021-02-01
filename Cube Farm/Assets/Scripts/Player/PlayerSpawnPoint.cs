using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeFarm.Player
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake() => PlayerSpawnSystem.AddSpawnPoint(transform);

        private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPoint(transform);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(transform.position + transform.up * 0.5f, .75f);
            Gizmos.DrawSphere(transform.position + transform.up, .5f);
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f, .25f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + transform.up * 1.45f
                + transform.forward * 0.25f, .05f);
            Gizmos.DrawSphere(transform.position + transform.up * 1.45f
                + transform.forward * 0.3f, .025f);
            Gizmos.DrawSphere(transform.position + transform.up * 1.45f
                + transform.forward * 0.325f, .015f);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f
                + transform.forward * 0.25f
                + transform.right * 0.1f, .025f);
            Gizmos.DrawSphere(transform.position + transform.up * 1.48f
                + transform.forward * 0.25f
                + transform.right * -0.1f, .025f);

            Gizmos.DrawSphere(transform.position + transform.up
                + transform.forward * 0.5f, .1f);
            Gizmos.DrawSphere(transform.position + transform.up * .75f
                + transform.forward * 0.65f, .1f);
        }
    }
}