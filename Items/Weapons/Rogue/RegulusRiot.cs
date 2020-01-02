using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RegulusRiot : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regulus Riot");
            Tooltip.SetDefault(@"Fires a swift homing disk
Stealth strikes explode into energy stars");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 101;
            item.knockBack = 4.5f;

            item.width = 28;
            item.height = 34;
            item.useStyle = 1;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.value = Item.buyPrice(0, 95, 0, 0);
            item.useTime = 26;
            item.useAnimation = 26;
            item.UseSound = SoundID.Item1;
            item.rare = 9;
            item.Calamity().rogue = true;

            item.autoReuse = true;
            item.shootSpeed = 8f;
            item.shoot = ModContent.ProjectileType<RegulusRiotProj>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
