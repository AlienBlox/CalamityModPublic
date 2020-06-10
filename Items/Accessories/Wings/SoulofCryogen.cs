using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
	[AutoloadEquip(EquipType.Wings)]
    public class SoulofCryogen : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of Cryogen");
            Tooltip.SetDefault("The magic of the ancient ice castle is yours\n" +
                "Counts as wings\n" +
                "Horizontal speed: 6.25\n" +
                "Acceleration multiplier: 1\n" +
                "Average vertical speed\n" +
                "Flight time: 120\n" +
                "7% increase to all damage\n" +
                "All melee attacks and projectiles inflict frostburn\n" +
                "Icicles rain down as you fly\n" +
				"Provides heat and cold protection in Death Mode");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 3));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0.3f * num, 0.3f * num);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6.25f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cryogenSoul = true;
            player.allDamage += 0.07f;
            player.wingTimeMax = 120;
            player.noFallDmg = true;
			if (modPlayer.icicleCooldown <= 0)
			{
				if (player.controlJump && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !player.mount.Active && !player.mount.Cart)
				{
					int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, player.velocity.X * 0f, 2f, ModContent.ProjectileType<FrostShardFriendly>(), (int)(25 * player.AverageDamage()), 3f, player.whoAmI, 1f);
					Main.projectile[p].Calamity().forceTypeless = true;
					Main.projectile[p].frame = Main.rand.Next(5);
					modPlayer.icicleCooldown = 10;
				}
			}
        }
    }
}
