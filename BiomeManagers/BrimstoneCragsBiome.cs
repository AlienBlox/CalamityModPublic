﻿using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class BrimstoneCragsBiome : ModBiome
    {
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("BrimstoneCrags") ?? MusicID.Eerie;

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/BrimstoneCragsIcon";
        public override string BackgroundPath => "Terraria/Images/MapBG3";

        public override bool IsBiomeActive(Player player)
        {
            return BiomeTileCounterSystem.BrimstoneCragTiles > 100 && player.ZoneUnderworldHeight;
        }
    }
}
