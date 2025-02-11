using System;
using Newtonsoft.Json.Linq;
using static CelesteArchipelago.ExtendedVariantInterop;

namespace Celeste.Mod.CelesteArchipelago
{
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

            TriggerVariant("BadelineChasersEverywhere", (bool)value, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();
            
            ChaserCount = 1;

            TriggerVariant("BadelineChasersEverywhere", false, false);
            TriggerVariant("ChaserCount", ChaserCount, false);
        }

        protected override void ExtendTrap()
        {
            // Does not run on first call because IsActive is false initially
            if (!IsActive)
            {
                return;
            }

            base.ExtendTrap();
            ChaserCount++;

            TriggerVariant("ChaserCount", ChaserCount, false);
        }
    }
}
