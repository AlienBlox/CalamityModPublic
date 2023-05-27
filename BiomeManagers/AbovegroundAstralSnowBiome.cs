﻿using CalamityMod.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralSnowBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSnowSurfaceBGStyle");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralInfection") ?? MusicID.Space;

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && player.ZoneSnow;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["CalamityMod:AstralSnow"] != null && isActive != SkyManager.Instance["CalamityMod:AstralSnow"].IsActive())
            {
                if (isActive)
                {
                    SkyManager.Instance.Activate("CalamityMod:AstralSnow");
                }
                else
                {
                    SkyManager.Instance.Deactivate("CalamityMod:AstralSnow");
                }
            }
        }

    }
}
