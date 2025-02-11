using System;
using Newtonsoft.Json.Linq;
using static CelesteArchipelago.ExtendedVariantInterop;

namespace Celeste.Mod.CelesteArchipelago
{
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
                ExtendTrap();
            }       

            TriggerVariant("Stamina", StaminaCount, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            StaminaCount = 110;
            
            TriggerVariant("Stamina", StaminaCount, false);
        }

        protected override void ExtendTrap()
        {
            // Does not run on first call because IsActive is false initially
            if (!IsActive)
            {
                StaminaCount = 30;
                return;
            }

            base.ExtendTrap();
            StaminaCount = Math.Max(0, StaminaCount - 10);
        }
    }
}
