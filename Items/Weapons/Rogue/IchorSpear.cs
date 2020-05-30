using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class IchorSpear : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Spear");
            Tooltip.SetDefault("Throws an ichor tipped trident\n" +
			"Stealth strikes are showered in splashes of ichor");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 40;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 52;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<IchorSpearProj>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                Main.projectile[stealth].usesLocalNPCImmunity = true;
                return false;
            }
            return true;
        }
    }
}
