using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GhoulishGouger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghoulish Gouger");
            Tooltip.SetDefault("Throws a ghoulish scythe that ignores immunity frames\n" +
			"Stealth strikes spawn souls on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 74;
            item.damage = 110;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 68;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<GhoulishGougerBoomerang>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(37f, 32f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/GhoulishGougerGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return true;
        }
	}
}
