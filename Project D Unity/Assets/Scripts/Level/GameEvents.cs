using System;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// A series of events that every script can access easily
    /// </summary>
    public static class GameEvents
    {
        public static Action LevelFinished;
        public static Action PlayerDeath;
        public static Action<Quaternion> PlayerRespawn;
        public static Action TimeRanOut;
    }
}