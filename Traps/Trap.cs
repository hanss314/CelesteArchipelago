using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExtendedVariants.Module;
using ExtendedVariants.UI;

namespace Celeste.Mod.CelesteArchipelago
{
    public abstract class Trap
    {
        private long TrapDeathDurationMax = 10;
        private long TrapRoomDurationMax  = 3;
        public long DeathCount { get; protected set; } = 0;
        public HashSet<PlayState> RoomStates { get; protected set; } = new();
        public bool IsActive { get; set; } = false;

        protected Trap(long deathDuration, long roomDuration)
        {
            TrapDeathDurationMax = deathDuration;
            TrapRoomDurationMax = roomDuration;
        }

        public abstract void SetTrap(object value); //IsActive gets set to true after the first time this gets successfully called
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

        public void AddRoom(PlayState state)
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
        public TheoCrystalTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public override void SetTrap(object value)
        {
            if (value is bool boolValue)
            {
                if (!IsActive)
                {
                    ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.TheoCrystalsEverywhere, boolValue);
                }
                else
                {
                    // "Extend" Trap and allow visits to previous rooms
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
            ExtendedVariantsModule.Instance.ResetToDefaultSettings();
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.BadelineChasersEverywhere, false);
        }
    }

    public class BadelineChasersTrap : Trap
    {
        public int chaserCount { get; protected set; } = 1;
        public BadelineChasersTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public override void SetTrap(object value)
        {
            if (value is bool boolValue)
            {
                if (!IsActive)
                {
                    ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.BadelineChasersEverywhere, boolValue);
                }
                else
                {
                    chaserCount++;
                    ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.ChaserCount, chaserCount);
                    
                    // "Extend" Trap and allow visits to previous rooms
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
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.BadelineChasersEverywhere, false);
            chaserCount = 1;
        }
    }

    public class SeekerTrap : Trap
    {
        public int seekerCount { get; protected set; } = 0;
        public SeekerTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public override void SetTrap(object value)
        {
            if (value is int intValue)
            {
                seekerCount = intValue;
                ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.AddSeekers, intValue);

                // "Extend" Trap and allow visits to previous rooms
                base.ResetTrap();
            }
            else
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for SeekerTrap. Expected int.");
            }
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            ModOptionsEntries.SetVariantValue(ExtendedVariantsModule.Variant.AddSeekers, 0);
            seekerCount = 0;
        }
    }
}