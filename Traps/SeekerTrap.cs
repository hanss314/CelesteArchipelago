using System;
using Newtonsoft.Json.Linq;
using static CelesteArchipelago.ExtendedVariantInterop;

namespace Celeste.Mod.CelesteArchipelago
{
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

            TriggerVariant("AddSeekers", SeekerCount, false);
        }

        public override void ResetTrap()
        {
            base.ResetTrap();

            SeekerCount = 0;
            TriggerVariant("AddSeekers", SeekerCount, false);
        }

        protected override void ExtendTrap()
        {
            // Does not run on first call because IsActive is false initially
            if (IsActive)
            {
                return;
            }

            base.ExtendTrap();
            SeekerCount++;
        }
    }
}
