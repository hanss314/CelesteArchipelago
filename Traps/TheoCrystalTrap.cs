using System;
using Newtonsoft.Json.Linq;
using static CelesteArchipelago.ExtendedVariantInterop;

namespace Celeste.Mod.CelesteArchipelago
{
    public class TheoCrystalTrap : AbstractTrap
    {
        public TheoCrystalTrap(long deathDuration, long roomDuration) : base(deathDuration, roomDuration) { }

        public TheoCrystalTrap(long deathDuration, long roomDuration, JToken trapValues) : base(deathDuration, roomDuration, trapValues) { }

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

            TriggerVariant("TheoCrystalsEverywhere", (bool)value, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            TriggerVariant("TheoCrystalsEverywhere", false, false);
        }
    }
}