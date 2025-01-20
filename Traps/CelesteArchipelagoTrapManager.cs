namespace Celeste.Mod.CelesteArchipelago
{
    public class CelesteArchipelagoTrapManager(long TrapDeathDuration, long TrapRoomDuration)
    {
        private long TrapRoomDurationMax = TrapRoomDuration;
        private long TrapDeathDurationMax = TrapDeathDuration;

        public void SetTrap(int trapID) {
            // Thinking of switching to making an enum
            switch (trapID) {
                case (int)TrapTypes.THEO_CRYSTAL:
                    SetTheoCrystal();
                    break;
                case (int)TrapTypes.BADELINE_CHASERS:
                    SetBadelineChasers();
                    break;
                case (int)TrapTypes.SEEKERS:
                    SetSeeker();
                    break;
            }
        }
        private void SetTheoCrystal() { Logger.Log("CelesteArchipelago", $"Toggle Theo Crystal"); }
        private void SetBadelineChasers() { Logger.Log("CelesteArchipelago", $"Toggle Badeline Chasers "); }
        private void SetSeeker() {Logger.Log("CelesteArchipelago", $"Toggle Seekers"); }
    }
}
