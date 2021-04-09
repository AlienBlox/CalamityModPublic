using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class StellarContemptMelee : ModItem
    {
        public static int BaseDamage = 300;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Contempt");
            Tooltip.SetDefault("Lunar flares rain down on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.height = 64;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useTime = 13;
            item.useAnimation = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;

			item.shoot = ModContent.ProjectileType<StellarContemptHammer>();
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddIngredient(ModContent.ItemType<TruePaladinsHammerMelee>());
            r.AddIngredient(ItemID.LunarBar, 5);
            r.AddIngredient(ItemID.FragmentSolar, 10);
            r.AddIngredient(ItemID.FragmentNebula, 10);
            r.AddTile(TileID.LunarCraftingStation);
            r.AddRecipe();
        }
    }
}
