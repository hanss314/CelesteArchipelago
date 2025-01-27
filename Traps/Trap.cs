using System;
using System.Collections.Generic;
using ExtendedVariants.Module;
using ExtendedVariants.UI;
using Newtonsoft.Json.Linq;

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
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
        }

        protected Trap(int deathDuration, int roomDuration, JToken trapValues)
        {
            // To your trap add all attributes that are recorded.
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
                ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.TheoCrystalsEverywhere, boolValue);
                
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
            
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.TheoCrystalsEverywhere, false);
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
                ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.BadelineChasersEverywhere, boolValue);

                if (isExtending)
                {
                    if (IsActive)
                    {
                        chaserCount++;
                    }

                    ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.ChaserCount, chaserCount);
                    
                    
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
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.BadelineChasersEverywhere, false);
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.ChaserCount, chaserCount);
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
                ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.AddSeekers, intValue);

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
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.AddSeekers, seekerCount);
        }
    }
}