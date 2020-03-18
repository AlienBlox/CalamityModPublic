using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CosmicViperEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Viper Engine");
            Tooltip.SetDefault("Summons a cosmic gunship to shoot down your foes");
        }

        public override void SetDefaults()
        {
            item.damage = 280;
            item.mana = 10;
            item.width = 46;
            item.height = 28;
            item.useTime = item.useAnimation = 10;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.UseSound = SoundID.Item15; //phaseblade sound effect
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CosmicViperSummon>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.rare = 10;
			item.Calamity().postMoonLordRarity = 14;
        }

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
				int i = Main.myPlayer;
				float num72 = item.shootSpeed;
				float knockback = knockBack;
				knockback = player.GetWeaponKnockback(item, knockback);
				player.itemTime = item.useTime;
				Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
				float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
				float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
				if (player.gravDir == -1f)
				{
					num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
				}
				float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
				if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
				{
					num78 = (float)player.direction;
					num79 = 0f;
					num80 = num72;
				}
				else
				{
					num80 = num72 / num80;
				}
				num78 *= num80;
				num79 *= num80;
				vector2.X = (float)Main.mouseX + Main.screenPosition.X;
				vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
				Vector2 spinningpoint = new Vector2(num78, num79);
				spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
				Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 1f);
			}
			return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlackHawkRemote>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
