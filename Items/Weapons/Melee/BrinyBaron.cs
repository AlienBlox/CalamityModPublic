using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BrinyBaron : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Briny Baron");
            Tooltip.SetDefault("Striking an enemy with the blade causes a briny typhoon to appear\n" +
                "Right click to fire a razorwind aqua blade");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.knockBack = 4f;
            item.useAnimation = item.useTime = 15;
            item.melee = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.shootSpeed = 4f;

            item.width = 100;
            item.height = 102;
			item.scale = 1.5f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
			item.Calamity().challengeDrop = true;
		}

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.noUseGraphic = true;
                item.UseSound = SoundID.Item84;
                item.shoot = ModContent.ProjectileType<Razorwind>();
            }
            else
            {
                item.noMelee = false;
                item.noUseGraphic = false;
                item.UseSound = SoundID.Item1;
                item.shoot = ProjectileID.None;
            }
            return base.CanUseItem(player);
        }

		public override float UseTimeMultiplier	(Player player)
		{
			if (player.altFunctionUse == 2)
				return 1f;
			return 0.75f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Razorwind>(), damage / 2, knockBack, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), (int)(item.damage * player.MeleeDamage()), knockback, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), (int)(item.damage * player.MeleeDamage()), item.knockBack, player.whoAmI);
        }
    }
}
