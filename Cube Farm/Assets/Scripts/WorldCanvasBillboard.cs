using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeFarm.Effects
{
    public class WorldCanvasBillboard : MonoBehaviour
    {
        private Canvas canvas;

        private void Start()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            if (Camera.main != null)
            {
                canvas.worldCamera = Camera.main;
                transform.forward = transform.position - Camera.main.transform.position;
            }
        }
    }
}