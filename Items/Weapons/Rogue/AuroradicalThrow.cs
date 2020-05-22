using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AuroradicalThrow : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auroradical Throw");
            Tooltip.SetDefault("Launches a star that splits after a short period of time\n" +
							"Split stars home in on nearby enemies after a few seconds\n" +
							"Stealth strikes summon a meteor upon enemy impact");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.height = 58;
            item.damage = 32;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 30;
            item.useStyle = 1;
            item.useTime = 30;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<AuroradicalSplitter>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int star = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
			Main.projectile[star].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			Texture2D glowmask = Main.itemTexture[item.type];
            Vector2 origin = new Vector2(glowmask.Width / 2f, glowmask.Height / 2f - 2f);
            spriteBatch.Draw(glowmask, item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
