using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Waters
{
    public class AstralWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).ZoneAstral;
        }
        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("AstralWaterflow");
        }
        public override int GetSplashDust()
        {
            return 52; //corruption water?
        }
        public override int GetDropletGore()
        {
            return mod.GetGoreSlot("Gores/AstralWaterDroplet");
        }
        public override Color BiomeHairColor()
        {
            return Color.MediumPurple;
        }
    }
}