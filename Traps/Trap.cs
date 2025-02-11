using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Celeste.Mod.CelesteArchipelago
{
    public class Trap
    {
        // All public get values will be saved in the datastorage.
        private long TrapDeathDurationMax = 10;
        private long TrapRoomDurationMax = 3;
        public int DeathCount { get; protected set; } = 0;
        public HashSet<string> RoomStates { get; protected set; } = new();
        public bool IsActive { get; set; } = false;

        protected Trap(long deathDuration, long roomDuration)
        {
            // No previous trap data exists, so all values should be default
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
        }

        protected Trap(long deathDuration, long roomDuration, JToken trapValues)
        {
            // Add all attributes that are recorded from previously played sessions to your trap.
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
            IsActive = (bool)trapValues["IsActive"];
            DeathCount = (int)trapValues["DeathCount"];
            RoomStates = trapValues["RoomStates"].ToObject<HashSet<string>>();
        }

        public virtual void LoadTrap()
        {
            // Load trap data from previous session
            throw new NotImplementedException("This method must be overridden in derived classes.");
        }

        public virtual void SetTrap(object value, bool isExtending)
        {
            // IsActive is set to true after the first successful call to this method, this can act like a first time ran check
            throw new NotImplementedException("This method must be overridden in derived classes.");
        }

        public virtual void ResetTrap()
        {
            // Always call this method before resetting the trap
            IsActive = false;
            DeathCount = 0;
            RoomStates = new();
        }

        protected virtual void ExtendTrap()
        {
            // Making a trap longer and/or harder based on getting sent multiple of this trap
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