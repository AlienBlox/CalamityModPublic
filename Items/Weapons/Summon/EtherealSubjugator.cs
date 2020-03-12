using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EtherealSubjugator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Subjugator");
            Tooltip.SetDefault("Summons a phantom to protect you");
        }

        public override void SetDefaults()
        {
            item.damage = 75;
            item.mana = 10;
            item.width = 66;
            item.height = 70;
            item.useTime = item.useAnimation = 10;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item82;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PhantomGuy>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(33f, 33f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Summon/EtherealSubjugatorGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
				int i = Main.myPlayer;
				float num72 = item.shootSpeed;
				float num74 = knockBack;
				num74 = player.GetWeaponKnockback(item, num74);
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
				spinningpoint = spinningpoint.RotatedBy(1.5707963705062866, default);
				Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, num74, i, 0f, 1f);
			}
			return false;
        }
    }
}
