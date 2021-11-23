using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class HeartoftheElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of the Elements");
            Tooltip.SetDefault("The heart of the world\n" +
                "Summons all elementals to protect you");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 8));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

		public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.brimstoneWaifu || modPlayer.sandWaifu || modPlayer.sandBoobWaifu || modPlayer.cloudWaifu || modPlayer.sirenWaifu)
            {
                return false;
            }
            return true;
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player != null && !player.dead)
				Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, Main.DiscoR / 255f, Main.DiscoG / 255f, Main.DiscoB / 255f);

			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.allWaifus = true;
			modPlayer.elementalHeart = true;

			int brimmy = ProjectileType<BrimstoneElementalMinion>();
			int siren = ProjectileType<WaterElementalMinion>();
			int healer = ProjectileType<SandElementalHealer>();
			int sandy = ProjectileType<SandElementalMinion>();
			int cloudy = ProjectileType<CloudElementalMinion>();

			Vector2 velocity = new Vector2(0f, -1f);
			int elementalDmg = (int)(90 * player.MinionDamage());
			float kBack = 2f + player.minionKB;

			if (player.ownedProjectileCounts[brimmy] > 1 || player.ownedProjectileCounts[siren] > 1 ||
				player.ownedProjectileCounts[healer] > 1 || player.ownedProjectileCounts[sandy] > 1 ||
				player.ownedProjectileCounts[cloudy] > 1)
			{
				player.ClearBuff(BuffType<HotE>());
			}
			if (player != null && player.whoAmI == Main.myPlayer && !player.dead)
			{
				if (player.FindBuffIndex(BuffType<HotE>()) == -1)
				{
					player.AddBuff(BuffType<HotE>(), 3600, true);
				}
				if (player.ownedProjectileCounts[brimmy] < 1)
				{
					Projectile.NewProjectile(player.Center, velocity, brimmy, elementalDmg, kBack, player.whoAmI);
				}
				if (player.ownedProjectileCounts[siren] < 1)
				{
					Projectile.NewProjectile(player.Center, velocity, siren, elementalDmg, kBack, player.whoAmI);
				}
				if (player.ownedProjectileCounts[healer] < 1)
				{
					Projectile.NewProjectile(player.Center, velocity, healer, elementalDmg, kBack, player.whoAmI);
				}
				if (player.ownedProjectileCounts[sandy] < 1)
				{
					Projectile.NewProjectile(player.Center, velocity, sandy, elementalDmg, kBack, player.whoAmI);
				}
				if (player.ownedProjectileCounts[cloudy] < 1)
				{
					Projectile.NewProjectile(player.Center, velocity, cloudy, elementalDmg, kBack, player.whoAmI);
				}
			}
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<WifeinaBottle>());
            recipe.AddIngredient(ItemType<WifeinaBottlewithBoobs>());
            recipe.AddIngredient(ItemType<LureofEnthrallment>());
            recipe.AddIngredient(ItemType<EyeoftheStorm>());
            recipe.AddIngredient(ItemType<RoseStone>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
