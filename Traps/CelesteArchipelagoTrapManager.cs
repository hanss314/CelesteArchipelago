using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;
using Newtonsoft.Json.Linq;

namespace Celeste.Mod.CelesteArchipelago
{
    public class CelesteArchipelagoTrapManager
    {
        public Dictionary<TrapType, Trap> Traps = new();
        private int LocalTrapCounter = 0; // Unrelated to Traps
        private int SavedTrapCounter = 0;

        public CelesteArchipelagoTrapManager() {}

        public CelesteArchipelagoTrapManager(int trapDeathDuration, int trapRoomDuration)
        {
            GenerateTraps(trapDeathDuration, trapRoomDuration);
        }

        public CelesteArchipelagoTrapManager(int SavedTrapCounter, int trapDeathDuration, int trapRoomDuration, JObject Traps)
        {
            this.SavedTrapCounter = SavedTrapCounter;
            GenerateTraps(trapDeathDuration, trapRoomDuration, Traps);
        }

        private void GenerateTraps(int trapDeathDuration, int trapRoomDuration)
        {
            // Create new trap
            Traps.Add(TrapType.THEO_CRYSTAL, new TheoCrystalTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.BADELINE_CHASERS, new BadelineChasersTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.SEEKERS, new SeekerTrap(trapDeathDuration, trapRoomDuration));
        }

        private void GenerateTraps(int trapDeathDuration, int trapRoomDuration, JObject traps)
        {
            // Create trap based off previous data
            Traps.Add(TrapType.THEO_CRYSTAL, new TheoCrystalTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.THEO_CRYSTAL.ToString()]));
            Traps.Add(TrapType.BADELINE_CHASERS, new BadelineChasersTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.BADELINE_CHASERS.ToString()]));
            Traps.Add(TrapType.SEEKERS, new SeekerTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.SEEKERS.ToString()]));
        }

        public void AddTrap(TrapType trapID)
        {
            // Upon loading previous save prevents traps from re-loading on screen
                        
            if (LocalTrapCounter < SavedTrapCounter)
            {
                LocalTrapCounter++;
                return;
            }

            // Traps automatically get disabled, so only make ways to increment or turn on
            switch (trapID)
            {
                case TrapType.THEO_CRYSTAL:
                    Traps[trapID].SetTrap(true, true);
                    break;
                case TrapType.BADELINE_CHASERS:
                    Traps[trapID].SetTrap(true, true);
                    break;
                case TrapType.SEEKERS:
                    SeekerTrap seekerTrap = (SeekerTrap)Traps[trapID];
                    seekerTrap.SetTrap(seekerTrap.seekerCount + 1, true);

                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Trap Type {trapID} has not been implemented");
            }

            Traps[trapID].IsActive = true;
            LocalTrapCounter++;

            ArchipelagoController.Instance.Session.DataStorage[Scope.Slot, "CelesteTrapCount"] = LocalTrapCounter;
            ArchipelagoController.Instance.Session.DataStorage[Scope.Slot, "CelesteTrapState"] = JObject.FromObject(Traps);
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

            ArchipelagoController.Instance.Session.DataStorage[Scope.Slot, "CelesteTrapState"] = JObject.FromObject(Traps);
        }

        public void AddRoomToAllTraps(string room)
        {
            foreach (var trap in Traps.Values)
            {
                if (trap.IsActive)
                {
                    trap.AddRoom(room);
                }
            }

            ArchipelagoController.Instance.Session.DataStorage[Scope.Slot, "CelesteTrapState"] = JObject.FromObject(Traps);
        }
    }
}