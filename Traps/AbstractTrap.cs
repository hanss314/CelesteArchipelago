using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Celeste.Mod.CelesteArchipelago
{
    public abstract class AbstractTrap
    {
        // All public get values will be saved in the datastorage.
        private long TrapDeathDurationMax = 10;
        private long TrapRoomDurationMax = 3;
        public int DeathCount { get; protected set; } = 0;
        public HashSet<string> RoomStates { get; protected set; } = new();
        public bool IsActive { get; set; } = false;

        // No previous trap data exists, so all values should be default
        protected AbstractTrap(long deathDuration, long roomDuration)
        {
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
        }

        // Add all attributes that are recorded from previously played sessions to your trap.
        protected AbstractTrap(long deathDuration, long roomDuration, JToken trapValues)
        {
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
            IsActive = (bool)trapValues["IsActive"];
            DeathCount = (int)trapValues["DeathCount"];
            RoomStates = trapValues["RoomStates"].ToObject<HashSet<string>>();
        }

        // Load trap data from previous session
        public abstract void LoadTrap();

        // IsActive is set to true after the first successful call to this method, this can act like a first time ran check
        public abstract void SetTrap(object value, bool isExtending);

        // Always call this method before resetting the trap
        public virtual void ResetTrap()
        {
            IsActive = false;
            DeathCount = 0;
            RoomStates = new();
        }

        // Making a trap longer and/or harder based on getting sent multiple of this trap
        protected virtual void ExtendTrap()
        {
            DeathCount = 0;
            RoomStates = new();
        }

        public void IncrementDeathCount()
        {
            DeathCount++;

            // Disabling trap when DeathCount reaches TrapDeathDurationMax
            if (DeathCount >= TrapDeathDurationMax)
            {
                ResetTrap();
            }
        }

        public void IncrementRoomCount(string state)
        {
            RoomStates.Add(state);

            // Disabling trap upon rooms travelled is bigger than roomMax value (as to not include room trap spawned in)
            if (RoomStates.Count > TrapRoomDurationMax)
            {
                ResetTrap();
            }
        }
    }
}