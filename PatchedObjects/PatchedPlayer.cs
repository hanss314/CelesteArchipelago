using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.CelesteArchipelago;

public class PatchedPlayer : IPatchable
{
    public void Load()
    {
        Everest.Events.Player.OnDie += OnDie;
    }

    public void Unload()
    {
        Everest.Events.Player.OnDie -= OnDie;
    }

    private static void OnDie(Player player)
    {
        ArchipelagoController.Instance.trapManager.IncrementAllDeathCounts();
    }
}