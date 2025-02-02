using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ExtendedVariants.Module;
using static ExtendedVariants.Module.ExtendedVariantsModule;

namespace Celeste.Mod.CelesteArchipelago
{
    public class Trap
    {
        private int TrapDeathDurationMax = 10;
        private int TrapRoomDurationMax = 3;
        public int DeathCount { get; protected set; } = 0;
        public HashSet<string> RoomStates { get; protected set; } = new();
        public bool IsActive { get; set; } = false;

        protected Trap(int deathDuration, int roomDuration)
        {
            // No previous trap data has been made to all values should be default
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
        }

        protected Trap(int deathDuration, int roomDuration, JToken trapValues)
        {
            // To your trap add all attributes that are recorded from previous played sessions.
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
            IsActive = (bool)trapValues["IsActive"];
            DeathCount = (int)trapValues["DeathCount"];
            RoomStates = trapValues["RoomStates"].ToObject<HashSet<string>>();
        }

        public virtual void SetTrap(object value, bool isExtending) //IsActive gets set to true after the first time this gets successfully called
        {
            throw new NotImplementedException("This method must be overridden in derived classes.");
        }

        public virtual void ResetTrap()
        {
            // Always call before making reset
            IsActive = false;
            DeathCount = 0;
            RoomStates = new();
        }

        public void IncrementDeathCount()
        {
            DeathCount++;

            // Disabling trap upon deathMax value
            if (DeathCount >= TrapDeathDurationMax)
            {
                ResetTrap();
            }
        }

        public void AddRoom(string state)
        {
            RoomStates.Add(state);

            // Disabling trap upon roomMax value, + 1 exists so that current spawning room does not count
            if (RoomStates.Count >= TrapRoomDurationMax + 1)
            {
                ResetTrap();
            }
        }
    }


    public class TheoCrystalTrap : Trap
    {
        public TheoCrystalTrap(int deathDuration, int roomDuration) : base(deathDuration, roomDuration) { }

        public TheoCrystalTrap(int deathDuration, int roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            if (IsActive)
            {
                SetTrap(true, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is bool boolValue)
            {
                LuaCutscenesUtils.TriggerVariant(Variant.TheoCrystalsEverywhere.ToString(), boolValue, false);
                
                if (isExtending)
                {
                    // Making the trap longer (or harder) as the trap has been collected twice in a short time frame
                    base.ResetTrap();
                }
            }
            else
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for TheoCrystalTrap. Expected bool.");
            }
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            LuaCutscenesUtils.TriggerVariant(Variant.TheoCrystalsEverywhere.ToString(), false, false);
        }
    }

    public class BadelineChasersTrap : Trap
    {
        public int chaserCount { get; protected set; } = 1;
        public BadelineChasersTrap(int deathDuration, int roomDuration) : base(deathDuration, roomDuration) { }

        public BadelineChasersTrap(int deathDuration, int roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            chaserCount = (int)trapValues["chaserCount"];

            if (IsActive)
            {
                SetTrap(true, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is bool boolValue)
            {
                LuaCutscenesUtils.TriggerVariant(Variant.BadelineChasersEverywhere.ToString(), boolValue, false);

                if (isExtending)
                {
                    if (IsActive)
                    {
                        chaserCount++;
                    }

                    LuaCutscenesUtils.TriggerVariant(Variant.ChaserCount.ToString(), chaserCount, false);
                    
                    
                    // Making the trap longer (or harder) as the trap has been collected twice in a short time frame
                    base.ResetTrap();
                }
            }
            else
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for BadelineChasersTrap. Expected bool.");
            }
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            chaserCount = 1;

            LuaCutscenesUtils.TriggerVariant(Variant.BadelineChasersEverywhere.ToString(), false, false);
            LuaCutscenesUtils.TriggerVariant(Variant.ChaserCount.ToString(), chaserCount, false);
        }
    }

    public class SeekerTrap : Trap
    {
        public int seekerCount { get; protected set; } = 0;
        public SeekerTrap(int deathDuration, int roomDuration) : base(deathDuration, roomDuration) { }

        public SeekerTrap(int deathDuration, int roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            seekerCount = (int)trapValues["seekerCount"];

            if (IsActive)
            {
                SetTrap(seekerCount, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is int intValue)
            {
                seekerCount = intValue;
                LuaCutscenesUtils.TriggerVariant(Variant.AddSeekers.ToString(), seekerCount, false);

                if (isExtending)
                {
                    // Making the trap longer (or harder) as the trap has been collected twice in a short time frame
                    base.ResetTrap();
                }
            }
            else
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for SeekerTrap. Expected int.");
            }
        }

        public override void ResetTrap()
        {
            base.ResetTrap();

            seekerCount = 0;
            LuaCutscenesUtils.TriggerVariant(Variant.AddSeekers.ToString(), seekerCount, false);
        }
    }
}