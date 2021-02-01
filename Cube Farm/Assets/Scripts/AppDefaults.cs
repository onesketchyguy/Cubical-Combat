using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeFarm.Managment
{
    public static class AppDefaults
    {
        /// <summary>
        /// Username used for empty slots in the match lobby
        /// </summary>
        public const string LOADING_USERNAME = "Looking for employees...";

        public const float MAX_HEALTH = 100;

        public static void Init()
        {
            Application.targetFrameRate = 60;
        }
    }
}