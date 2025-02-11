using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ExtendedVariants.Module;
using static ExtendedVariants.Module.ExtendedVariantsModule;

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
            // IsActive is set to true after the first successful call to this method
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

            // Disable trap when DeathCount reaches TrapDeathDurationMax
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


    public class TheoCrystalTrap : Trap
    {
        public TheoCrystalTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public TheoCrystalTrap(long deathDuration, long roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            
        }

        public override void LoadTrap()
        {
            if (IsActive)
            {
                SetTrap(true, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is not bool)
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for TheoCrystalTrap. Expected bool.");
            }

            if (isExtending)
            {
                base.ExtendTrap();
            }

            LuaCutscenesUtils.TriggerVariant(Variant.TheoCrystalsEverywhere.ToString(), (bool)value, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            LuaCutscenesUtils.TriggerVariant(Variant.TheoCrystalsEverywhere.ToString(), false, false);
        }
    }

    public class BadelineChasersTrap : Trap
    {
        public int ChaserCount { get; protected set; } = 1;
        public BadelineChasersTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public BadelineChasersTrap(long deathDuration, long roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            ChaserCount = (int)trapValues["ChaserCount"];
        }

        public override void LoadTrap()
        {
            if (IsActive)
            {
                SetTrap(true, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is not bool)
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for BadelineChasersTrap. Expected bool.");
            }

            if (isExtending)
            {
                ExtendTrap();
            }

            LuaCutscenesUtils.TriggerVariant(Variant.BadelineChasersEverywhere.ToString(), (bool)value, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            ChaserCount = 1;

            LuaCutscenesUtils.TriggerVariant(Variant.BadelineChasersEverywhere.ToString(), false, false);
            LuaCutscenesUtils.TriggerVariant(Variant.ChaserCount.ToString(), ChaserCount, false);
        }

        protected override void ExtendTrap()
        {
            base.ExtendTrap();

            if (IsActive)
            {
                ChaserCount++;
            }

            LuaCutscenesUtils.TriggerVariant(Variant.ChaserCount.ToString(), ChaserCount, false);
        }
    }

    public class SeekerTrap : Trap
    {
        public int SeekerCount { get; protected set; } = 0;
        public SeekerTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public SeekerTrap(long deathDuration, long roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            SeekerCount = (int)trapValues["SeekerCount"];
        }

        public override void LoadTrap()
        {
            if (IsActive)
            {
                SetTrap(SeekerCount, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is not int)
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for SeekerTrap. Expected int.");
            }

            SeekerCount = (int)value;

            if (isExtending)
            {
                ExtendTrap();
            }

            LuaCutscenesUtils.TriggerVariant(Variant.AddSeekers.ToString(), SeekerCount, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();

            SeekerCount = 0;
            LuaCutscenesUtils.TriggerVariant(Variant.AddSeekers.ToString(), SeekerCount, false);
        }

        protected override void ExtendTrap()
        {
            base.ExtendTrap();
            SeekerCount++;
        }
    }

    public class StaminaTrap : Trap
    {
        public int StaminaCount { get; protected set; } = 110;
        public StaminaTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public StaminaTrap(long deathDuration, long roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues)
        {
            StaminaCount = (int)trapValues["StaminaCount"];
        }

        public override void LoadTrap()
        {
            if (IsActive)
            {
                SetTrap(StaminaCount, false);
            }
        }

        public override void SetTrap(object value, bool isExtending)
        {
            if (value is not int)
            {
                throw new ArgumentException($"{value.GetType()} is an invalid value type for StaminaTrap. Expected int.");
            }

            StaminaCount = (int)value;

            if (isExtending)
            {
                base.ExtendTrap();
            }

            LuaCutscenesUtils.TriggerVariant(Variant.Stamina.ToString(), StaminaCount, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            StaminaCount = 110;
            
            LuaCutscenesUtils.TriggerVariant(Variant.Stamina.ToString(), StaminaCount, false);
        }

        protected override void ExtendTrap()
        {
            base.ExtendTrap();
            StaminaCount -= 15;
        }
    }
}