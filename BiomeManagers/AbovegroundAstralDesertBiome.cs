﻿using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Systems;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralDesertBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralDesertSurfaceBGStyle");
        public override int BiomeTorchItemType => ModContent.ItemType<AstralTorch>();
        public override SceneEffectPriority Priority => Sandstorm.Happening ? SceneEffectPriority.Environment : SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AstralDesertIcon";
        // Could use its own unique background
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralInfection") ?? MusicID.Space;

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && player.ZoneDesert && !player.ZoneSnow;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["CalamityMod:AstralDesert"] != null && isActive != SkyManager.Instance["CalamityMod:AstralDesert"].IsActive())
            {
                if (isActive)
                {
                    SkyManager.Instance.Activate("CalamityMod:AstralDesert");
                }
                else
                {
                    SkyManager.Instance.Deactivate("CalamityMod:AstralDesert");
                }
            }
        }
    }
}
