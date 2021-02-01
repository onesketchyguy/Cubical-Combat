using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace CubeFarm.Player
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        public GameObject playerPrefab;

        private static Queue<Transform> spawnPoints = new Queue<Transform>();

        private static Dictionary<uint, Transform> usedPoints = new Dictionary<uint, Transform>();

        public static void AddSpawnPoint(Transform t)
        {
            spawnPoints.Enqueue(t);
        }

        public static void RemoveSpawnPoint(Transform t)
        {
            var n_queue = new Queue<Transform>();
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                var point = spawnPoints.Dequeue();

                if (point != t)
                    n_queue.Enqueue(point);
            }
        }

        internal Transform GetSpawnPoint(uint connID)
        {
            if (usedPoints.ContainsKey(connID))
            {
                // Return this users old position to the list
                Transform t;
                usedPoints.TryGetValue(connID, out t);

                spawnPoints.Enqueue(t);

                usedPoints.Remove(connID);
            }

            var point = spawnPoints.Dequeue();

            usedPoints.Add(connID, point);

            return point;
        }
    }
}