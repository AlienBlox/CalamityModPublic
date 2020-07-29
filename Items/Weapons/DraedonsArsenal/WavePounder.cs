using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class WavePounder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Pounder");
            Tooltip.SetDefault("Throws a bomb which explodes into a forceful shockwave\n" +
                               "Stealth Strikes emit absurdly powerful shockwaves");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 100;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 26;
            item.height = 44;
            item.useTime = 56;
            item.useAnimation = 56;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0f;

            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;

            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<WavePounderProjectile>();

			item.Calamity().Chargeable = true;
			item.Calamity().ChargeMax = 190;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f).Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 18);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
