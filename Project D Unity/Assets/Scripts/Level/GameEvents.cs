using System;

namespace Level
{
    /// <summary>
    /// A series of events that every script can access easily
    /// </summary>
    public static class GameEvents
    {
        public static Action LevelFinished;
        public static Action PlayerDeath;
        public static Action PlayerRespawn;
        public static Action TimeRanOut;
    }
}