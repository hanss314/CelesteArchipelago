using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Archipelago.MultiClient.Net.Enums;

namespace Celeste.Mod.CelesteArchipelago
{
    public class CelesteArchipelagoTrapManager
    {
        public TrapLoadStatus LoadStatus = TrapLoadStatus.NONE;
        public Dictionary<TrapType, AbstractTrap> Traps = new();
        private int LocalTrapCounter = 0; // Unrelated to Traps
        private int SavedTrapCounter = 0; // Unrelated to Traps

        public CelesteArchipelagoTrapManager() {}

        public CelesteArchipelagoTrapManager(long trapDeathDuration, long trapRoomDuration)
        {
            GenerateTraps(trapDeathDuration, trapRoomDuration);
            LoadStatus = TrapLoadStatus.PENDING;
        }

        public CelesteArchipelagoTrapManager(long trapDeathDuration, long trapRoomDuration, int SavedTrapCounter, JObject Traps)
        {
            this.SavedTrapCounter = SavedTrapCounter;
            GenerateTraps(trapDeathDuration, trapRoomDuration, Traps);
            LoadStatus = TrapLoadStatus.PENDING;
        }

        private void GenerateTraps(long trapDeathDuration, long trapRoomDuration)
        {
            // Create new trap
            Traps.Add(TrapType.THEO_CRYSTAL, new TheoCrystalTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.BADELINE_CHASERS, new BadelineChasersTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.SEEKER, new SeekerTrap(trapDeathDuration, trapRoomDuration));
            Traps.Add(TrapType.STAMINA, new StaminaTrap(trapDeathDuration, trapRoomDuration));
        }

        private void GenerateTraps(long trapDeathDuration, long trapRoomDuration, JObject traps)
        {
            // Create trap based off previous data
            Traps.Add(TrapType.THEO_CRYSTAL, new TheoCrystalTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.THEO_CRYSTAL.ToString()]));
            Traps.Add(TrapType.BADELINE_CHASERS, new BadelineChasersTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.BADELINE_CHASERS.ToString()]));
            Traps.Add(TrapType.SEEKER, new SeekerTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.SEEKER.ToString()]));
            Traps.Add(TrapType.STAMINA, new StaminaTrap(trapDeathDuration, trapRoomDuration, traps[TrapType.STAMINA.ToString()]));
        }

        public void LoadTraps()
        {
            if (LoadStatus != TrapLoadStatus.PENDING)
            {
                return;
            }

            Logger.Log("CelesteArchipelago", "Pending trap load. Loading traps.");

            foreach (var trap in Traps.Values)
            {
                trap.LoadTrap();
            }

            LoadStatus = TrapLoadStatus.LOADED;
        }

        public void AddTrap(TrapType trapID)
        {
            // Upon loading previous save prevents traps from re-loading on screen
            if (LocalTrapCounter < SavedTrapCounter)
            {
                LocalTrapCounter++;
                return;
            }

            // Traps automatically get disabled
            switch (trapID)
            {
                case TrapType.THEO_CRYSTAL:
                    Traps[trapID].SetTrap(true, true);
                    break;
                case TrapType.BADELINE_CHASERS:
                    Traps[trapID].SetTrap(true, true);
                    break;
                case TrapType.SEEKER:
                    SeekerTrap seekerTrap = (SeekerTrap)Traps[trapID];
                    seekerTrap.SetTrap(seekerTrap.SeekerCount, true);

                    break;
                case TrapType.STAMINA:
                    StaminaTrap staminaTrap = (StaminaTrap)Traps[trapID];
                    Traps[trapID].SetTrap(staminaTrap.StaminaCount, true);
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

        public void IncrementAllRoomCounts(PlayState state)
        {
            foreach (var trap in Traps.Values)
            {
                if (trap.IsActive)
                {
                    trap.IncrementRoomCount(state.Room);
                }
            }

            ArchipelagoController.Instance.Session.DataStorage[Scope.Slot, "CelesteTrapState"] = JObject.FromObject(Traps);
        }
    }

    public enum TrapLoadStatus
    {
        NONE,
        PENDING,
        LOADED,
    }
}