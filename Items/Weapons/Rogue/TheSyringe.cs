using CalamityMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TheSyringe : RogueWeapon
    {
        public static int BaseDamage = 60;
        public static float Knockback = 5f;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Syringe");
            Tooltip.SetDefault("Throws a high velocity syringe that increases damage as it travels\n" +
				"Shatters into glass and plague cinders on impact\n" +
				"Stealth strikes also shatter into plague bees");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.knockBack = Knockback;
            item.autoReuse = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.width = 14;
            item.height = 50;
            item.UseSound = SoundID.Item106;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<TheSyringeProj>();
            item.shootSpeed = Speed;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<TheSyringeProj>(), damage, knockBack, player.whoAmI, 0f, 1f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
