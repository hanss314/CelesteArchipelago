using System;
using System.Collections.Generic;

namespace Celeste.Mod.CelesteArchipelago
{
    public class CelesteArchipelagoTrapManager
    {
        private Dictionary<TrapType, Trap> Traps = new();

        public CelesteArchipelagoTrapManager(){}

        public CelesteArchipelagoTrapManager(long trapDeathDuration, long trapRoomDuration)
        {
            GenerateTraps(trapDeathDuration, trapRoomDuration);
        }

        private void GenerateTraps(long trapDeathDuration, long trapRoomDuration)
        {
            Traps.Add(TrapType.THEO_CRYSTAL, new TheoCrystalTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.BADELINE_CHASERS, new BadelineChasersTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.SEEKERS, new SeekerTrap(trapDeathDuration, trapRoomDuration));
        }

        public void AddTrap(TrapType trapID)
        {
            // Traps automatically get disabled, so only make ways to increment or turn on
            switch (trapID)
            {
                case TrapType.THEO_CRYSTAL:
                    Traps[trapID].SetTrap(true);
                    break;
                case TrapType.BADELINE_CHASERS:
                    Traps[trapID].SetTrap(true);
                    break;
                case TrapType.SEEKERS:
                    SeekerTrap seekerTrap = (SeekerTrap)Traps[trapID];
                    seekerTrap.SetTrap(seekerTrap.seekerCount + 1);

                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Trap Type {trapID} has not been implemented");
            }

            Traps[trapID].IsActive = true;
        }

        public void ResetAllTraps()
        {
            foreach (var trap in Traps.Values)
            {
                trap.ResetTrap();
            }
        }

        public void IncrementAllDeathCounts()
        {
            foreach (var trap in Traps.Values)
            {
                if (trap.IsActive)
                {
                    trap.IncrementDeathCount();
                }
            }
        }

        public void AddRoomState(PlayState state)
        {
            foreach (var trap in Traps.Values)
            {
                if (trap.IsActive)
                {
                    trap.AddRoom(state);
                }
            }
        }
    }
}
