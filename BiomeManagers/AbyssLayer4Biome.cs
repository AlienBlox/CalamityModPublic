﻿using CalamityMod.CalPlayer;
using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer4Biome : ModBiome
    {
        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("AbyssLayer4") ?? MusicID.Hell;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/VoidWater");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer4Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer4";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer4";

        public override bool IsBiomeActive(Player player)
        {
            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && BiomeTileCounterSystem.Layer4Tiles >= 200 &&
            playerYTileCoords > Main.rockLayer + Main.maxTilesY * 0.268;
        }
    }
}
