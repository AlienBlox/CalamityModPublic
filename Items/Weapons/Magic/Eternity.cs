using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Eternity : ModItem
    {
        public const int BaseDamage = 6969;
        public const int ExplosionDamage = 42000;
        public const int MaxHomers = 40;
        public const int dustID = 16;
        public static readonly Color blueColor = new Color(34, 34, 160);
        public static readonly Color pinkColor = new Color(169, 30, 184);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Tooltip.SetDefault("Hexes a possible nearby enemy, trapping them in a brilliant display of destruction\n" +
                               "This line is modified in ModifyTooltips");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
            item.mana = 30;
            item.width = 38;
            item.height = 40;
            item.useTime = item.useAnimation = 120;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Developer;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<EternityBook>();
            item.channel = true;
            item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = $"[" + DisoHex + "There's pictures of ponies in the book]";
                }
            }
        }
        public static string DisoHex => "c/" +
            ((int)(156 + Main.DiscoR * 99f / 255f)).ToString("X2") 
            + 108.ToString("X2") + 251.ToString("X2") + ":";
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeethingDischarge>());
            recipe.AddIngredient(ModContent.ItemType<TomeofFates>());
            recipe.AddIngredient(ModContent.ItemType<GammaFusillade>());
            recipe.AddIngredient(ModContent.ItemType<PrimordialAncient>());
            recipe.AddIngredient(ModContent.ItemType<SubsumingVortex>());
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 10);
            recipe.AddIngredient(ItemID.UnicornHorn, 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}