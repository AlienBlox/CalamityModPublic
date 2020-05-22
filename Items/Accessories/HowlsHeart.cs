using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
	public class HowlsHeart : ModItem
	{
		public const int HowlDamage = 45;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Howl's Heart");
			Tooltip.SetDefault("Summons Howl to fight for you, Calcifer to light your way, and Turnip-Head to follow you around");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 26;
			item.value = CalamityGlobalItem.Rarity4BuyPrice;
			item.rare = 4;
			item.Calamity().customRarity = CalamityRarity.Dedicated;
			item.accessory = true;
		}

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().howlsHeart;

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.howlsHeart = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(BuffType<HowlTrio>()) == -1)
				{
					player.AddBuff(BuffType<HowlTrio>(), 3600, true);
				}
				if (player.ownedProjectileCounts[ProjectileType<HowlsHeartHowl>()] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ProjectileType<HowlsHeartHowl>(), (int)(HowlDamage * player.MinionDamage()), 1f, player.whoAmI, 0f, 0f);
				}
				if (player.ownedProjectileCounts[ProjectileType<HowlsHeartCalcifer>()] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ProjectileType<HowlsHeartCalcifer>(), 0, 0f, player.whoAmI, 0f, 0f);
				}
				if (player.ownedProjectileCounts[ProjectileType<HowlsHeartTurnipHead>()] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ProjectileType<HowlsHeartTurnipHead>(), 0, 0f, player.whoAmI, 0f, 0f);
				}
			}
		}
	}
}
